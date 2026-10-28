using System.Xml.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Harvest_Everything;
int checks=0;
void Check(bool ok,string label){if(!ok)throw new Exception(label);checks++;}
AccessTools.Field(typeof(Prefs),"data").SetValue(null,new PrefsData());
new Harmony("safety.fixture").Patch(AccessTools.Method(typeof(HediffSet),"PartIsMissing"),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.MissingPart)));
new Harmony("safety.clean").CreateClassProcessor(typeof(TailMetadataClean)).Patch();
new Harmony("safety.noanomaly").Patch(AccessTools.PropertyGetter(typeof(RaceProperties),"IsAnomalyEntity"),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.NoAnomaly)));
var human=new Pawn{def=new ThingDef{defName="FixtureHuman",label="human",race=new RaceProperties{intelligence=Intelligence.Humanlike}}};human.health=new Pawn_HealthTracker(human);
var root=new BodyPartRecord{def=new BodyPartDef{defName="Torso"},parts=new()};
var tail=new BodyPartRecord{def=new BodyPartDef{defName="Tail",label="tail"},parent=root,parts=new()};root.parts.Add(tail);
human.health.hediffSet.hediffs.Add(new Hediff_TailOrigin{pawn=human,Part=tail,origin=new TailOrigin{race="Dog",label="dog",part="Tail"}});
Check(MedicalRecipesUtility.IsClean(human,tail),"transplanted healthy tail remains clean for native surgery");
human.health.hediffSet.hediffs.Add(new Hediff{pawn=human,Part=tail,def=new HediffDef()});
Check(!MedicalRecipesUtility.IsClean(human,tail),"real injury remains dirty");human.health.hediffSet.hediffs.RemoveAt(1);
Fixture.Missing=tail;Check(!MedicalRecipesUtility.IsClean(human,tail),"metadata cannot make a missing tail recoverable");Fixture.Missing=null;
var animal=new Pawn{def=new ThingDef{defName="ModdedAnimal",label="modded animal",race=new RaceProperties()}};animal.health=new Pawn_HealthTracker(animal);
var paw=new BodyPartRecord{def=new BodyPartDef{defName="Paw",label="paw"},parent=root,parts=new()};
var item=new Thing_AnimalOrgan();item.SetSourceRace(animal.def);Check(!item.Fits(animal,paw),"unknown donor anatomy rejected");item.SetAnimalPart(animal,paw);Check(item.Fits(animal,paw),"known donor accepted");
Check(!item.Fits(animal,new BodyPartRecord{def=new BodyPartDef{defName="OtherPaw",label="paw"}}),"same display label cannot bypass anatomy");
Check(!item.Fits(new Pawn{def=new ThingDef{defName="OtherAnimal"}},paw),"species mismatch rejected");
Check(!AnimalButcherParts.ShouldApply(animal),"living animal cannot be butcher-extracted");
Check(AnimalButcherParts.Extract(animal,1).Count==0,"direct extraction cannot farm a living animal");
foreach(float bad in new[]{float.NaN,float.PositiveInfinity,float.NegativeInfinity,-1f,0f})
 Check(AnimalButcherParts.RecoveryChance(paw,bad)==0,"invalid efficiency rejected");
