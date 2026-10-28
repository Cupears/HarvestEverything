# Current humanoid race anatomy audit

Every humanlike race below is checked against the tail body part it actually has.
`Tail item` is the item the removed tail turns into and the item the install
recipe asks for (either the race mod's own item or one that Harvest Everything + Tails adds).

This table covers the tail half of the mod. Natural body-part harvesting (arms, legs,
organs) is driven directly by the anatomy each race defines, so it works on any compatible
humanoid without a per-race entry here.

| Race | Definition | Body | Anatomical tail | Tail item |
|---|---|---|---|---|
| human | Human | Human | No anatomical tail | - |
| human | CreepJoiner | Human | No anatomical tail | - |
| Lapelli | xenotype on Human | Human | No anatomical tail | - |
| centauroid | BS_Centaur | BS_Centaur_Body | Tail | HATT17_Tail |
| Four Armed Human | BS_HumanFouramed | BS_FourArmedHuman | No anatomical tail | - |
| snake-person | BS_Naga | BS_SnakeHuman | Tail | HATT17_Tail |
| six-armed snake-person | BS_Marlith | BS_Marlith | Tail | HATT17_Tail |
| mech | BS_UrbMech | BS_StandardAndroid | No anatomical tail | - |
| Six Armed Human | BS_SixArmedHuman | BS_SixArmedHuman | No anatomical tail | - |
| spider-person | BS_SpiderPersonRace | BS_SpiderHybrid | No anatomical tail | - |
| tailed humanoid | BS_HumanoidWithTail | BS_HumanoidWithTail_Body | Tail | HATT17_Tail |
| winged humanoid | BS_HumanoidWithWings | BS_HumanoidWithWings_Body | No anatomical tail | - |
| disembodied head | BS_JustAHead | BS_JustAHead | No anatomical tail | - |
| Camerline Kobold | Alien_Dogbold | DogboldBody | DogboldTail | DogboldTail |
| Venerek Kobold | AFoxbold | FoxboldBody | FoxboldTail | FoxboldTail |
| generic anthro | Anthro | Anthro | No anatomical tail | - |
| canine anthro | CanineAnthro | Anthro | No anatomical tail | - |
| feline anthro | FelineAnthro | Anthro | No anatomical tail | - |
| scaly anthro | ReptileAnthro | Anthro | No anatomical tail | - |
| protogen | Protogen | Anthro | No anatomical tail | - |
| synth | Synth | Anthro | No anatomical tail | - |
| amphibian anthro | AmpAnthro | Anthro | No anatomical tail | - |
| insect anthro | BugAnthro | Anthro | No anatomical tail | - |
| bat anthro | BatAnthro | Anthro | No anatomical tail | - |
| bunny anthro | BunAnthro | Anthro | No anatomical tail | - |
| bovidae anthro | BovAnthro | Anthro | No anatomical tail | - |
| avali | Avali | Anthro | No anatomical tail | - |
| sergal | Sergal | Anthro | No anatomical tail | - |
| avian anthro | AvianAnthro | Anthro | No anatomical tail | - |
| fish anthro | FishAnthro | Anthro | No anatomical tail | - |
| mustelid anthro | MustAnthro | Anthro | No anatomical tail | - |
| skull anthro | SkullAnthro | Anthro | No anatomical tail | - |
| glutton | BS_GluttonRace | BS_Glutton | No anatomical tail | - |
| insect-like humanoid | BS_InsectoidHumanoid_FourArmed | BS_InsectoidHumanoid_FourArmedBody | No anatomical tail | - |
| Frosty | FrostyRace | Frosty_body | Tail | HATT17_Tail |
| neko | Alien_OrassanHumanHybrid | Orassan | OTail | OTail |
| orassan | Alien_Orassan | Orassan | OTail | OTail |
| tozol | Alien_Tozol | Tozol_Body | Tozol_Tail | HATT17_Tozol_Tail |
| Cheetah | YanzihkoCheetah | Feles | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Jaguar | YanzihkoJaguar | Feles | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Leo | YanzihkoLeo | Feles | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Panther | YanzihkoPanther | Feles | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Tigris | YanzihkoTigris | Feles | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Fressalfar | Fressalfar | Fressalfar | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Kisalfar | Kisalfar | Kisalfar | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Lombax | Lombax | Lombax | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Vulpoid | Yanz_Vulpoid | Yanz_Vulpoid | Yanz_Base_Tail | HATT17_Yanz_Base_Tail |
| Logann | Alien_Wolvx | Wolvx | Tail | Tail / HATT17_Tail |
| Ferrex | Alien_Ferrex | Ferrex | Tail | Tail / HATT17_Tail |
| xenomorph | XMT_Starbeast_AlienRace | XMT_Adult | StarbeastTail | HATT17_StarbeastTail |
| royal xenomorph | XMT_Royal_AlienRace | XMT_Queen | StarbeastTail | HATT17_StarbeastTail |

## Notes

- `Tail` is the shared generic body part. Several mods reuse it (Big and Small bodies,
  Frosty, Logann, Ferrex). When a race mod already supplies a `Tail` item, that item is
  reused; otherwise this mod adds `HATT17_Tail`. The generic install recipe accepts both
  `HATT17_Tail` and the race mod's `Tail` item, so a removed tail can always be put back.
- Logann and Ferrex both define a body part and an item called `Tail`, so RimWorld keeps
  only one of the two `Tail` item defs depending on load order. The tail still maps to the
  correct body part, and both races share the same generic tail item.
- Dogbold, Foxbold and Orassan already ship their own tail item (`DogboldTail`,
  `FoxboldTail`, `OTail`); this mod only adds the install/remove recipes for them.
- Tozol defines both `Tozol_Tail` (body part) and `TozolTail` (older 1.3/1.4 body part
  name). Only `Tozol_Tail` is used by the 1.5+ `Tozol_Body`, so only it is patched.
- Xenomorph Type's humanlike races use `StarbeastTail` (tagged as a moving/manipulation
  limb). Removing it costs the tail's share of moving/manipulation capacity.
- `Leaftail` and `VaporeonTail` exist as body part defs in Yanzihko Race Core, but no body
  uses them in the installed version, so they need no patch.
- Cat Hiss is an ability mod, not a race mod.
- HAR-based races get Human recipes automatically via HAR's `humanRecipeImport`, so the
  legacy per-race patches only need to supply the tail item and the recipe definitions.
- Race modules are only loaded when their race mod is active. In a game with no tailed
  race mods installed, the race-specific defs and recipes never load, and the generic
  tail recipes simply have no valid target body part.
