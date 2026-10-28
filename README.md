# Harvest Everything + Tails

![RimWorld](https://img.shields.io/badge/RimWorld-1.6%20%7C%201.7-orange) ![Stage](https://img.shields.io/badge/stage-beta-purple) ![Tails](https://img.shields.io/badge/tails-supported-green)

Harvest and transplant natural body parts on compatible humanoid races, including natural
tails. Butcher fresh animal corpses for their organs with a dedicated butcher-table bill.

This mod merges Harvest Everything with Harvest All The Tails. Tail harvesting, tail
installation and per-race tail support are built in, so the standalone tails mod is not needed.

## Features

- **Natural body parts.** Harvest and transplant organs, limbs, hands, feet, fingers, toes and tongues on compatible humanoid anatomy.
- **Animal parts.** The **Butcher for animal parts** bill recovers healthy parts from fresh animal corpses and yields half the normal meat. Recovered items show the donor species. Animal transplant ingredients must match the patient's exact species and body-part type.
- **Tails.** Natural tails can be transplanted between compatible animal and humanoid tail anatomy. Newly recovered tails retain their donor name through successful surgery, save/load and later removal. Cosmetic gene tails without an anatomical body part cannot be harvested.
- **Recovery chances.** At 100% butcher efficiency, digits and claws have a 5% chance, hooves/horns/antlers/tusks/beaks 12.5%, and larger parts and organs 25%. Whole parts need intact, healthy children; a recovered parent does not also produce its child parts.
- **Modded animals and races.** Shared anatomy works automatically. Recognized custom body-part names and function tags support tails, limbs, paws, digits, claws, hooves, horns, beaks, tusks, wings, shells, tentacles and natural organs. Unrecognized anatomy requires a specific mapping.
- **Surgery menus.** Part names follow the target anatomy. Matching-animal surgery is offered only when a suitable donor part is available; queued bills wait for donor parts before requesting medical rest.

Old items without saved donor information cannot be given a reliable donor identity. The new donor records have no pain or health effects.

## Surgery

Use an animal bed or sleeping box and suitable medicine. Animal sleeping spots apply a 0.7 surgery factor; beds and boxes use 1.0 before other modifiers. Doctor skill, sight, manipulation and medicine still affect the final chance, and normal surgeries retain their failure risk.

The **remove tail** operation recovers a healthy natural animal tail. Transplant bills require an intact parent body part and a donor that matches the animal's species and exact anatomy; tails use their separate cross-species recipe.

## Compatibility

Works with [Bionic Icons](https://steamcommunity.com/sharedfiles/filedetails/?id=1677616980) to show body-part icons on recovered items. Bionic Icons is optional.

## Load order

Harmony -> race and content mods -> **Harvest Everything + Tails** -> Harvest Organs Post Mortem -> compatibility patches.

## Installation

Supports RimWorld 1.6 and 1.7. Keep Harmony enabled, and enable this mod instead of the original
Harvest Everything or the standalone Harvest All The Tails. All three are declared incompatible
with each other, so only one copy can load. Existing upstream item names are retained.

## Credits

Original Harvest Everything: [Ogliss/HarvestEverything](https://github.com/Ogliss/HarvestEverything),
based on Ykara's Harvest Everything and later updates.

Tails content: [EspioIdsavant/Harvest-All-The-Tails](https://github.com/EspioIdsavant/Harvest-All-The-Tails),
used under the MIT license. The original MIT notice is retained in [LICENSE-Tails](LICENSE-Tails).

**Workshop redistribution pending upstream permission.** No license file was supplied in the
Harvest Everything upstream repository. A public GitHub fork does not establish permission to
redistribute upstream code or assets on Steam. No new license is assigned to upstream content,
and the original Workshop publishing IDs are not used by this fork.

## Building from source

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

## Changes

See [CHANGELOG.md](CHANGELOG.md).

### Small natural parts

Post Mortem autopsies recover eligible fingers and toes at 20% of the normal natural-part chance. Living surgery has no added recovery-loss roll. Matching natural fingers, toes and tongues can be transplanted when their parent anatomy is present.