Check(AnimalButcherParts.RecoveryChance(paw,float.MaxValue)<=1,"chance capped");
animal.health.hediffSet.hediffs.Add(new Hediff_AddedPart{pawn=animal,Part=root,def=new HediffDef()});
Check(!AnimalButcherParts.IsPartClean(animal,paw),"no natural parts harvested beneath a prosthetic ancestor");animal.health.hediffSet.hediffs.Clear();
var shoulder=new BodyPartRecord{def=new BodyPartDef{defName="Shoulder"},parent=root,parts=new()};
var arm=new BodyPartRecord{def=new BodyPartDef{defName="Arm"},parent=shoulder,parts=new()};
var wrist=new BodyPartRecord{def=new BodyPartDef{defName="Wrist"},parent=arm,parts=new()};
var finger=new BodyPartRecord{def=new BodyPartDef{defName="Finger"},parent=wrist,parts=new()};
Check(HealthCardUtility_CreateSurgeryBill_Patch.HasSelectedAncestor(finger,new[]{shoulder}),"deep modded limbs cannot queue parent and child removal");
Check(HealthCardUtility_CreateSurgeryBill_Patch.OrderFor(null)==4,"missing tags handled");
new Harmony("safety.ids.fixture").Patch(AccessTools.Method(typeof(Bill),"InitializeAfterClone"),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.NoIds)));
RecipeDefOf.RemoveBodyPart=new RecipeDef{defName="RemoveFixturePart",defaultIngredientFilter=new ThingFilter()};
Fixture.HarvestParts=new(){shoulder,finger};
new Harmony("safety.bulkfixture").Patch(AccessTools.Method(typeof(HealthCardUtility_CreateSurgeryBill_Patch),nameof(HealthCardUtility_CreateSurgeryBill_Patch.GetHarvestablePartsToApplyOn)),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.Harvest)));
var bulk=new RecipeDef{defName="HarvestEverything"};
HealthCardUtility_CreateSurgeryBill_Patch.Prefix(animal,bulk,null);
Check(animal.BillStack.Bills.Count==1,"bulk removal queues only intact ancestor");
Check(((Bill_Medical)animal.BillStack.Bills[0]).Part==shoulder,"bulk bill receives target after attaching");
HealthCardUtility_CreateSurgeryBill_Patch.Prefix(animal,bulk,null);Check(animal.BillStack.Bills.Count==1,"repeat bulk click cannot duplicate bills");
foreach(var patch in new[]{typeof(AnimalPartMenuAvailability),typeof(AnimalPartBillAvailability),typeof(AnimalTransplantIngredients)}){new Harmony("safety.bind."+patch.Name).CreateClassProcessor(patch).Patch();Check(true,"bind "+patch.Name);}
// Exercise the native removal worker and custom replacement targeting against
// a real body tree; fixtures replace only map-independent missing/mutant state.
new Harmony("safety.mutant.fixture").Patch(AccessTools.PropertyGetter(typeof(Pawn),"IsMutant"),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.NoAnomaly)));
var body=new BodyDef{defName="FixtureBody",corePart=root};root.body=body;tail.body=body;paw.body=body;
root.parts.Add(paw);body.AllParts.AddRange(new[]{root,tail,paw});human.def.race.body=body;animal.def.race.body=body;
AccessTools.Field(typeof(RaceProperties),"fleshType").SetValue(animal.RaceProps,new FleshTypeDef());
tail.def.spawnThingOnRemoved=new ThingDef{defName="FixtureTailItem"};paw.def.spawnThingOnRemoved=new ThingDef{defName="FixturePawItem"};
var removal=new RecipeDef{defName="FixtureTailRemoval",label="remove tail",workerClass=typeof(Recipe_RemoveDefinedParts),appliedOnFixedBodyParts=new(){tail.def}};
var targets=removal.Worker.GetPartsToApplyOn(human,removal).ToList();
Check(targets.Contains(tail),"healthy donor-tracked tail can be removed again");
Check(!targets.Contains(paw),"tail removal cannot target a paw or another body part");
var replacement=new RecipeDef{defName="HET_InstallAnimalPart",workerClass=typeof(Recipe_InstallAnimalPart)};
animal.health.hediffSet.hediffs.Add(new Hediff{pawn=animal,Part=paw,def=new HediffDef()});
Check(replacement.Worker.GetPartsToApplyOn(animal,replacement).Contains(paw),"injured animal paw is replaceable");
animal.health.hediffSet.hediffs.Add(new Hediff_AddedPart{pawn=animal,Part=root,def=new HediffDef()});
Check(!replacement.Worker.GetPartsToApplyOn(animal,replacement).Contains(paw),"natural replacement beneath prosthetic parent rejected");
animal.health.hediffSet.hediffs.Clear();animal.health.hediffSet.hediffs.Add(new Hediff_AddedPart{pawn=animal,Part=paw,def=new HediffDef()});
Check(replacement.Worker.GetPartsToApplyOn(animal,replacement).Contains(paw),"prosthetic itself can be replaced with a matching natural donor");
animal.health.hediffSet.hediffs.Clear();
foreach(var type in new[]{typeof(ThoughtWorker_Precept_KurinWoolApparel),typeof(ThoughtWorker_Precept_ReviaWoolApparel)}) {
 var state=(ThoughtState)AccessTools.Method(type,"CurrentStateInternal").Invoke(Activator.CreateInstance(type),new object[]{new Pawn()});
 Check(!state.Active,"apparel-free pawn safe for "+type.Name);
}
foreach(var type in new[]{typeof(ThoughtWorker_KurinWoolWorn_Disapproved),typeof(ThoughtWorker_ReviaWoolWorn)}) {
 var state=(ThoughtState)AccessTools.Method(type,"CurrentSocialStateInternal").Invoke(Activator.CreateInstance(type),new object[]{new Pawn(),new Pawn()});
 Check(!state.Active,"apparel-free hybrid social thought safe for "+type.Name);
}
Check(TailOrigins.FromItem(null)==null,"missing tail ingredient is safe");
Check(TailOrigins.FromPawn(null,null)==null,"missing donor is safe");
string project=args[0];
var animals=XDocument.Load(Path.Combine(project,"Defs/AnimalOrgans.xml"));
var items=animals.Descendants("ThingDef").Where(x=>x.Element("defName")!=null).Select(x=>(string)x.Element("defName")).ToHashSet();
var install=animals.Descendants("RecipeDef").Single(x=>(string)x.Element("defName")=="HET_InstallAnimalPart");
foreach(var name in items.Where(x=>x!="HET_AnimalTail")){
 Check(install.Element("ingredients").Descendants("thingDefs").Elements("li").Any(x=>x.Value==name),"donor item in ingredient filter: "+name);
 Check(install.Element("fixedIngredientFilter").Descendants("thingDefs").Elements("li").Any(x=>x.Value==name),"donor item in fixed filter: "+name);
}
foreach(var f in new[]{"1.6/Patches/Patches.xml","1.7/Patches/Patches.xml"})
 foreach(var ing in XDocument.Load(Path.Combine(project,f)).Descendants("RecipeDef").Elements("ingredients"))
  if(ing.Descendants("categories").Elements("li").Any(x=>x.Value=="Medicine"))Check((string)ing.Attribute("Inherit")=="False","complete medicine lists replace inherited lists");
