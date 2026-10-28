using Verse;
using RimWorld;
using HarmonyLib;
using Harvest_Everything;
int checks=0;AccessTools.Field(typeof(Prefs),"data").SetValue(null,new PrefsData());
new Harmony("tails.fixture").Patch(AccessTools.PropertyGetter(typeof(RaceProperties),"IsAnomalyEntity"),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.NoAnomaly)));
void Check(bool ok,string why){if(!ok)throw new Exception(why);checks++;}
ThingWithComps Item(){var t=new ThingWithComps{def=new ThingDef{defName="FixtureTail",label="tail",comps=new(){new CompProperties(typeof(CompTailOrigin))}}};t.InitializeComps();return t;}
var dog=new Pawn{def=new ThingDef{defName="Dog",label="dog",race=new RaceProperties()}};dog.health=new Pawn_HealthTracker(dog);
var tail=new BodyPartRecord{def=new BodyPartDef{defName="Tail",label="tail"},parts=new()};
var origin=TailOrigins.FromPawn(dog,tail);Check(origin.race=="Dog","fresh donor identity");
var item=Item();TailOrigins.Apply(item,origin);
Check(item.TryGetComp<CompTailOrigin>().TransformLabel("tail")=="dog tail","source label");
var hybrid=new Pawn{def=new ThingDef{defName="ModdedHybrid",label="hybrid",race=new RaceProperties{intelligence=Intelligence.Humanlike}}};hybrid.health=new Pawn_HealthTracker(hybrid);
var hybridTail=new BodyPartRecord{def=new BodyPartDef{defName="ModdedHybridTail",label="tail"},parts=new()};
var marker=new Hediff_TailOrigin{pawn=hybrid,Part=hybridTail,origin=origin.Copy()};hybrid.health.hediffSet.hediffs.Add(marker);
Check(!marker.Visible,"origin hidden");
Check(TailOrigins.FromPawn(hybrid,hybridTail).race=="Dog","removal retains donor across species");
var removed=Item();TailOrigins.Apply(removed,TailOrigins.FromPawn(hybrid,hybridTail));
Check(removed.TryGetComp<CompTailOrigin>().origin.race=="Dog","recovered tail keeps donor");
Check(item.TryGetComp<CompTailOrigin>().AllowStackWith(removed),"same-source stack accepted");
var foxItem=Item();TailOrigins.Apply(foxItem,new TailOrigin{race="Fox",label="fox",part="Tail"});
Check(!item.TryGetComp<CompTailOrigin>().AllowStackWith(foxItem),"different sources do not merge");
var split=Item();item.TryGetComp<CompTailOrigin>().PostSplitOff(split);
Check(split.TryGetComp<CompTailOrigin>().origin.race=="Dog","split retains source");
Check(!ReferenceEquals(split.TryGetComp<CompTailOrigin>().origin,item.TryGetComp<CompTailOrigin>().origin),"split copies metadata");
TailInstallOrigin.Prefix(hybrid,hybridTail,new(){item},out var previous);
Check(TailInstallOrigin.current.origin.race=="Dog","install captures donor");
var expected=new Exception("fixture");Check(TailInstallOrigin.Finalizer(expected,previous)==expected&&TailInstallOrigin.current==previous,"failure cleans context");
bool clean=false;TailOrigins.AutopsyClean(hybrid,hybridTail,ref clean);
// Humanlike race is required for Post Mortem natural corpse recovery.
hybrid.def.race=new RaceProperties{intelligence=Intelligence.Humanlike};hybridTail.def.spawnThingOnRemoved=item.def;AccessTools.Field(typeof(RaceProperties),"fleshType").SetValue(hybrid.RaceProps,new FleshTypeDef());
TailOrigins.AutopsyClean(hybrid,hybridTail,ref clean);Check(clean,"metadata does not make an autopsy part dirty");
var normal=TailOrigins.FromPawn(hybrid,hybridTail);Check(normal.label=="dog","host label not substituted");
var corpse=new Corpse{InnerPawn=hybrid};var products=new List<Thing>();
object[] callArgs={corpse,new object(),hybridTail,1f,products,new List<BodyPartRecord>()};
TailOrigins.AutopsyBefore(callArgs,out var state);products.Add(Item());TailOrigins.AutopsyAfter(callArgs,state);
Check(products[0].TryGetComp<CompTailOrigin>().origin.race=="Dog","autopsy output keeps donor");
foreach(var pair in new[]{("Finger",.05f),("FrontClaw",.05f),("Horn",.125f),("Hoof",.125f),("Paw",.25f),("Leg",.25f),("ModdedWing",.25f)})
 Check(Math.Abs(AnimalButcherParts.RecoveryChance(new BodyPartRecord{def=new BodyPartDef{defName=pair.Item1}},1)-pair.Item2)<.00001,pair.Item1+" rate");
