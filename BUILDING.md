# Building from source

## Requirements

- .NET 10 SDK for RimWorld 1.7.
- A RimWorld 1.7 installation and its Harmony assembly.
- RimWorld 1.6 game and Harmony assemblies to build the 1.6 version.

## Build for RimWorld 1.7

Replace the sample paths with the locations on your computer:

```powershell
dotnet build Source17/Mod.csproj -c Release `
  -p:RimWorldDir="C:/path/to/RimWorld" `
  -p:HarmonyPath="C:/path/to/0Harmony.dll"
```

## Build for RimWorld 1.6

`Ref16Dir` must contain `Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll` and `0Harmony.dll` from a RimWorld 1.6 installation or its Harmony folder.

```powershell
dotnet build Source16/Mod.csproj -c Release -p:Ref16Dir="C:/path/to/RimWorld1.6/Assemblies"
```

Game and Harmony assemblies are referenced from local installations and are not included in this repository.