var tagNames=new[]{"BloodPumpingSource","BreathingSource","BloodFiltrationKidney","BloodFiltrationLiver","MetabolismSource","ConsciousnessSource","SightSource","HearingSource","MovingLimbDigit","ManipulationLimbDigit","MovingLimbCore","MovingLimbSegment","ManipulationLimbCore","ManipulationLimbSegment"};
var tags=tagNames.ToDictionary(n=>n,n=>new BodyPartTagDef{defName=n});
foreach(var pair in tags)AccessTools.Field(typeof(BodyPartTagDefOf),pair.Key).SetValue(null,pair.Value);
foreach(var expected in new[]{("Heart","HET_AnimalHeart"),("Ear","HET_AnimalEar"),("FootClaw","HET_AnimalClaw"),("AnimalJaw","HET_AnimalJaw"),("HeadWithEarHoles",(string)null),("MechanicalLeg",(string)null),("Tailbone",(string)null),("Skull",(string)null)})
 Check(AnimalPartCatalog.ItemName(new BodyPartDef{defName=expected.Item1})==expected.Item2,"anatomy classification: "+expected.Item1);
Check(AnimalPartCatalog.ItemName(new BodyPartDef{defName="Mod_Organ",tags=new(){tags["BloodPumpingSource"]}})=="HET_AnimalHeart","custom organ mapped by actual function");
Check(AnimalButcherParts.RecoveryChance(new BodyPartRecord{def=new BodyPartDef{defName="CustomDigit",tags=new(){tags["ManipulationLimbDigit"]}}},1)==.05f,"custom digits use the small-part rate");
foreach(var expected in new[]{("Pincer","HET_AnimalClaw"),("Plastron","HET_AnimalShell"),("AA_LeftCreatureLimb","HET_AnimalLimb"),("Antenna","HET_AnimalAntenna"),("Spine","HET_AnimalSpine"),("Fin","HET_AnimalFin"),("Toe","HET_AnimalToe"),("Finger","HET_AnimalFinger")})
 Check(AnimalPartCatalog.ItemName(new BodyPartDef{defName=expected.Item1})==expected.Item2,"extra anatomy mapping: "+expected.Item1);
var iconXml=XDocument.Load(Path.Combine(project,"Integrations/BionicIcons/Defs/BodyPartIcons.xml"));
string Icon(string id)=>(string)iconXml.Descendants("BionicIconsIconDef").Single(x=>(string)x.Element("thingDef")==id).Element("texture");
Check(Icon("HET_AnimalPaw").EndsWith("AnimalLeg"),"paw uses animal fallback rather than human foot");
Check(Icon("HET_AnimalHoof").EndsWith("AnimalLeg"),"hoof uses animal fallback");
Check(Icon("HET_AnimalLiver").EndsWith("/Liver"),"liver keeps organ icon");
Check(Icon("HET_AnimalEar").EndsWith("AnimalEar"),"animal ear keeps animal artwork");
if(args.Length>1){
 var resolved=XDocument.Load(args[1]);int mapped=0,unmapped=0;var unsupported=new List<string>();
 foreach(var xml in resolved.Descendants("BodyPartDef").Where(x=>x.Element("defName")!=null)) {
  var part=new BodyPartDef{defName=(string)xml.Element("defName"),tags=xml.Element("tags")?.Elements("li").Where(x=>tags.ContainsKey(x.Value)).Select(x=>tags[x.Value]).ToList()};
  var id=AnimalPartCatalog.ItemName(part);if(id==null){unmapped++;unsupported.Add(part.defName);continue;}
  Check(items.Contains(id),"mapped body part has a real item: "+part.defName);mapped++;
 }
 if(args.Length>2)File.WriteAllLines(args[2],unsupported.Distinct().OrderBy(n=>n));
 Console.WriteLine($"Resolved anatomy: {mapped} supported part types; {unmapped} structural, mechanical or unmapped types left excluded.");
}
Console.WriteLine($"{checks} safety audit checks passed.");
public static class Fixture{
 public static BodyPartRecord Missing;public static List<BodyPartRecord> HarvestParts;
 public static bool MissingPart(BodyPartRecord part,ref bool __result){__result=part==Missing;return false;}
 public static bool NoIds()=>false;
 public static bool NoAnomaly(ref bool __result){__result=false;return false;}
 public static bool Harvest(ref IEnumerable<BodyPartRecord> __result){__result=HarvestParts;return false;}
}

