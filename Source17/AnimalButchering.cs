using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Harvest_Everything;

/// <summary>
/// Lets butchered animals give up their healthy organs, not just humanlikes.
/// Vanilla animal butchery only yields meat, leather and the life stage's
/// butcherBodyPart (horn, tusk, ...). This postfix appends clean organ parts
/// that already have a spawnThingOnRemoved thing def.
/// </summary>
[HarmonyPatch(typeof(Pawn), nameof(Pawn.ButcherProducts))]
public static class Pawn_ButcherProducts_Patch
{
	public static void Postfix(Pawn __instance, float efficiency, ref IEnumerable<Thing> __result)
	{
		if (__result == null || !AnimalButcherParts.ShouldApply(__instance))
		{
			return;
		}
		// Vanilla's iterator marks lifeStage.butcherBodyPart parts as missing while
		// it yields them, so drain it first and only then look for leftover organs.
		List<Thing> vanilla = __result.ToList();
		List<Thing> extra = AnimalButcherParts.Extract(__instance, efficiency);
		if (extra.Count == 0)
		{
			__result = vanilla;
			return;
		}
		vanilla.AddRange(extra);
		__result = vanilla;
	}
}

public static class AnimalButcherParts
{
	// Chance per organ at 100% butchery efficiency.
	private const float BaseDropChance = 0.25f;

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
			if (thingDef == null || !IsOrgan(part))
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
			results.Add(ThingMaker.MakeThing(thingDef));
		}
		return results;
	}

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
