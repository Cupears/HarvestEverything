# Harvest Everything + Tails

![RimWorld](https://img.shields.io/badge/RimWorld-1.6%20%7C%201.7-orange) ![Stage](https://img.shields.io/badge/stage-beta.1-purple) ![Tails](https://img.shields.io/badge/tails-supported-green)

Harvest and transplant natural body parts on compatible humanoid races, including natural
tails. Butcher fresh animal corpses for their organs with a dedicated butcher-table bill.

This mod merges Harvest Everything with Harvest All The Tails. Tail harvesting, tail
installation and per-race tail support are built in, so the standalone tails mod is not needed.

## Features

- **Body parts.** Harvest and reinstall natural body parts on humanoid races that actually have
  the part. Missing parts, incompatible anatomy and vanilla surgery checks are respected.
- **Tails.** A race with a real tail body part can have it removed and reinstalled. The removed
  tail becomes an item and the install recipe asks for that same item. Each race maps to its own
  tail type, so the correct tail is restored.
- **Animal butchering.** A separate **Butcher for animal parts** bill on the butcher spot or
  butcher table recovers undamaged organs that have a natural body-part item: heart, lung, kidney,
  liver, stomach and brain. Each organ is labelled with the source animal, for example
  "muffalo heart", and cannot be transplanted into humanlikes. The chance per organ scales with
  butchery efficiency, at 25% for 100% efficiency. This bill yields half the meat of ordinary
  butchering; the ordinary **Butcher creature** bill is unchanged and never recovers organs.
- **Race support.** 23 tailed humanoid races, listed in [RACE-COVERAGE.md](RACE-COVERAGE.md).
  Race-specific content only loads when that race mod is active.

Purely visual tails, such as gene or xenotype appearance tails and anthro variants with no tail
body part, have nothing to remove and are not touched.

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

Natural fingers and toes can be transplanted into matching humanoid anatomy. Post Mortem autopsies use a separate recovery roll for each eligible digit, at 20% of the usual natural-part chance. Whole limbs and their loose digits are not both recovered. Damaged or missing child parts prevent the enclosing limb counting as intact. Surgery on living pawns has no added recovery chance penalty. Mechanical digits are excluded.

Natural tongues can be recovered from eligible humanoid corpses and transplanted with surgery.

Animal body-part support starts with shared RimWorld anatomy. Recovered parts keep the source species in their item label and stack only with matching source and part. Natural tails can be transplanted between pawns whose anatomy uses the shared `Tail` body-part type. Race-specific anatomy is handled in later compatibility updates.
