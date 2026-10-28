## Changes - 1.7.1.10

- Added matching animal-part transplant surgery for damaged or missing parts with an intact natural parent part.
- Ingredient selection requires the donor race definition and exact body-part definition to match the patient and surgery target. Tails retain their separate cross-species recipe.
- Added saved anatomical identity to recovered parts. Unknown old donor records are rejected safely.
- Fixed external-part collection skipping parts without a pre-existing removal item.
- Built for 1.6 and 1.7; seven matching and patch-binding checks passed, including a modded-race fixture. Live surgery remains to be checked in game.

## Changes - 1.7.1.9

- Extended source-labeled collection and tail replacement to modded body-part names containing a supported part type.
- Tail transplant surgery now includes compatible custom tail anatomy and source-labeled animal tail items.
- Built successfully for RimWorld 1.6 and 1.7.

## Changes - 1.7.1.8

- Started animal-part support with shared RimWorld anatomy: tails, paws/feet, fingers/toes, claws, hooves, horns/antlers, tusks and beaks.
- Butchered parts are labeled with the source species and exact body-part label; different species and part types do not stack together.
- Added a tail transplant recipe for animals and humanoids that share the standard Tail body-part type. The tail keeps its source-species name in storage and on the ingredient.
- Mod-specific body-part types remain for the next compatibility pass.

## Changes - 1.7.1.7

- Added a recoverable natural tongue and a matching transplant surgery.
- Humanlike corpse autopsies can recover tongues through the existing natural-part recovery rules.
- An existing tongue item mapping from another mod is preserved.

## Changes - 1.7.1.6

- Added natural finger and toe items and matching transplant recipes for humanoid anatomy.
- Existing finger/toe item mappings from other mods remain unchanged.
- Post Mortem corpse autopsies recover each eligible finger/toe at 20% of the normal natural-part chance. For a 40% autopsy chance, each digit has an 8% final chance.
- Missing, injured or implanted digits remain subject to Post Mortem health checks.
- Damaged or missing child parts prevent an enclosing limb being recovered as fully intact; surviving smaller parts can then be considered.
- Whole recovered limbs retain their digits; no duplicate loose fingers/toes are produced alongside the enclosing limb.
- Autopsy age limits, skill scaling and total-part limits remain in effect. Living surgery has no added random recovery loss.
- Built for 1.6 and 1.7; 27 automated checks passed, including Harmony binding against the installed Post Mortem assembly. In-game extraction/transplant testing remains pending.

## Changes - 1.7.1.5

- Fixed animal organ names repeating the species twice.
- Stack labels keep their normal counts.
- Existing organs use the corrected labels without save edits.

## Changes - 1.7.1.4

- Animal organs now come from a separate **Butcher for animal parts** bill on the butcher spot and butcher table, instead of the ordinary **Butcher creature** bill.
- The organ bill only accepts animal corpses and yields half the meat of ordinary butchering.
- Ordinary butchering is unchanged: it gives full meat and leather and never recovers organs.
- The new bill is added to every work table that already supports creature butchery, including modded tables.

## Changes - 1.7.1.3

- Animal butchering now produces animal-specific organs labelled with the source species, for example "muffalo heart" or "muffalo brain", instead of the human donor organs.
- Animal organs cannot be installed in humanlikes. They remain sellable.
- Added the `HET_AnimalHeart`, `HET_AnimalLung`, `HET_AnimalKidney`, `HET_AnimalLiver`, `HET_AnimalStomach` and `HET_AnimalBrain` item defs.
- Organs already present in existing saves are unchanged; only new butchering uses the animal items.

## Changes - 1.7.1.2

- About.xml now states requirements and conflicts: Harmony is a hard dependency; Humanoid Alien Races and tailed-race mods are optional.
- Added the old `Cupears.HarvestEverything17` package id to `incompatibleWith` so an older copy cannot load alongside this one.
- Fixed package id casing on the existing incompatibility entries and removed a duplicated `loadBefore` entry for Harvest Organs Post Mortem.

## Changes - 1.7.1.1

- Renamed the assembly to `HarvestEverythingTails` so it no longer shares a name with the original Harvest Everything DLL.
- Changed the package id to `cupears.harvesteverythingtails`.
- Renamed the local mod folder to `Cupears-HarvestEverything-Tails`.
- Existing saves reference the old package id and will list it as missing once; defs and items are unchanged, so no content is lost.

## Changes - 1.7.1.0

- Added RimWorld 1.6 support with a dedicated `1.6/` content folder and a `<v1.6>` load-folder block.
- Added `Source16`, which compiles the shared C# sources against the 1.6 assemblies for .NET Framework 4.7.2.
- Verified the shared XML against the original 1.6 Workshop release: defs identical, patches content-identical.
- About now lists both 1.6 and 1.7 as supported versions.

## Changes - 1.7.0.2

- Merged Harvest All The Tails into this mod; the standalone tails mod is no longer required.
- Added tail harvesting and transplantation for 23 tailed humanoid races.
- Added per-race tail mapping so each race restores its own tail type and item.
- Added generic `Tail`, Tozol, Yanzihko and xenomorph tail items and recipes.
- Added Logann, Ferrex and Xenomorph Type coverage.
- The generic tail install recipe also accepts race-supplied `Tail` items.
- Reused native Dogbold, Foxbold and Orassan tail items.
- Added anatomy-based surgery availability across installed humanoid races.
- Fixed job-string typos in the race patches.
- Unwrapped tail patches whose "if Harvest Everything is loaded" gate could never match.
- Race modules are gated on their race mod, so a game with no tailed races loads no race defs and produces no errors.
- Moved every race module under `Races/`; load gating is unchanged.
- Declared incompatibility with `cupears.harvestalltails17` and `espio.harvestkurintails`.

## Changes - 1.7.0-beta.1

- Rebuilt runtime code against RimWorld 1.7 / .NET 10.
- Included current 1.6 Workshop XML content in the 1.7 content folder.
- Replaced removed surgery-bill fields with the supported Part properties.
- Fixed a null-parent access when queueing body-part removal.
- Added null-safe natural-transplant recipe handling.
- Added anatomy-based natural surgery availability for humanoid races.
- Kept missing parent parts, incompatible anatomy, and existing game surgery checks intact.
- Added animal butchering support: fresh animal corpses now give up clean, undamaged organs (heart, lung, kidney, liver, stomach, brain).
- Added a separate package identity and incompatibility with the original.
