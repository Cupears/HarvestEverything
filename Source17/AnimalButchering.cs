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

	// Vanilla animals reuse the human organ body parts, whose spawnThingOnRemoved
	// points at the human donor items. Swap those for animal-specific items.
	private static readonly Dictionary<string, string> AnimalOrganDefNames = new Dictionary<string, string>
	{
		{ "Heart", "HET_AnimalHeart" },
		{ "Lung", "HET_AnimalLung" },
		{ "Kidney", "HET_AnimalKidney" },
		{ "Liver", "HET_AnimalLiver" },
		{ "Stomach", "HET_AnimalStomach" },
		{ "Brain", "HET_AnimalBrain" },
	};

	private static readonly BodyPartTagDef[] OrganTags =
	{
		BodyPartTagDefOf.BloodPumpingSource,
		BodyPartTagDefOf.BreathingSource,
		BodyPartTagDefOf.BloodFiltrationKidney,
		BodyPartTagDefOf.BloodFiltrationLiver,
		BodyPartTagDefOf.MetabolismSource,
		BodyPartTagDefOf.ConsciousnessSource,
	};

	public static bool ShouldApply(Pawn pawn)
	{
		if (pawn?.RaceProps == null || pawn.health?.hediffSet == null)
		{
			return false;
		}
		RaceProperties props = pawn.RaceProps;
		if (!props.Animal || props.IsMechanoid || props.IsAnomalyEntity)
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
		float chance = Mathf.Clamp01(efficiency * BaseDropChance);
		if (chance <= 0f)
		{
			return results;
		}
		foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts().ToList())
		{
			ThingDef thingDef = part.def?.spawnThingOnRemoved;
			if (thingDef == null || (!IsOrgan(part) && !IsBaseAnimalCollectible(part)))
			{
				continue;
			}
			if (!IsPartClean(pawn, part))
			{
				continue;
			}
			if (!Rand.Chance(chance))
			{
				continue;
			}
			pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, part));
			Thing thing = ThingMaker.MakeThing(AnimalOrganFor(part, thingDef));
			if (thing is Thing_AnimalOrgan organ)
			{
				organ.SetAnimalPart(pawn, part);
			}
			results.Add(thing);
		}
		return results;
	}

	private static ThingDef AnimalOrganFor(BodyPartRecord part, ThingDef fallback)
	{
		if (part?.def != null)
		{
			string collectible = part.def.defName switch
			{
				"Tail" => "HET_AnimalTail",
				"Paw" or "Foot" => "HET_AnimalPaw",
				"Finger" or "Toe" or "FrontClaw" or "RearClaw" => "HET_AnimalClaw",
				"Hoof" => "HET_AnimalHoof",
				"Horn" or "Antler" => "HET_AnimalHorn",
				"Tusk" or "Beak" => "HET_AnimalBeak",
				_ => null
			};
			if (collectible != null && DefDatabase<ThingDef>.GetNamedSilentFail(collectible) is ThingDef collectibleDef)
				return collectibleDef;
		}
		if (part.def != null && AnimalOrganDefNames.TryGetValue(part.def.defName, out string animalDefName))
		{
			ThingDef animalDef = DefDatabase<ThingDef>.GetNamedSilentFail(animalDefName);
			if (animalDef != null)
			{
				return animalDef;
			}
		}
		return fallback;
	}

	// Phase one covers shared RimWorld body-part types. Race-specific anatomical
	// types are added in the mod-race pass after these common types.
	private static bool IsBaseAnimalCollectible(BodyPartRecord part) => part?.def?.defName is
		"Tail" or "Paw" or "Foot" or "Finger" or "Toe" or "FrontClaw" or "RearClaw" or
		"Hoof" or "Horn" or "Antler" or "Tusk" or "Beak";

	private static bool IsOrgan(BodyPartRecord part)
	{
		List<BodyPartTagDef> tags = part.def?.tags;
		if (tags == null || tags.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < OrganTags.Length; i++)
		{
			if (tags.Contains(OrganTags[i]))
			{
				return true;
			}
		}
		return false;
	}

	// MedicalRecipesUtility.IsClean returns false for dead pawns, and every animal
	// here is a corpse, so mirror its check without the liveness requirement.
	private static bool IsPartClean(Pawn pawn, BodyPartRecord part)
	{
		List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
		for (int i = 0; i < hediffs.Count; i++)
		{
			if (hediffs[i].Part == part)
			{
				return false;
			}
		}
		return true;
	}
}
