using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Harvest_Everything;

[StaticConstructorOnStartup]
public static class BionicIconsBridge
{
 static BionicIconsBridge()
 {
  var renderer=AccessTools.TypeByName("BionicIcons.BionicIcons");
  var iconType=AccessTools.TypeByName("Verse.BionicIconsIconDef");
  var boxType=AccessTools.TypeByName("Verse.BionicIconsTextureDef");
  if(renderer==null||iconType==null||boxType==null)return;
  RuntimeHelpers.RunClassConstructor(renderer.TypeHandle);
  var replace=AccessTools.Method(renderer,"replace");
  if(replace==null) {Log.Warning("Harvest Everything: Bionic Icons renderer was not found.");return;}
  var database=typeof(DefDatabase<>).MakeGenericType(iconType);
  var definitions=(IEnumerable)AccessTools.Property(database,"AllDefsListForReading").GetValue(null,null);
  foreach(Def icon in definitions)
  {
   if(!icon.defName.StartsWith("HET_Icon_",StringComparison.Ordinal))continue;
   string name=(string)AccessTools.Field(iconType,"thingDef").GetValue(icon);
   string texture=(string)AccessTools.Field(iconType,"texture").GetValue(icon);
   var item=DefDatabase<ThingDef>.GetNamedSilentFail(name);
   if(item==null)continue;
   // BodyParts textures are two-channel masks, not directly drawable sprites.
   // Use the mod's normal box + mask renderer instead of its raw-sprite path.
   var box=Activator.CreateInstance(boxType);
   AccessTools.Field(boxType,"replacement").SetValue(box,
    name.StartsWith("HET_Animal",StringComparison.Ordinal)?"BionicIcons/Boxes/Animal":"BionicIcons/Boxes/Default");
   replace.Invoke(null,new object[]{item,texture,Color.white,box});
  }
 }
}
