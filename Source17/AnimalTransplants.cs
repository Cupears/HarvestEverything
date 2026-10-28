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
  if(pawn?.RaceProps?.Animal!=true || pawn.RaceProps.IsMechanoid) yield break;
  foreach(var part in pawn.RaceProps.body.AllParts)
   if(part.parent!=null && !pawn.health.hediffSet.PartIsMissing(part.parent) &&
      !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part) &&
      pawn.health.hediffSet.hediffs.Any(h=>h.Part==part) &&
      part.def.defName.IndexOf("Tail",System.StringComparison.OrdinalIgnoreCase)<0)
    yield return part;
 }
 public override void ApplyOnPawn(Pawn pawn,BodyPartRecord part,Pawn billDoer,List<Thing> ingredients,Bill bill)
 {
  if(!ingredients.OfType<Thing_AnimalOrgan>().Any(i=>i.Fits(pawn,part))) {
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
 }
}
