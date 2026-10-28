using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Harvest_Everything;

[HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnNaturalPartIfClean")]
public class MedicalRecipesUtility_SpawnNaturalPartIfClean_Patch
{
	public static bool Prefix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map, ref Thing __result)
	{
		if(pawn?.health?.hediffSet==null||part?.def==null){__result=null;return false;}
  if(pawn.RaceProps.Animal){__result=AnimalNaturalRecovery.Spawn(pawn,part,pos,map);return false;}
		if (!GenList.NullOrEmpty<BodyPartRecord>((IList<BodyPartRecord>)part.parts))
		{
			return HarmonyPatches.IsChildrenClean(pawn, part);
		}
		return true;
	}
}
[HarmonyPatch(typeof(HealthCardUtility), "CreateSurgeryBill")]
public class HealthCardUtility_CreateSurgeryBill_Patch
{
	public static bool Prefix(Pawn medPawn, RecipeDef recipe, BodyPartRecord part, List<Thing> uniqueIngredients = null, bool sendMessages = true)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected Obj, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected Obj, but got Unknown
		if (((Def)recipe).defName == "HarvestEverything")
		{
			List<Hediff> list = new List<Hediff>();
			medPawn.health.hediffSet.GetHediffs<Hediff>(ref list, (Predicate<Hediff>)((Hediff x) => x.def.spawnThingOnRemoved != null && ((object)x).GetType() == typeof(Hediff_Implant)));
			foreach (Hediff item in list)
			{
				RecipeDef val = DefDatabase<RecipeDef>.AllDefs.FirstOrDefault((RecipeDef x) => x.removesHediff == item.def);
				if (val != null && !HasBill(medPawn,val,item.Part))
				{
					Bill_Medical val2 = new Bill_Medical(val, (List<Thing>)null);
					medPawn.BillStack.AddBill((Bill)(object)val2);
					val2.Part = item.Part;
				}
			}
			var enumerable = GetHarvestablePartsToApplyOn(medPawn).OrderByDescending(x=>OrderFor(x.def.tags)).ToList();
			foreach (BodyPartRecord item2 in enumerable)
			{
				if (!item2.IsCorePart && !HasSelectedAncestor(item2,enumerable) && !HasBill(medPawn,RecipeDefOf.RemoveBodyPart,item2))
				{
					Bill_Medical val3 = new Bill_Medical(RecipeDefOf.RemoveBodyPart, (List<Thing>)null);
					medPawn.BillStack.AddBill((Bill)(object)val3);
					val3.Part = item2;
				}
			}
			return false;
		}
		return true;
	}

	public static bool HasBill(Pawn pawn,RecipeDef recipe,BodyPartRecord part) =>
  pawn.BillStack.Bills.OfType<Bill_Medical>().Any(b=>b.recipe==recipe && b.Part==part);
 public static bool HasSelectedAncestor(BodyPartRecord part,IEnumerable<BodyPartRecord> selected) {
  for(var ancestor=part.parent;ancestor!=null;ancestor=ancestor.parent)
   if(selected.Contains(ancestor))return true;
  return false;
 }
 public static int OrderFor(List<BodyPartTagDef> tags)
 {
  if(tags==null)return 4;
		if (tags.Contains(BodyPartTagDefOf.BloodPumpingSource) || tags.Contains(BodyPartTagDefOf.ConsciousnessSource))
		{
			return 0;
		}
		if (tags.Contains(BodyPartTagDefOf.MetabolismSource))
		{
			return 1;
		}
		if (tags.Contains(BodyPartTagDefOf.BreathingSource))
		{
			return 2;
		}
		if (tags.Contains(BodyPartTagDefOf.BloodFiltrationKidney))
		{
			return 3;
		}
		return 4;
	}

	public static IEnumerable<BodyPartRecord> GetHarvestablePartsToApplyOn(Pawn pawn)
	{
		IEnumerable<BodyPartRecord> notMissingParts = pawn.health.hediffSet.GetNotMissingParts((BodyPartHeight)0, (BodyPartDepth)0, (BodyPartTagDef)null, (BodyPartRecord)null);
		foreach (BodyPartRecord part in notMissingParts)
		{
			if (HarmonyPatches.HasRemoveableDirectlyAddedPartFor(pawn, part))
			{
				yield return part;
			}
			else if (((Def)part.def).HasModExtension<ModExtension>() && ((Def)part.def).GetModExtension<ModExtension>().requireCleanChildrenToRemove)
			{
				if (HarmonyPatches.IsChildrenClean(pawn, part))
				{
					yield return part;
				}
			}
			else if (part.def.spawnThingOnRemoved != null || part.def.forceAlwaysRemovable)
			{
				yield return part;
			}
		}
	}
}
public class Recipe_InstallNaturalBodyPart_GetPartsToApplyOn_Patch
{
	public static IEnumerable<BodyPartRecord> Postfix(IEnumerable<BodyPartRecord> __result, Pawn pawn, RecipeDef recipe)
	{
		var original = __result?.ToList() ?? new List<BodyPartRecord>();
		foreach (BodyPartRecord item in original)
		{
			yield return item;
		}
		if(pawn?.health?.hediffSet==null || recipe?.appliedOnFixedBodyParts==null)yield break;
		IEnumerable<BodyPartRecord> notMissingParts = from x in pawn.health.hediffSet.GetNotMissingParts((BodyPartHeight)0, (BodyPartDepth)0, (BodyPartTagDef)null, (BodyPartRecord)null)
			where x.def.spawnThingOnRemoved != null && recipe.appliedOnFixedBodyParts.Contains(x.def) && !original.Contains(x)
			select x;
		foreach (BodyPartRecord part in notMissingParts)
		{
			if (HarmonyPatches.IsChildrenDamaged(pawn, part) && !HarmonyPatches.HasAmputateableFor(pawn, part) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				yield return part;
			}
		}
	}
}
public class Recipe_RemoveImplant_GetPartsToApplyOn_Patch
{
	public static IEnumerable<BodyPartRecord> Postfix(IEnumerable<BodyPartRecord> __result, Pawn pawn, RecipeDef recipe)
	{
		foreach (BodyPartRecord item in __result)
		{
			yield return item;
		}
	}
}
public class Recipe_RemoveBodyPart_GetPartsToApplyOn_Patch
{
	public static IEnumerable<BodyPartRecord> Postfix(IEnumerable<BodyPartRecord> __result, Pawn pawn)
	{
		foreach (BodyPartRecord part in __result)
		{
			if (HarmonyPatches.HasRemoveableDirectlyAddedPartFor(pawn, part))
			{
				yield return part;
			}
			else if (HarmonyPatches.HasAmputateableFor(pawn, part))
			{
				yield return part;
			}
			else if (((Def)part.def).HasModExtension<ModExtension>() && ((Def)part.def).GetModExtension<ModExtension>().requireCleanChildrenToRemove)
			{
				if (HarmonyPatches.IsChildrenClean(pawn, part))
				{
					yield return part;
				}
			}
			else if (part.def.spawnThingOnRemoved != null || part.def.forceAlwaysRemovable)
			{
				yield return part;
			}
		}
	}
}
public class ModExtension : DefModExtension
{
	public bool requireCleanChildrenToRemove;
}
[StaticConstructorOnStartup]
internal class HarmonyPatches
{
	public static bool enabled_QuestionableEthics;

