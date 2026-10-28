using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Harvest_Everything;

public class TailOrigin : IExposable
{
 public string race, label, part, partLabel;
 public TailOrigin Copy() => new TailOrigin { race=race,label=label,part=part,partLabel=partLabel };
 public bool Known => !race.NullOrEmpty() && !label.NullOrEmpty() && !part.NullOrEmpty();
 public string Name => label.NullOrEmpty()?"tail":label+" tail";
 public void ExposeData() {
  Scribe_Values.Look(ref race,"race");Scribe_Values.Look(ref label,"label");
  Scribe_Values.Look(ref part,"part");Scribe_Values.Look(ref partLabel,"partLabel");
 }
}
public class CompTailOrigin : ThingComp
{
 public TailOrigin origin;
 public override void PostExposeData() {base.PostExposeData();Scribe_Deep.Look(ref origin,"HET_tailOrigin");if(Scribe.mode==LoadSaveMode.PostLoadInit && origin?.Known!=true)origin=null;}
 public override string TransformLabel(string label) => origin==null || parent is Thing_AnimalOrgan ? label : origin.Name;
 public override bool AllowStackWith(Thing other) {
  var b=other.TryGetComp<CompTailOrigin>()?.origin;
  return origin==null ? b==null : b!=null && b.race==origin.race && b.part==origin.part;
 }
 public override void PostSplitOff(Thing piece) {
  var comp=piece.TryGetComp<CompTailOrigin>();if(comp!=null)comp.origin=origin?.Copy();
 }
}
// Metadata only: no pain, capacity changes, severity effects or visible ailment.
public class Hediff_TailOrigin : Hediff
{
 public TailOrigin origin;
 public override bool Visible => false;
 public override void ExposeData() {base.ExposeData();Scribe_Deep.Look(ref origin,"HET_tailOrigin");}
}
[StaticConstructorOnStartup]
public static class TailOrigins
{
 static TailOrigins() {
  var items=DefDatabase<BodyPartDef>.AllDefsListForReading.Where(IsTail).Select(p=>p.spawnThingOnRemoved)
   .Concat(new[]{DefDatabase<ThingDef>.GetNamedSilentFail("HET_AnimalTail")}).Where(d=>d!=null).Distinct();
  foreach(var def in items) {
   if(def.thingClass==null || !typeof(ThingWithComps).IsAssignableFrom(def.thingClass))continue;
   def.comps??=new List<CompProperties>();
   if(!def.comps.Any(c=>c.compClass==typeof(CompTailOrigin)))def.comps.Add(new CompProperties(typeof(CompTailOrigin)));
  }
  var utility=AccessTools.TypeByName("Autopsy.NewMedicalRecipesUtility");
  if(utility==null)return;
  var extract=AccessTools.Method(utility,"TryGetParts");
  var clean=AccessTools.Method(utility,"IsCleanAndDroppable");
  if(extract?.GetParameters().Length!=6 || clean?.GetParameters().Length!=2) {
   Log.Warning("Harvest Everything: Post Mortem tail tracking skipped because its API has changed.");return;
  }
  var h=new Harmony("cupears.harvesteverything.tailorigin.autopsy");
  h.Patch(extract,prefix:new HarmonyMethod(typeof(TailOrigins),nameof(AutopsyBefore)),
   postfix:new HarmonyMethod(typeof(TailOrigins),nameof(AutopsyAfter)));
  h.Patch(clean,prefix:new HarmonyMethod(typeof(TailOrigins),nameof(AutopsyClean)));
 }
 public static bool IsTail(BodyPartDef part) => part?.defName?.IndexOf("Tail",StringComparison.OrdinalIgnoreCase)>=0 && part.defName.IndexOf("Tailbone",StringComparison.OrdinalIgnoreCase)<0 && !AnimalPartCatalog.Has(part.defName,"Mech") && !AnimalPartCatalog.Has(part.defName,"Artificial");
 public static TailOrigin FromPawn(Pawn pawn,BodyPartRecord part) {
  if(pawn?.def==null || part?.def==null)return null;
  var saved=pawn.health?.hediffSet?.hediffs.OfType<Hediff_TailOrigin>().FirstOrDefault(h=>h.Part==part && h.origin?.Known==true)?.origin;
  return saved?.Copy() ?? new TailOrigin{race=pawn.def.defName,label=pawn.def.label,part=part.def.defName,partLabel=part.def.label};
 }
 public static TailOrigin FromItem(Thing item) {
  if(item==null)return null;
  var origin=item.TryGetComp<CompTailOrigin>()?.origin ?? (item as Thing_AnimalOrgan)?.DonorOrigin();
  return origin?.Known==true?origin.Copy():null;
 }
 public static void Apply(Thing item,TailOrigin origin) {
  if(item==null||origin?.Known!=true)return;
  var comp=item.TryGetComp<CompTailOrigin>();if(comp!=null)comp.origin=origin.Copy();
  if(item is Thing_AnimalOrgan animal)animal.SetDonorOrigin(origin);
 }
 public class AutopsyState {public int count;public TailOrigin origin;}
 public static void AutopsyBefore(object[] __args,out AutopsyState __state) {
  __state=null;
  if(__args?.Length>=5 && __args[0] is Corpse corpse && __args[2] is BodyPartRecord part && IsTail(part.def) && __args[4] is List<Thing> result)
   __state=new AutopsyState{count=result.Count,origin=FromPawn(corpse.InnerPawn,part)};
 }
 public static void AutopsyAfter(object[] __args,AutopsyState __state) {
  if(__state==null||__args?.Length<5||!(__args[4] is List<Thing> result))return;
  for(int i=__state.count;i<result.Count;i++)
   if(result[i].TryGetComp<CompTailOrigin>()!=null)Apply(result[i],__state.origin);
 }
 public static bool AutopsyClean(Pawn pawn,BodyPartRecord part,ref bool __result) {
  if(pawn?.health?.hediffSet==null||!IsTail(part?.def)||!pawn.health.hediffSet.hediffs.Any(h=>h.Part==part && h is Hediff_TailOrigin))return true;
  __result=pawn.RaceProps.Humanlike && !pawn.RaceProps.IsMechanoid && !pawn.RaceProps.IsAnomalyEntity &&
   part.def.spawnThingOnRemoved!=null && !pawn.health.hediffSet.PartIsMissing(part) && !pawn.health.hediffSet.hediffs.Any(h=>h.Part==part && !(h is Hediff_TailOrigin));
  return false;
 }
}
[HarmonyPatch(typeof(MedicalRecipesUtility),nameof(MedicalRecipesUtility.SpawnNaturalPartIfClean))]
public static class TailRemovalOrigin
{
 public static void Prefix(Pawn pawn,BodyPartRecord part,out TailOrigin __state) =>
  __state=TailOrigins.IsTail(part?.def)?TailOrigins.FromPawn(pawn,part):null;
 public static void Postfix(Thing __result,TailOrigin __state) => TailOrigins.Apply(__result,__state);
}
[HarmonyPatch(typeof(Recipe_InstallNaturalBodyPart),nameof(Recipe_InstallNaturalBodyPart.ApplyOnPawn))]
public static class TailInstallOrigin
{
 public class Context {public Pawn pawn;public BodyPartRecord part;public TailOrigin origin;}
 [ThreadStatic] public static Context current;
 public static void Prefix(Pawn pawn,BodyPartRecord part,List<Thing> ingredients,out Context __state) {
  __state=current;current=null;
  if(TailOrigins.IsTail(part?.def) && ingredients!=null) {
   var origin=ingredients.Select(TailOrigins.FromItem).FirstOrDefault(o=>o!=null);
   if(origin!=null)current=new Context{pawn=pawn,part=part,origin=origin};
  }
 }
 public static Exception Finalizer(Exception __exception,Context __state) {current=__state;return __exception;}
}
[HarmonyPatch(typeof(MedicalRecipesUtility),nameof(MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts))]
public static class TailInstalledOrigin
{
 public static void Postfix(Pawn pawn,BodyPartRecord part) {
  var context=TailInstallOrigin.current;
  if(context==null||context.pawn!=pawn||context.part!=part||context.origin?.Known!=true||pawn.health.hediffSet.PartIsMissing(part))return;
  var marker=(Hediff_TailOrigin)HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("HET_TailOrigin"),pawn,part);
  marker.origin=context.origin.Copy();pawn.health.AddHediff(marker,part);
 }
}
[HarmonyPatch(typeof(Recipe_InstallNaturalBodyPart),nameof(Recipe_InstallNaturalBodyPart.GetPartsToApplyOn))]
public static class TailMetadataNotDamage
{
 [HarmonyPriority(Priority.Last)]
 public static IEnumerable<BodyPartRecord> Postfix(IEnumerable<BodyPartRecord> __result,Pawn pawn) {
  foreach(var part in __result??Enumerable.Empty<BodyPartRecord>()) {
   var onPart=pawn.health.hediffSet.hediffs.Where(h=>h.Part==part).ToList();
   if(onPart.Any(h=>h is Hediff_TailOrigin) && !onPart.Any(h=>!(h is Hediff_TailOrigin)) &&
      !SmallPartAutopsy.HasDamagedDescendant(pawn,part))continue;
   yield return part;
  }
 }
}

[HarmonyPatch(typeof(Thing),nameof(Thing.DescriptionFlavor),MethodType.Getter)]
public static class TailItemDescription
{
 public static string Description(TailOrigin origin) {
  string name=origin.Name;
  return ("aeiouAEIOU".IndexOf(name[0])>=0?"An ":"A ")+name+" used for balance, movement or display.";
 }
 public static void Postfix(Thing __instance,ref string __result) {
  var origin=__instance.TryGetComp<CompTailOrigin>()?.origin;
  if(origin!=null)__result=Description(origin);
 }
}
[HarmonyPatch(typeof(Thing),nameof(Thing.DescriptionDetailed),MethodType.Getter)]
public static class TailItemDetailedDescription
{
 public static void Postfix(Thing __instance,ref string __result) {
  var origin=__instance.TryGetComp<CompTailOrigin>()?.origin;
  if(origin!=null && !__instance.def.description.NullOrEmpty() && __result!=null)
   __result=__result.Replace(__instance.def.description,TailItemDescription.Description(origin));
 }
}
