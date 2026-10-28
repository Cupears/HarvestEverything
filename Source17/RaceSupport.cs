using HarmonyLib;
using RimWorld;
using Verse;
namespace Harvest_Everything;

[StaticConstructorOnStartup]
public static class NaturalRaceSupport {
 static NaturalRaceSupport(){Apply();}
 public static bool Compatible(ThingDef race,RecipeDef recipe){
  if(race?.race==null||race.thingClass==null||!typeof(Pawn).IsAssignableFrom(race.thingClass)||race.race.IsMechanoid||race.race.body==null)return false;
  if(recipe.defName=="HET_InstallAnimalPart") return race.race.Animal;
  if(recipe.defName=="HET_InstallAnimalTail")
   return (race.race.Humanlike||race.race.Animal)&&race.race.body.AllParts.Any(p=>p.def.defName=="Tail");
  if(!race.race.Humanlike)return false;
  if(recipe.defName=="HarvestEverything")return true;
  if(recipe.workerClass!=typeof(Recipe_InstallNaturalBodyPart)||recipe.appliedOnFixedBodyParts==null)return false;
  return race.race.body.AllParts.Any(p=>recipe.appliedOnFixedBodyParts.Contains(p.def));
 }
 public static int Apply(){
  int count=0;
  foreach(var race in DefDatabase<ThingDef>.AllDefsListForReading.Where(r=>r.race?.Humanlike==true||r.race?.Animal==true)){
   bool changed=false;
   foreach(var recipe in DefDatabase<RecipeDef>.AllDefsListForReading){
    if(recipe.defName=="HET_InstallAnimalTail"){
     recipe.appliedOnFixedBodyParts??=new List<BodyPartDef>();
     foreach(var tail in DefDatabase<BodyPartDef>.AllDefsListForReading.Where(p=>p.defName.IndexOf("Tail",System.StringComparison.OrdinalIgnoreCase)>=0))
      if(!recipe.appliedOnFixedBodyParts.Contains(tail))recipe.appliedOnFixedBodyParts.Add(tail);
    }
    if(!Compatible(race,recipe))continue;
    recipe.recipeUsers??=new List<ThingDef>();
    if(!recipe.recipeUsers.Contains(race)){recipe.recipeUsers.Add(race);changed=true;count++;}
   }
   if(changed)AccessTools.Field(typeof(ThingDef),"allRecipesCached")?.SetValue(race,null);
  }
  return count;
 }
}