	static HarmonyPatches()
	{
		enabled_QuestionableEthics = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "kongmd.qee");
		Harmony harmony = new Harmony("com.rimwold.ogliss.harvest_everything");
		MethodInfo method = AccessTools.TypeByName("RimWorld.Recipe_RemoveBodyPart").GetMethod("GetPartsToApplyOn");
		MethodInfo method2 = typeof(Recipe_RemoveBodyPart_GetPartsToApplyOn_Patch).GetMethod("Postfix");
		if (method2 == null)
		{
			Log.Error("RemoveBodyPart Postfix is null");
		}
		if (harmony.Patch(method, null, new HarmonyMethod(method2)) == null)
		{
			Log.Error("Harvest Everything RemoveBodyPart patch failed.");
		}
		MethodInfo method3 = AccessTools.TypeByName("RimWorld.Recipe_RemoveImplant").GetMethod("GetPartsToApplyOn");
		MethodInfo method4 = typeof(Recipe_RemoveImplant_GetPartsToApplyOn_Patch).GetMethod("Postfix");
		if (method4 == null)
		{
			Log.Error("RemoveImplant Postfix is null");
		}
		if (harmony.Patch(method3, null, new HarmonyMethod(method4)) == null)
		{
			Log.Error("Harvest Everything RemoveImplant patch failed.");
		}
		if (!enabled_QuestionableEthics)
		{
			MethodInfo method5 = AccessTools.TypeByName("RimWorld.Recipe_InstallNaturalBodyPart").GetMethod("GetPartsToApplyOn");
			MethodInfo method6 = typeof(Recipe_InstallNaturalBodyPart_GetPartsToApplyOn_Patch).GetMethod("Postfix");
			if (method6 == null)
			{
				Log.Error("InstallNaturalBodyPart Postfix is null");
			}
			if (harmony.Patch(method5, null, new HarmonyMethod(method6)) == null)
			{
				Log.Error("Harvest Everything InstallNaturalBodyPart patch failed.");
			}
		}
		else
		{
			Log.Message("QEE deteched");
		}
		harmony.PatchAll(Assembly.GetExecutingAssembly());
		if (Prefs.DevMode)
		{
			Log.Message($"Harvest Everything: successfully completed {harmony.GetPatchedMethods().Select(Harmony.GetPatchInfo).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))} harmony patches.");
		}
	}

	public static bool HasAmputateableFor(Pawn pawn, BodyPartRecord part)
	{
		for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
		{
			if (pawn.health.hediffSet.hediffs[i].Part == part && pawn.health.hediffSet.hediffs[i].def.isBad && part.def.canSuggestAmputation)
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<BodyPartRecord> GetAllChildParts(BodyPartRecord part)
	{
		yield return part;
		foreach (BodyPartRecord child in part.parts)
		{
			foreach (BodyPartRecord allChildPart in GetAllChildParts(child))
			{
				yield return allChildPart;
			}
		}
	}

	public static bool IsChildrenClean(Pawn pawn, BodyPartRecord part)
	{
		IEnumerable<BodyPartRecord> allChildParts = GetAllChildParts(part);
		foreach (BodyPartRecord item in allChildParts)
		{
			if (!MedicalRecipesUtility.IsClean(pawn, item))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsChildrenDamaged(Pawn pawn, BodyPartRecord part)
	{
		IEnumerable<BodyPartRecord> allChildParts = GetAllChildParts(part);
		foreach (BodyPartRecord item in allChildParts)
		{
			if (pawn.health.hediffSet.PartIsMissing(item) || !IsClean(pawn, item))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsClean(Pawn pawn, BodyPartRecord part)
	{
		return !pawn.Dead && !pawn.health.hediffSet.hediffs.Where((Hediff x) => x.Part == part && (x.def.chronic || HediffUtility.IsPermanent(x) || x.def == HediffDefOf.WoundInfection) && x.def.isBad).Any();
	}

	public static bool HasRemoveableDirectlyAddedPartFor(Pawn pawn, BodyPartRecord part)
	{
		for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
		{
			if (pawn.health.hediffSet.hediffs[i].Part == part && pawn.health.hediffSet.hediffs[i] is Hediff_AddedPart)
			{
				return pawn.health.hediffSet.hediffs[i].Part.def.spawnThingOnRemoved != null;
			}
		}
		return false;
	}

	public static bool PartOrAnyAncestorHasRemoveableDirectlyAddedParts(Pawn pawn, BodyPartRecord part)
	{
		return HasRemoveableDirectlyAddedPartFor(pawn, part) || (part.parent != null && PartOrAnyAncestorHasRemoveableDirectlyAddedParts(pawn, part.parent));
	}

	public static bool AncestorHasRemoveableDirectlyAddedParts(Pawn pawn, BodyPartRecord part)
	{
		return part.parent != null && PartOrAnyAncestorHasRemoveableDirectlyAddedParts(pawn, part.parent);
	}
}
