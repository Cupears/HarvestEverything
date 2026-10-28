using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
namespace Harvest_Everything;
public class Recipe_InstallAnimalPart : Recipe_InstallNaturalBodyPart
{
 public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part) =>
  part?.def == null ? base.GetLabelWhenUsedOn(pawn, part) :
   "HET_InstallAnimalPartLabel".Translate(part.def.label);

 public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
 {
  if(pawn?.RaceProps?.Animal!=true || pawn.Dead || !pawn.RaceProps.IsFlesh || pawn.RaceProps.IsMechanoid || pawn.RaceProps.IsAnomalyEntity || pawn.health?.hediffSet==null || pawn.RaceProps.body==null) yield break;
  foreach(var part in pawn.RaceProps.body.AllParts)
   if(part.parent!=null && AnimalPartCatalog.ItemName(part.def)!=null && !pawn.health.hediffSet.PartIsMissing(part.parent) &&
      !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part.parent) &&
      (pawn.health.hediffSet.hediffs.Any(h=>h.Part==part && !(h is Hediff_TailOrigin)) || SmallPartAutopsy.HasDamagedDescendant(pawn,part)) &&
      part.def.defName.IndexOf("Tail",System.StringComparison.OrdinalIgnoreCase)<0)
    yield return part;
 }
 public override void ApplyOnPawn(Pawn pawn,BodyPartRecord part,Pawn billDoer,List<Thing> ingredients,Bill bill)
 {
  if(part==null || ingredients==null || !GetPartsToApplyOn(pawn,recipe).Contains(part) || !ingredients.OfType<Thing_AnimalOrgan>().Any(i=>i.Fits(pawn,part))) {
   Log.Warning("Harvest Everything: animal transplant stopped because the donor species or body part did not match.");return;
  }
  base.ApplyOnPawn(pawn,part,billDoer,ingredients,bill);
 }
}
[HarmonyPatch(typeof(Bill),nameof(Bill.IsFixedOrAllowedIngredient),typeof(Thing))]
public static class AnimalTransplantIngredients
{
 public static void Postfix(Bill __instance,Thing thing,ref bool __result)
 {
  if(!__result || __instance.recipe?.defName!="HET_InstallAnimalPart" || !(__instance is Bill_Medical medical))return;
  if(thing is Thing_AnimalOrgan donor) __result=donor.Fits(medical.GiverPawn,medical.Part);
  else __result=thing?.def?.IsMedicine==true;
 }
}

public static class AnimalPartAvailability
{
 public static bool HasMatchingPart(Pawn pawn, BodyPartRecord part)
 {
  var map=pawn?.MapHeld;
  if(map==null || part==null)return false;
  if(map.listerThings.AllThings.OfType<Thing_AnimalOrgan>().Any(d=>!d.Destroyed && d.stackCount>0 && !d.IsForbidden(pawn)&&d.Fits(pawn,part)))return true;
  return map.mapPawns.AllPawnsSpawned.Any(p=>p.Faction==pawn.Faction && p.carryTracker?.CarriedThing is Thing_AnimalOrgan d && !d.Destroyed && d.stackCount>0 && !d.IsForbidden(pawn) && d.Fits(pawn,part));
 }
}
[HarmonyPatch(typeof(RecipeDef),nameof(RecipeDef.AvailableOnNow))]
public static class AnimalPartMenuAvailability
{
 public static void Postfix(RecipeDef __instance,Thing __0,BodyPartRecord __1,ref bool __result)
 {
  if(__result && __instance.defName=="HET_InstallAnimalPart")
   __result=__0 is Pawn pawn && AnimalPartAvailability.HasMatchingPart(pawn,__1);
 }
}
[HarmonyPatch(typeof(Bill_Medical),nameof(Bill_Medical.ShouldDoNow))]
public static class AnimalPartBillAvailability
{
 public static void Postfix(Bill_Medical __instance,ref bool __result)
 {
  if(__result && __instance.recipe?.targetsBodyPart==true && __instance.Part==null){__result=false;return;}
  if(__result && __instance.recipe?.defName=="HET_InstallAnimalPart")
   __result=AnimalPartAvailability.HasMatchingPart(__instance.GiverPawn,__instance.Part);
 }
}
