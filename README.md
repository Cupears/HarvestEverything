# Harvest Everything + Tails - 1.7 Race Support

![RimWorld](https://img.shields.io/badge/RimWorld-1.7-orange) ![Stage](https://img.shields.io/badge/stage-beta.1-purple) ![Tails](https://img.shields.io/badge/tails-supported-green)

Natural body-part harvesting and transplantation across compatible humanoid races,
**now including natural tails**, plus animal butchering for organs.

This mod merges Harvest Everything with Harvest All The Tails. You no longer need a
separate tails mod: tail harvesting, tail installation and per-race tail support are all
built in here.

## Changes - 1.7.0.2

- Merged Harvest All The Tails into this mod. Tail harvesting and transplantation are now built in.
- Added tail support for 23 tailed humanoid races, including Big and Small bodies, Dogbold, Foxbold, Orassan, Tozol, Yanzihko felines, Frosty, Logann, Ferrex and Xenomorph Type.
- Each supported race maps to its own tail body part and tail item, so the correct tail type is restored.
- Race modules load only when their race mod is active, so nothing breaks if you have no tailed races installed.
- Fixed tail patches whose "if Harvest Everything is loaded" gate could never match, so the compatible item and recipe patches now apply.
- Rebuilt runtime code against RimWorld 1.7 / .NET 10.
- Included current 1.6 Workshop XML content in the 1.7 content folder.
- Replaced removed surgery-bill fields with the supported Part properties.
- Fixed a null-parent access when queueing body-part removal.
- Added null-safe natural-transplant recipe handling.
- Added anatomy-based natural surgery availability for humanoid races.
- Kept missing parent parts, incompatible anatomy, and existing game surgery checks intact.
- Added animal butchering support: fresh animal corpses now give up clean organs.
- Added a separate package identity and incompatibility with the original.

## Tail harvesting

A humanoid with an actual tail body part can have that tail surgically removed and
installed again. Removing a clean tail produces a tail item; the install recipe asks for
that same item, so a removed tail can always be put back. Matching tail types share an
item, and different anatomical tail types do not.

**Visual-only tails are not anatomical parts.** Anthro Race variants and xenotype or gene
appearance tails with no tail BodyPartRecord have nothing to amputate or transplant. No body
structure or gene is invented or changed. See [race coverage](RACE-COVERAGE.md) for the full
51-race audit.

### Playing without tailed races

Tail support is entirely def-driven. Race-specific modules are gated on their race mod
being active; the generic tail content targets a `Tail` body part and stays inert on races
that do not have one. If you install this mod with no tailed race mods at all:

- No errors, red errors or missing-def warnings are produced.
- The race patches for Kurin, Revia, Moyo, Dragonian, Silkiera, Ratkin, Kiiro, Rabbie,
  Glassy, Mofy, Heruan and Nyaron are not loaded at all.
- Generic tail items and recipes exist but simply have no valid pawn part to act on.
- Body-part harvesting and animal butchering continue to work normally.

## Animal butchering

Butchering a fresh animal corpse yields its undamaged organs in addition to vanilla meat,
leather and life-stage parts. Only organ body parts that have a natural body-part item are
dropped (heart, lung, kidney, liver, stomach and brain), and only if the organ is undamaged.
Limbs, jaws, eyes and ears are never taken this way. Mechanoids, anomaly entities and
non-fresh corpses are skipped. The chance per organ scales with butchery efficiency
(25% at 100% efficiency), so a skilled cook recovers more organs.

## Coverage

Compatible recipes become available on humanoid races that contain the required actual body part. Standard arms and legs are covered on HAR bodies, Anthro variants and compatible gene-based humanoids. Mechanical-only parts, nonexistent limbs and purely visual tails are not converted into biological transplants. Tail items come from this mod or from the race mod itself.

## Load order

Harmony -> race and content mods -> **Harvest Everything + Tails** -> Harvest Organs Post Mortem -> compatibility patches.

Do not load the standalone Harvest All The Tails mod alongside this one. It is declared
incompatible because its content is already included here.

## Credits and Workshop status

Original Harvest Everything project: [Ogliss/HarvestEverything](https://github.com/Ogliss/HarvestEverything), based on Ykara's Harvest Everything and later updates. Runtime source was reconstructed from the installed author's 1.6 assembly to match the current Workshop behavior, then updated for 1.7.

Tails content: [EspioIdsavant/Harvest-All-The-Tails](https://github.com/EspioIdsavant/Harvest-All-The-Tails), used under the MIT license. The original MIT notice is retained in [LICENSE-Tails](LICENSE-Tails).

**Workshop redistribution pending upstream permission.** No license file was supplied in the Harvest Everything upstream repository. A public GitHub fork does not establish permission to redistribute upstream code/assets on Steam. No new license is assigned to upstream content. The original Workshop publishing IDs are not used by this fork.

## Repository layout

| Path | Purpose |
|---|---|
| `1.7/` | RimWorld 1.7 content (Assemblies, Defs, Patches) |
| `About/` | Mod metadata and preview image |
| `Languages/` | Translations |
| `Textures/` | Textures |
| `Source17/` | C# source for the 1.7 build |
| `tools/` | Packaging script |
| `Races17/` | Insect tail patches (def-driven fallback) |
| `Kurin/`, `Revia/`, `Moyo/`, `Dragonian/`, `DragonianUnofficial/`, `Silkiera/`, `Ratkin/`, `Kiiro/`, `Rabbie/`, `Glassy/`, `Mofy/`, `Heruan/`, `Nyaron/` | Race modules for HAR and anthro races |
| `BigSmall/`, `Dogbold/`, `Foxbold/`, `Orassan/`, `Tozol/`, `Yanzihko/`, `Xenomorph/` | Race modules for tailed non-HAR races |

## Build

Requires the .NET 10 SDK, RimWorld 1.7 and Harmony.

```powershell
dotnet build Source17/Mod.csproj -c Release -p:RimWorldDir="M:/Steam/steamapps/common/RimWorld" -p:HarmonyPath="M:/Steam/steamapps/workshop/content/294100/2009463077/1.7/Assemblies/0Harmony.dll"
```

Game and Harmony assemblies are referenced locally and are not included in the 1.7 package.

## Installation

Enable this mod instead of the original Harvest Everything. Enable only one copy: the
original, the standalone tails mod and this merge are declared incompatible to prevent
duplicate content. Keep Harmony enabled. Existing upstream item names are retained.
