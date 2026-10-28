using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;

namespace Harvest_Everything;

[HarmonyPatch(typeof(MedicalRecipesUtility),nameof(MedicalRecipesUtility.IsClean))]
public static class TailMetadataClean
{
 public static void Postfix(Pawn pawn,BodyPartRecord part,ref bool __result) {
  if(__result||pawn?.health?.hediffSet==null||pawn.Dead||part==null)return;
  var onPart=pawn.health.hediffSet.hediffs.Where(h=>h.Part==part).ToList();
  if(onPart.Any(h=>h is Hediff_TailOrigin) && !onPart.Any(h=>!(h is Hediff_TailOrigin)) &&
    !pawn.health.hediffSet.PartIsMissing(part))__result=true;
 }
}

public static class AnimalNaturalRecovery
{
 // Called from our existing SpawnNaturalPartIfClean prefix so there is one
 // owner for spawning and no human donor item can leak from animal surgery.
 public static Thing Spawn(Pawn pawn,BodyPartRecord part,IntVec3 pos,Map map) {
  if(pawn==null||pawn.Dead||map==null||part?.parent==null||!pawn.RaceProps.Animal||
    !pawn.RaceProps.IsFlesh||pawn.RaceProps.IsMechanoid||pawn.RaceProps.IsAnomalyEntity||
    !AnimalButcherParts.IsPartClean(pawn,part))return null;
  var def=AnimalPartCatalog.ItemDef(part.def);
  if(def==null)return null;
  var item=ThingMaker.MakeThing(def);
  if(!(item is Thing_AnimalOrgan donor))return null;
  donor.SetAnimalPart(pawn,part);
  if(TailOrigins.IsTail(part.def))TailOrigins.Apply(item,TailOrigins.FromPawn(pawn,part));
  return GenSpawn.Spawn(item,pos,map);
 }
}
