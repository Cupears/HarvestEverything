using System;
using RimWorld;
using Verse;

namespace Harvest_Everything;

public static class AnimalPartCatalog
{
 public static bool Has(string name,string term) => name?.IndexOf(term,StringComparison.OrdinalIgnoreCase)>=0;
 public static string ItemName(BodyPartDef part)
 {
  string name=part?.defName;
  if(string.IsNullOrEmpty(name) || Has(name,"Mechanical") || Has(name,"Artificial") || Has(name,"Mech") ||
    Has(name,"Sensor") || Has(name,"Reactor") || Has(name,"Reprocessor") || Has(name,"Battery"))return null;
  if(name=="Torso"||name=="Body"||name=="Chest"||name=="Waist"||name=="Thorax"||name=="Skull"||
    name=="Ribcage"||name=="Sternum"||name=="Pelvis"||name=="Clavicle"||
    (Has(name,"Head")&&!Has(name,"Claw")))return null;
  if(TailOrigins.IsTail(part))return "HET_AnimalTail";
  if(Has(name,"Tailbone"))return null;
  var tags=part.tags;
  if(name=="Heart"||tags?.Contains(BodyPartTagDefOf.BloodPumpingSource)==true)return "HET_AnimalHeart";
  if(name=="Lung"||tags?.Contains(BodyPartTagDefOf.BreathingSource)==true)return "HET_AnimalLung";
  if(name=="Kidney"||tags?.Contains(BodyPartTagDefOf.BloodFiltrationKidney)==true)return "HET_AnimalKidney";
  if(name=="Liver"||tags?.Contains(BodyPartTagDefOf.BloodFiltrationLiver)==true)return "HET_AnimalLiver";
  if(name=="Stomach"||tags?.Contains(BodyPartTagDefOf.MetabolismSource)==true)return "HET_AnimalStomach";
  if(name=="Brain"||tags?.Contains(BodyPartTagDefOf.ConsciousnessSource)==true)return "HET_AnimalBrain";
  if(Has(name,"Tongue"))return "HET_AnimalTongue";
  if(Has(name,"Jaw")||Has(name,"Mouth"))return "HET_AnimalJaw";
  if(Has(name,"Eye")||tags?.Contains(BodyPartTagDefOf.SightSource)==true)return "HET_AnimalEye";
  if(name.EndsWith("Ear",StringComparison.OrdinalIgnoreCase)||tags?.Contains(BodyPartTagDefOf.HearingSource)==true)return "HET_AnimalEar";
  if(Has(name,"Nose")||Has(name,"Nostril"))return "HET_AnimalNose";
  if(Has(name,"Pincer")||Has(name,"Claw")||Has(name,"Sting")||Has(name,"Blade"))return "HET_AnimalClaw";
  if(Has(name,"Finger"))return "HET_AnimalFinger";
  if(Has(name,"Toe"))return "HET_AnimalToe";
  if(Has(name,"Hoof"))return "HET_AnimalHoof";
  if(Has(name,"Horn")||Has(name,"Antler")||Has(name,"Spike")||Has(name,"Crest")||name=="Club")return "HET_AnimalHorn";
  if(Has(name,"Tusk")||Has(name,"Fang"))return "HET_AnimalTusk";
  if(Has(name,"Beak"))return "HET_AnimalBeak";
  if(Has(name,"Wing"))return "HET_AnimalWing";
  if(Has(name,"Shell")||Has(name,"Carapace")||Has(name,"Plastron")||Has(name,"Elytra")||Has(name,"TreeBark"))return "HET_AnimalShell";
  if(Has(name,"Tentacle"))return "HET_AnimalTentacle";
  if(Has(name,"Antenna"))return "HET_AnimalAntenna";
  if(name=="Fin"||name.EndsWith("_Fin",StringComparison.OrdinalIgnoreCase))return "HET_AnimalFin";
  if(name.EndsWith("Spine",StringComparison.OrdinalIgnoreCase))return "HET_AnimalSpine";
  if(Has(name,"Paw"))return "HET_AnimalPaw";
  if(Has(name,"Foot"))return "HET_AnimalFoot";
  if(tags?.Contains(BodyPartTagDefOf.ManipulationLimbDigit)==true)return "HET_AnimalFinger";
  if(tags?.Contains(BodyPartTagDefOf.MovingLimbDigit)==true)return "HET_AnimalToe";
  if(Has(name,"Leg")||Has(name,"Arm")||Has(name,"Shoulder")||Has(name,"Limb")||
    tags?.Contains(BodyPartTagDefOf.MovingLimbCore)==true||tags?.Contains(BodyPartTagDefOf.MovingLimbSegment)==true||
    tags?.Contains(BodyPartTagDefOf.ManipulationLimbCore)==true||tags?.Contains(BodyPartTagDefOf.ManipulationLimbSegment)==true)return "HET_AnimalLimb";
  return null;
 }
 public static ThingDef ItemDef(BodyPartDef part) {
  string name=ItemName(part);return name==null?null:DefDatabase<ThingDef>.GetNamedSilentFail(name);
 }
}
