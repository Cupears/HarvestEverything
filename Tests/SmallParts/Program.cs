using System.Xml;
using System.Xml.Linq;
using Harvest_Everything;
using Verse;
using RimWorld;
using HarmonyLib;
int checks=0;
void Check(bool ok,string msg) {if(!ok)throw new Exception(msg);checks++;}
AccessTools.Field(typeof(Prefs),"data").SetValue(null,new PrefsData());
var patchDoc=new XmlDocument();patchDoc.Load(Path.Combine(args[1],"Patches","SmallNaturalParts.xml"));
var defs=new XmlDocument();defs.LoadXml("<Defs><BodyPartDef><defName>Finger</defName></BodyPartDef><BodyPartDef><defName>Toe</defName></BodyPartDef></Defs>");
foreach(XmlNode node in patchDoc.DocumentElement.ChildNodes)if(node.Name=="Operation")Check(DirectXmlToObject.ObjectFromXml<PatchOperation>(node,false).Apply(defs),"patch applies");
Check(defs.SelectNodes("/Defs/ThingDef").Count==2,"two digit items");
Check(defs.SelectNodes("/Defs/RecipeDef").Count==2,"two install recipes");
string once=defs.OuterXml;
foreach(XmlNode node in patchDoc.DocumentElement.ChildNodes)if(node.Name=="Operation")Check(DirectXmlToObject.ObjectFromXml<PatchOperation>(node,false).Apply(defs),"repeat patch applies");
Check(defs.OuterXml==once,"idempotent patches");
var custom=new XmlDocument();custom.LoadXml("<Defs><BodyPartDef><defName>Finger</defName><spawnThingOnRemoved>OtherFinger</spawnThingOnRemoved></BodyPartDef></Defs>");
foreach(XmlNode node in patchDoc.DocumentElement.ChildNodes)if(node.Name=="Operation")Check(DirectXmlToObject.ObjectFromXml<PatchOperation>(node,false).Apply(custom),"optional definitions");
Check(custom.SelectSingleNode("//spawnThingOnRemoved").InnerText=="OtherFinger","other mod mapping retained");
var finger=new BodyPartRecord{def=new BodyPartDef{defName="Finger"},parts=new()};
Check(SmallPartAutopsy.IsDigit(finger),"finger detection");
Check(!SmallPartAutopsy.IsDigit(new BodyPartRecord{def=new BodyPartDef{defName="MechanicalFinger"}}),"mechanical digits excluded");
Check(Math.Abs(SmallPartAutopsy.EffectiveDigitChance(1,1)-.2f)<.00001,"20 percent cap");
Check(Math.Abs(SmallPartAutopsy.EffectiveDigitChance(.3f,.4f)-.06f)<.00001,"skill scaling");
Check(Math.Abs(SmallPartAutopsy.EffectiveDigitChance(1,.4f)-.08f)<.00001,"autopsy cap scaling");
Check(SmallPartAutopsy.EffectiveDigitChance(0,1)==0,"no skill no recovery");
var race=new ThingDef{defName="FixtureHuman",race=new RaceProperties{intelligence=Intelligence.Humanlike},thingClass=typeof(Pawn)};
var pawn=new Pawn{def=race};pawn.health=new Pawn_HealthTracker(pawn);
var corpse=new Corpse();corpse.InnerPawn=pawn;
var info=new FixtureInfo(.4f,7500,.9f,99,.5f);
object[] callArgs={corpse,info,finger,1f,new List<Thing>(),new List<BodyPartRecord>()};
SmallPartAutopsy.ScaleDigitChance(callArgs);
Check(Math.Abs(((FixtureInfo)callArgs[1]).NaturalChance-.08f)<.00001,"per-call natural chance");
Check(Math.Abs((float)callArgs[3]-.2f)<.00001,"per-call skill");
Check(info.NaturalChance==.4f,"original settings unchanged");
Check(((FixtureInfo)callArgs[1]).CorpseValidAge==7500 && ((FixtureInfo)callArgs[1]).PartNumber==99,"age and total limits retained");
Check(((FixtureInfo)callArgs[1]).BionicChance==.9f,"bionic cap retained");
pawn.health.hediffSet.hediffs.Add(new Hediff{pawn=pawn,Part=finger,def=new HediffDef()});
callArgs=new object[]{corpse,info,finger,1f,new List<Thing>(),new List<BodyPartRecord>()};
SmallPartAutopsy.ScaleDigitChance(callArgs);
Check(ReferenceEquals(callArgs[1],info),"damaged or implanted digit untouched by chance patch");
var hand=new BodyPartRecord {def=new BodyPartDef{defName="Hand"},parts=new(){finger}};finger.parent=hand;
// For this isolated fixture, patch only the missing-part lookup; direct injuries
// still use real HediffSet records and the production descendant code.
new Harmony("smallparts.fixture").Patch(AccessTools.Method(typeof(HediffSet),"PartIsMissing"),prefix:new HarmonyMethod(typeof(FixtureMissing),nameof(FixtureMissing.Prefix)));
Check(SmallPartAutopsy.HasDamagedDescendant(pawn,hand),"injured digit blocks intact hand recovery");
pawn.health.hediffSet.hediffs.Clear();
Check(!SmallPartAutopsy.HasDamagedDescendant(pawn,hand),"healthy hand remains recoverable");
FixtureMissing.Missing=finger;
Check(SmallPartAutopsy.HasDamagedDescendant(pawn,hand),"missing digit blocks intact hand recovery");
FixtureMissing.Missing=null;
var assembly=System.Reflection.Assembly.LoadFrom(args[0]);
var utility=assembly.GetType("Autopsy.NewMedicalRecipesUtility",true);
var harmony=new Harmony("smallparts.bindingcheck");
harmony.Patch(AccessTools.Method(utility,"IsCleanAndDroppable"),postfix:new HarmonyMethod(typeof(SmallPartAutopsy),nameof(SmallPartAutopsy.CheckChildren)));
Check(true,"actual Post Mortem clean patch binds");
harmony.Patch(AccessTools.Method(utility,"TryGetParts"),prefix:new HarmonyMethod(typeof(SmallPartAutopsy),nameof(SmallPartAutopsy.ScaleDigitChance)));
Check(true,"actual Post Mortem chance patch binds");
Console.WriteLine($"{checks} small-part checks passed.");
public class FixtureInfo {
 public readonly float NaturalChance,BionicChance,FrozenDecay;public readonly int CorpseValidAge,PartNumber;
 public FixtureInfo(float naturalChance,int corpseValidAge,float bionicChance,int partNumber,float frozenDecay) {NaturalChance=naturalChance;CorpseValidAge=corpseValidAge;BionicChance=bionicChance;PartNumber=partNumber;FrozenDecay=frozenDecay;}
}



public static class FixtureMissing {public static BodyPartRecord Missing; public static bool Prefix(BodyPartRecord part,ref bool __result) {__result=part==Missing;return false;}}
