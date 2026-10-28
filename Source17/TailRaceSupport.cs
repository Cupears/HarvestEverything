using HarmonyLib;
using RimWorld;
using Verse;
namespace Harvest_Everything;

[StaticConstructorOnStartup]
public static class TailRaceSupport {
 static TailRaceSupport(){Apply();}
 public static bool Compatible(ThingDef race,RecipeDef recipe)=>race?.race?.Humanlike==true&&race.thingClass!=null&&typeof(Pawn).IsAssignableFrom(race.thingClass)
  &&!race.race.IsMechanoid&&race.race.body!=null&&recipe.defName.StartsWith("HATT17_")
  &&recipe.appliedOnFixedBodyParts!=null&&race.race.body.AllParts.Any(p=>recipe.appliedOnFixedBodyParts.Contains(p.def));
 public static int Apply(){
  int count=0;
  foreach(var race in DefDatabase<ThingDef>.AllDefsListForReading.Where(r=>r.race?.Humanlike==true)){
   bool changed=false;
   foreach(var recipe in DefDatabase<RecipeDef>.AllDefsListForReading){
    if(!Compatible(race,recipe))continue;
    recipe.recipeUsers??=new List<ThingDef>();
    if(!recipe.recipeUsers.Contains(race)){recipe.recipeUsers.Add(race);changed=true;count++;}
   }
   if(changed)AccessTools.Field(typeof(ThingDef),"allRecipesCached")?.SetValue(race,null);
  }
  return count;
 }
}
