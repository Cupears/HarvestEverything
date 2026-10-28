using System;
using Verse;

namespace Harvest_Everything;

/// <summary>
/// An organ item taken from an animal. The source species is stored on the item
/// so it reads as "muffalo heart" instead of a human donor organ, and organs
/// from different species do not stack together.
/// </summary>
public class Thing_AnimalOrgan : ThingWithComps
{
	private string sourceRaceDefName;
	private string sourceRaceLabel;
	private string sourcePartLabel;
	private string sourcePartDefName;
	public bool Fits(Pawn pawn, BodyPartRecord part) => pawn?.def?.defName == sourceRaceDefName && part?.def != null && (sourcePartDefName.NullOrEmpty() ? part.def.label == sourcePartLabel : part.def.defName == sourcePartDefName);

	public void SetSourceRace(ThingDef race)
	{
		if (race == null)
		{
			return;
		}
		sourceRaceDefName = race.defName;
		sourceRaceLabel = race.label;
	}

	public void SetAnimalPart(Pawn source, BodyPartRecord part)
	{
		if (source?.def != null)
		{
			sourceRaceDefName = source.def.defName;
			sourceRaceLabel = source.def.label;
		}
		sourcePartLabel = part?.def?.label;
		sourcePartDefName = part?.def?.defName;
	}

	// Thing.Label already calls the virtual LabelNoCount, then adds stack count.
	// Adding the species there too would prefix it twice.
	public override string LabelNoCount => WithSpecies(base.LabelNoCount);

	private string WithSpecies(string baseLabel)
	{
		if (sourceRaceLabel.NullOrEmpty())
		{
			return baseLabel;
		}
		// Our def labels start with "animal " so an unnamed item still reads as
		// an animal organ; the species name replaces that generic prefix.
		string noun = sourcePartLabel.NullOrEmpty() ? baseLabel : sourcePartLabel;
		if (noun.StartsWith("animal ", StringComparison.OrdinalIgnoreCase))
			noun = noun.Substring("animal ".Length);
		if (noun.EndsWith(" (natural)", StringComparison.OrdinalIgnoreCase))
			noun = noun.Substring(0, noun.Length - " (natural)".Length);
		return sourceRaceLabel + " " + noun;
	}

	public override bool CanStackWith(Thing other)
	{
		if (!base.CanStackWith(other))
		{
			return false;
		}
		return !(other is Thing_AnimalOrgan organ) ||
			(organ.sourceRaceDefName == sourceRaceDefName && organ.sourcePartLabel == sourcePartLabel && organ.sourcePartDefName == sourcePartDefName);
	}

	public override Thing SplitOff(int count)
	{
		Thing split = base.SplitOff(count);
		if (split is Thing_AnimalOrgan organ)
		{
			organ.sourceRaceDefName = sourceRaceDefName;
			organ.sourceRaceLabel = sourceRaceLabel;
			organ.sourcePartLabel = sourcePartLabel;
			organ.sourcePartDefName = sourcePartDefName;
		}
		return split;
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref sourceRaceDefName, "HET_sourceRaceDefName");
		Scribe_Values.Look(ref sourceRaceLabel, "HET_sourceRaceLabel");
		Scribe_Values.Look(ref sourcePartLabel, "HET_sourcePartLabel");
		Scribe_Values.Look(ref sourcePartDefName, "HET_sourcePartDefName");
	}
}
