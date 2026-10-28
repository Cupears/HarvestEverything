using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Harvest_Everything;

// Optional Post Mortem integration. No Workshop assembly is changed.
[StaticConstructorOnStartup]
public static class SmallPartAutopsy
{
    public const float DigitRecoveryFactor = 0.20f;

    static SmallPartAutopsy()
    {
        var utility = AccessTools.TypeByName("Autopsy.NewMedicalRecipesUtility");
        if (utility == null) return;
        var clean = AccessTools.Method(utility, "IsCleanAndDroppable");
        var extract = AccessTools.Method(utility, "TryGetParts");
        if (clean == null || extract == null)
        {
            Log.Warning("Harvest Everything: Post Mortem small-part integration unavailable: expected methods were not found.");
            return;
        }
        var harmony = new Harmony("cupears.harvesteverything.smallparts");
        harmony.Patch(clean, postfix: new HarmonyMethod(typeof(SmallPartAutopsy), nameof(CheckChildren)));
        harmony.Patch(extract, prefix: new HarmonyMethod(typeof(SmallPartAutopsy), nameof(ScaleDigitChance)));
    }

    public static bool IsDigit(BodyPartRecord part) =>
        part?.def?.defName == "Finger" || part?.def?.defName == "Toe";

    public static float EffectiveDigitChance(float skillChance, float naturalChance) =>
        Math.Max(0f, Math.Min(1f, Math.Min(skillChance, naturalChance))) * DigitRecoveryFactor;

    // A damaged/missing finger must prevent the hand being recovered as a fully
    // intact hand. This also lets Post Mortem visit surviving smaller parts.
    public static bool HasDamagedDescendant(Pawn pawn, BodyPartRecord part)
    {
        foreach (var child in part.parts)
        {
            if (pawn.health.hediffSet.PartIsMissing(child) ||
                pawn.health.hediffSet.hediffs.Any(h => h.Part == child && !(h is Hediff_TailOrigin)) ||
                HasDamagedDescendant(pawn, child)) return true;
        }
        return false;
    }

    public static void CheckChildren(Pawn pawn, BodyPartRecord part, ref bool __result)
    {
        if (__result && pawn?.RaceProps?.Humanlike == true && HasDamagedDescendant(pawn, part))
            __result = false;
    }

    // Create a per-call RecipeInfo; never change shared autopsy settings.
    // Scale both inputs so the final min(skill, cap) is exactly 20% of normal.
    // Age checks, total-part limits and corpse damage remain Post Mortem's own.
    public static void ScaleDigitChance(object[] __args)
    {
        if (__args.Length < 4 || !(__args[0] is Corpse corpse) ||
            corpse.InnerPawn?.RaceProps?.Humanlike != true ||
            !(__args[2] is BodyPartRecord part) || !IsDigit(part) ||
            corpse.InnerPawn.health.hediffSet.hediffs.Any(h => h.Part == part)) return;
        var info = __args[1];
        var type = info?.GetType();
        var ctor = type?.GetConstructor(new[] { typeof(float), typeof(int), typeof(float), typeof(int), typeof(float) });
        if (ctor == null) return;
        object Read(string name) => AccessTools.Field(type, name).GetValue(info);
        __args[1] = ctor.Invoke(new object[] {
            (float)Read("NaturalChance") * DigitRecoveryFactor,
            (int)Read("CorpseValidAge"), (float)Read("BionicChance"),
            (int)Read("PartNumber"), (float)Read("FrozenDecay") });
        __args[3] = (float)__args[3] * DigitRecoveryFactor;
    }
}
