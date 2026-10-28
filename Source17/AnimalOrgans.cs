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

	public void SetSourceRace(ThingDef race)
	{
		if (race == null)
		{
			return;
		}
		sourceRaceDefName = race.defName;
		sourceRaceLabel = race.label;
	}

	public override string Label => WithSpecies(base.Label);

	public override string LabelNoCount => WithSpecies(base.LabelNoCount);

	private string WithSpecies(string baseLabel)
	{
		if (sourceRaceLabel.NullOrEmpty())
		{
			return baseLabel;
		}
		// Our def labels start with "animal " so an unnamed item still reads as
		// an animal organ; the species name replaces that generic prefix.
		const string animalPrefix = "animal ";
		string noun = baseLabel.StartsWith(animalPrefix, StringComparison.Ordinal)
			? baseLabel.Substring(animalPrefix.Length)
			: baseLabel;
		return sourceRaceLabel + " " + noun;
	}

	public override bool CanStackWith(Thing other)
	{
		if (!base.CanStackWith(other))
		{
			return false;
		}
		return !(other is Thing_AnimalOrgan organ) || organ.sourceRaceDefName == sourceRaceDefName;
	}

	public override Thing SplitOff(int count)
	{
		Thing split = base.SplitOff(count);
		if (split is Thing_AnimalOrgan organ)
		{
			organ.sourceRaceDefName = sourceRaceDefName;
			organ.sourceRaceLabel = sourceRaceLabel;
		}
		return split;
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref sourceRaceDefName, "HET_sourceRaceDefName");
		Scribe_Values.Look(ref sourceRaceLabel, "HET_sourceRaceLabel");
	}
}
