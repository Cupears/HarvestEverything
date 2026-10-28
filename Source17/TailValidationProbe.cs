#if HARVEST_VALIDATION
using HarmonyLib;
using RimWorld;
using Verse;
using System.Text.Json;
namespace Harvest_Everything;
[StaticConstructorOnStartup]
public static class ValidationProbe {
 static bool done;
 static ValidationProbe(){new Harmony("cupears.harvest.bootprobe").Patch(AccessTools.Method(typeof(Root_Entry),"Update"),postfix:new HarmonyMethod(typeof(ValidationProbe),nameof(Update)));}
 static void Update(){
  if(done||LongEventHandler.AnyEventNowOrWaiting)return;done=true;
  var rows=new List<object>();var failures=new List<string>();
  var tailRecipes=DefDatabase<RecipeDef>.AllDefsListForReading.Where(r=>r.defName.StartsWith("HATT17_")).ToList();
  foreach(var race in DefDatabase<ThingDef>.AllDefsListForReading.Where(r=>r.race?.Humanlike==true&&r.thingClass!=null&&typeof(Pawn).IsAssignableFrom(r.thingClass))){
   var tails=race.race.body.AllParts.Where(p=>p.def.defName.Contains("Tail",StringComparison.OrdinalIgnoreCase)).ToList();
   foreach(var tail in tails){
    bool available=race.AllRecipes.Any(r=>r.workerClass==typeof(Recipe_InstallNaturalBodyPart)&&r.appliedOnFixedBodyParts?.Contains(tail.def)==true);
    if(tail.def.spawnThingOnRemoved==null||!available)failures.Add(race.defName+":"+tail.def.defName);
   }
   rows.Add(new{race=race.defName,tails=tails.Count,naturalRecipes=race.AllRecipes.Count(r=>r.workerClass==typeof(Recipe_InstallNaturalBodyPart))});
  }
  var report=new{tailRecipeCount=tailRecipes.Count,races=rows,failures};
  File.WriteAllText(Path.Combine(GenFilePaths.SaveDataFolderPath,"Cupears-harvest-validation.json"),JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
  Log.Message("[Cupears harvest validation] "+tailRecipes.Count+" tail recipes, "+failures.Count+" missing tail mappings.");
  Root.Shutdown();
 }
}
#endif
