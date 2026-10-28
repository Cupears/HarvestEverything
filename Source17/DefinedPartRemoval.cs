using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Harvest_Everything;

public class Recipe_RemoveDefinedParts : Recipe_RemoveBodyPart
{
 public override string GetLabelWhenUsedOn(Pawn pawn,BodyPartRecord part) => recipe.label;
 public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn,RecipeDef recipe)
 {
  if(pawn?.health?.hediffSet==null||pawn.Dead||recipe?.appliedOnFixedBodyParts==null)yield break;
  if(pawn.RaceProps.Animal) {
   if(!pawn.RaceProps.IsFlesh||pawn.RaceProps.IsMechanoid||pawn.RaceProps.IsAnomalyEntity)yield break;
   foreach(var part in pawn.health.hediffSet.GetNotMissingParts())
    if(part.parent!=null && recipe.appliedOnFixedBodyParts.Contains(part.def) &&
      AnimalPartCatalog.ItemDef(part.def)!=null && AnimalButcherParts.IsPartClean(pawn,part))yield return part;
   yield break;
  }
  foreach(var part in base.GetPartsToApplyOn(pawn,recipe))
   if(recipe.appliedOnFixedBodyParts.Contains(part.def))yield return part;
 }
}