foreach(var patch in new[]{typeof(TailRemovalOrigin),typeof(TailInstallOrigin),typeof(TailInstalledOrigin),typeof(TailMetadataNotDamage),typeof(TailItemDescription),typeof(TailItemDetailedDescription)}) {
 new Harmony("tails.validation."+patch.Name).CreateClassProcessor(patch).Patch();Check(true,"bind "+patch.Name);
}
new Harmony("tails.missingfixture").Patch(AccessTools.Method(typeof(HediffSet),"PartIsMissing"),prefix:new HarmonyMethod(typeof(Fixture),nameof(Fixture.MissingPart)));
var toe=new BodyPartRecord{def=new BodyPartDef{defName="Toe"},parts=new()};
var paw=new BodyPartRecord{def=new BodyPartDef{defName="Paw"},parts=new(){toe}};toe.parent=paw;
var cleanMethod=AccessTools.Method(typeof(AnimalButcherParts),"IsPartClean");
Check((bool)cleanMethod.Invoke(null,new object[]{dog,paw}),"intact whole part accepted");
dog.health.hediffSet.hediffs.Add(new Hediff{pawn=dog,Part=toe,def=new HediffDef()});
Check(!(bool)cleanMethod.Invoke(null,new object[]{dog,paw}),"damaged child rejects whole part");
dog.health.hediffSet.hediffs.Clear();Fixture.Missing=toe;
Check(!(bool)cleanMethod.Invoke(null,new object[]{dog,paw}),"missing child rejects whole part");Fixture.Missing=null;
Check(!TailOrigins.IsTail(new BodyPartDef{defName="Kiiro_Tailbone"}),"tailbone not treated as donor tail");
string path=Path.Combine(Path.GetTempPath(),"cupears-tail-origin-"+Guid.NewGuid()+".xml");
TailOrigin saved=origin.Copy();
Scribe.saver.InitSaving(path,"tailOriginTest");Scribe_Deep.Look(ref saved,"origin");Scribe.saver.FinalizeSaving();
TailOrigin loaded=null;Scribe.loader.InitLoading(path);Scribe_Deep.Look(ref loaded,"origin");Scribe.loader.FinalizeLoading();
Check(loaded.race=="Dog" && loaded.part=="Tail" && loaded.label=="dog","donor survives Scribe save/load");
File.Delete(path);
if(args.Length>0) {
 var assembly=System.Reflection.Assembly.LoadFrom(args[0]);
 var utility=assembly.GetType("Autopsy.NewMedicalRecipesUtility",true);
 var harmony=new Harmony("tails.autopsy.bindingvalidation");
 harmony.Patch(AccessTools.Method(utility,"TryGetParts"),prefix:new HarmonyMethod(typeof(TailOrigins),nameof(TailOrigins.AutopsyBefore)),postfix:new HarmonyMethod(typeof(TailOrigins),nameof(TailOrigins.AutopsyAfter)));
 Check(true,"Post Mortem donor hooks bind");
 harmony.Patch(AccessTools.Method(utility,"IsCleanAndDroppable"),prefix:new HarmonyMethod(typeof(TailOrigins),nameof(TailOrigins.AutopsyClean)));
 Check(true,"Post Mortem metadata-clean hook binds");
}
Console.WriteLine($"{checks} tail-origin and recovery checks passed.");




public static class Fixture { public static BodyPartRecord Missing; public static bool MissingPart(BodyPartRecord part,ref bool __result){__result=part==Missing;return false;} public static bool NoAnomaly(ref bool __result){__result=false;return false;} }

