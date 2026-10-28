# Building from source

Requires the .NET 10 SDK, RimWorld 1.7 and Harmony.

```powershell
dotnet build Source17/Mod.csproj -c Release -p:RimWorldDir="M:/Steam/steamapps/common/RimWorld" -p:HarmonyPath="M:/Steam/steamapps/workshop/content/294100/2009463077/1.7/Assemblies/0Harmony.dll"
```

The 1.6 build compiles the same sources against the 1.6 assemblies:

```powershell
dotnet build Source16/Mod.csproj -c Release -p:Ref16Dir="C:/ref16"
```

`Ref16Dir` must contain `Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll` and `0Harmony.dll`
from a RimWorld 1.6 install or the Harmony 1.6 folder.

Game and Harmony assemblies are referenced locally and are not shipped.
