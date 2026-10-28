using HarmonyLib;
using RimWorld;
using Verse;
namespace Harvest_Everything;

[StaticConstructorOnStartup]
public static class NaturalRaceSupport {
 static NaturalRaceSupport(){Apply();}
 public static bool Compatible(ThingDef race,RecipeDef recipe){
  if(race?.race==null||race.thingClass==null||!typeof(Pawn).IsAssignableFrom(race.thingClass)||race.race.IsMechanoid||race.race.body==null)return false;
  if(recipe.defName=="HET_InstallAnimalPart"||recipe.defName=="HET_RemoveAnimalTail") return race.race.Animal;
  if(recipe.defName=="HET_InstallAnimalTail")
   return (race.race.Humanlike||race.race.Animal)&&race.race.body.AllParts.Any(p=>recipe.appliedOnFixedBodyParts.Contains(p.def));
  if(!race.race.Humanlike)return false;
  if(recipe.defName=="HarvestEverything")return true;
  if(recipe.workerClass!=typeof(Recipe_InstallNaturalBodyPart)||recipe.appliedOnFixedBodyParts==null)return false;
  return race.race.body.AllParts.Any(p=>recipe.appliedOnFixedBodyParts.Contains(p.def));
 }
 public static int Apply(){
  int count=0;
  var sharedTailRecipe=DefDatabase<RecipeDef>.GetNamedSilentFail("HET_InstallAnimalTail");
  var animalTail=DefDatabase<ThingDef>.GetNamedSilentFail("HET_AnimalTail");
  if(sharedTailRecipe!=null && animalTail!=null) {
   var tailIngredients=sharedTailRecipe.ingredients.Where(i=>i.filter.Allows(animalTail)).ToList();
   foreach(var item in DefDatabase<BodyPartDef>.AllDefsListForReading.Where(TailOrigins.IsTail).Select(p=>p.spawnThingOnRemoved).Where(d=>d!=null).Distinct()) {
    sharedTailRecipe.fixedIngredientFilter.SetAllow(item,true);
    foreach(var ingredient in tailIngredients)ingredient.filter.SetAllow(item,true);
   }
  }

  foreach(var recipe in DefDatabase<RecipeDef>.AllDefsListForReading)
   if(recipe.workerClass!=null && typeof(Recipe_InstallNaturalBodyPart).IsAssignableFrom(recipe.workerClass) &&
      (recipe.defName.StartsWith("HET_") || recipe.defName.StartsWith("HATT17_") || recipe.modContentPack?.PackageId=="cupears.harvesteverythingtails"))
    recipe.dontShowIfAnyIngredientMissing=true;
  foreach(var race in DefDatabase<ThingDef>.AllDefsListForReading.Where(r=>r.race?.Humanlike==true||r.race?.Animal==true)){
   bool changed=false;
   foreach(var recipe in DefDatabase<RecipeDef>.AllDefsListForReading){
    if(recipe.defName=="HET_InstallAnimalTail"||recipe.defName=="HET_RemoveAnimalTail"){
     recipe.appliedOnFixedBodyParts??=new List<BodyPartDef>();
     foreach(var tail in DefDatabase<BodyPartDef>.AllDefsListForReading.Where(p=>TailOrigins.IsTail(p)))
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

