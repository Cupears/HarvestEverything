# Harvest Everything - 1.7 Race Support

![RimWorld](https://img.shields.io/badge/RimWorld-1.7-orange) ![Stage](https://img.shields.io/badge/stage-beta.1-purple)

Natural body-part harvesting and transplantation across compatible humanoid races.

## Changes - 1.7.0-beta.1

- Rebuilt runtime code against RimWorld 1.7 / .NET 10.
- Included current 1.6 Workshop XML content in the 1.7 content folder.
- Replaced removed surgery-bill fields with the supported Part properties.
- Fixed a null-parent access when queueing body-part removal.
- Added null-safe natural-transplant recipe handling.
- Added anatomy-based natural surgery availability for humanoid races.
- Kept missing parent parts, incompatible anatomy, and existing game surgery checks intact.
- Added a separate package identity and incompatibility with the original.

## Coverage

Compatible recipes become available on humanoid races that contain the required actual body part. Standard arms and legs are covered on HAR bodies, Anthro variants and compatible gene-based humanoids. Mechanical-only parts, nonexistent limbs and purely visual tails are not converted into biological transplants. Tail items are supplied by the tail fork or race mods.

## Load order

Harmony -> race/content mods -> Harvest All The Tails fork -> this fork -> Harvest Organs Post Mortem -> compatibility patches.

## Credits and Workshop status

Original project: [Ogliss/HarvestEverything](https://github.com/Ogliss/HarvestEverything), based on Ykara's Harvest Everything and later updates. Runtime source was reconstructed from the installed author's 1.6 assembly to match the current Workshop behavior, then updated for 1.7.

**Workshop redistribution pending upstream permission.** No license file was supplied in the upstream repository. A public GitHub fork does not establish permission to redistribute upstream code/assets on Steam. No new license is assigned to upstream content. The original Workshop publishing ID is not used by this fork.

## Repository layout

| Path | Purpose |
|---|---|
| `1.7/` | RimWorld 1.7 content (Assemblies, Defs, Patches) |
| `About/` | Mod metadata and preview image |
| `Languages/` | Translations |
| `Textures/` | Textures |
| `Source17/` | C# source for the 1.7 build |
| `tools/` | Packaging script |

## Build

Requires the .NET 10 SDK, RimWorld 1.7 and Harmony.

```powershell
dotnet build Source17/Mod.csproj -c Release -p:RimWorldDir="M:/Steam/steamapps/common/RimWorld" -p:HarmonyPath="M:/Steam/steamapps/workshop/content/294100/2009463077/1.7/Assemblies/0Harmony.dll"
```

Game and Harmony assemblies are referenced locally and are not included in the 1.7 package.

## Installation

Enable the fork instead of the original. Both versions are declared incompatible to prevent duplicate content. Keep Harmony enabled. Existing upstream item names are retained.
