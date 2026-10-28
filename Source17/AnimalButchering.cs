using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Harvest_Everything;

/// <summary>
/// Lets the separate "Butcher for animal parts" bill recover healthy organs
/// from animal corpses. The ordinary "butcher creature" bill is untouched and
/// keeps its full meat and leather yield. The organ bill trades half of the
/// meat for the organs it recovers.
/// </summary>
[HarmonyPatch(typeof(Pawn), nameof(Pawn.ButcherProducts))]
public static class Pawn_ButcherProducts_Patch
{
	public static void Postfix(Pawn __instance, Pawn butcher, float efficiency, ref IEnumerable<Thing> __result)
	{
		if (__result == null || !AnimalButcherParts.IsAnimalPartsBill(butcher))
		{
			return;
		}
		// Vanilla's iterator marks lifeStage.butcherBodyPart parts as missing while
		// it yields them, so drain it first and only then look for leftover organs.
		List<Thing> vanilla = __result.ToList();
		AnimalButcherParts.ReduceMeat(__instance, vanilla);
		if (AnimalButcherParts.ShouldApply(__instance))
		{
			vanilla.AddRange(AnimalButcherParts.Extract(__instance, efficiency));
		}
		__result = vanilla;
	}
}

public static class AnimalButcherParts
{
	// Matches the recipe def in Defs/Recipes_AnimalParts.xml.
	public const string AnimalPartsRecipeDefName = "HET_ButcherAnimalParts";

	// The organ bill wastes meat because the carcass is picked apart for parts.
	private const float MeatYieldFactor = 0.5f;

	// Chance per organ at 100% butchery efficiency.
	private const float BaseDropChance = 0.25f;

	public static bool ShouldApply(Pawn pawn)
	{
		if (pawn?.RaceProps == null || pawn.health?.hediffSet == null)
		{
			return false;
		}
		RaceProperties props = pawn.RaceProps;
		if (!pawn.Dead || !props.Animal || !props.IsFlesh || props.IsMechanoid || props.IsAnomalyEntity)
		{
			return false;
		}
		if (props.body == null || pawn.IsMutant)
		{
			return false;
		}
		// Only while butchering a corpse, and only fresh ones.
		return pawn.ParentHolder is Corpse corpse && corpse.GetRotStage() == RotStage.Fresh;
	}

	// True while the pawn is running the mod's own organ bill. GenRecipe calls
	// ButcherProducts from inside that job, so the recipe is the current bill's.
	public static bool IsAnimalPartsBill(Pawn butcher)
	{
		RecipeDef recipe = butcher?.jobs?.curJob?.RecipeDef;
		return recipe != null && recipe.defName == AnimalPartsRecipeDefName;
	}

	// The organ bill recovers parts instead of meat, so it yields less meat.
	public static void ReduceMeat(Pawn pawn, List<Thing> products)
	{
		ThingDef meatDef = pawn?.RaceProps?.meatDef;
		if (meatDef == null)
		{
			return;
		}
		for (int i = 0; i < products.Count; i++)
		{
			if (products[i].def != meatDef)
			{
				continue;
			}
			int reduced = Mathf.RoundToInt(products[i].stackCount * MeatYieldFactor);
			products[i].stackCount = Mathf.Max(1, reduced);
		}
	}

	public static List<Thing> Extract(Pawn pawn, float efficiency)
	{
		List<Thing> results = new List<Thing>();
		if (!ShouldApply(pawn) || float.IsNaN(efficiency) || float.IsInfinity(efficiency) || efficiency<=0)
		{
			return results;
		}
		foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts().OrderBy(AncestorDepth).ToList())
		{
			ThingDef thingDef = AnimalPartCatalog.ItemDef(part.def);
			if (part.parent==null || pawn.health.hediffSet.PartIsMissing(part) || thingDef==null)
			{
				continue;
			}
			if (!IsPartClean(pawn, part))
			{
				continue;
			}
			if (!Rand.Chance(RecoveryChance(part, efficiency)))
			{
				continue;
			}
			var tailOrigin=TailOrigins.IsTail(part.def)?TailOrigins.FromPawn(pawn,part):null;
			pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, part));
			Thing thing = ThingMaker.MakeThing(thingDef);
			if (thing is Thing_AnimalOrgan organ)
			{
				organ.SetAnimalPart(pawn, part);
			}
			TailOrigins.Apply(thing,tailOrigin);
			results.Add(thing);
		}
		return results;
	}

 public static int AncestorDepth(BodyPartRecord part) {
  int depth=0;for(var parent=part?.parent;parent!=null;parent=parent.parent)depth++;
  return depth;
 }

	// MedicalRecipesUtility.IsClean returns false for dead pawns, and every animal
	// here is a corpse, so mirror its check without the liveness requirement.
	public static bool IsPartClean(Pawn pawn, BodyPartRecord part)
	{
		if(pawn?.health?.hediffSet==null||part?.def==null||pawn.health.hediffSet.PartIsMissing(part)||pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))return false;
		List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
		for (int i = 0; i < hediffs.Count; i++)
		{
			if (hediffs[i].Part == part && !(hediffs[i] is Hediff_TailOrigin))
			{
				return false;
			}
		}
		foreach(var child in part.parts)
   if(pawn.health.hediffSet.PartIsMissing(child)||!IsPartClean(pawn,child))return false;
  return true;
 }
 public static float RecoveryChance(BodyPartRecord part,float efficiency) {
  if(part?.def?.defName==null||float.IsNaN(efficiency)||float.IsInfinity(efficiency)||efficiency<=0)return 0f;
  string kind=AnimalPartCatalog.ItemName(part.def);
  if(kind==null)return 0f;
  float factor=kind=="HET_AnimalFinger"||kind=="HET_AnimalToe"||kind=="HET_AnimalClaw"?0.2f:
   kind=="HET_AnimalHoof"||kind=="HET_AnimalHorn"||kind=="HET_AnimalTusk"||kind=="HET_AnimalBeak"||kind=="HET_AnimalAntenna"?0.5f:1f;
  return Mathf.Clamp01(efficiency*BaseDropChance)*factor;
 }
}

