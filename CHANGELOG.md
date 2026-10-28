## Changes — 1.7.0-beta.1

- Rebuilt runtime code against RimWorld 1.7 / .NET 10.
- Included current 1.6 Workshop XML content in the 1.7 content folder.
- Replaced removed surgery-bill fields with the supported Part properties.
- Fixed a null-parent access when queueing body-part removal.
- Added null-safe natural-transplant recipe handling.
- Added anatomy-based natural surgery availability for humanoid races.
- Kept missing parent parts, incompatible anatomy, and existing game surgery checks intact.
- Added animal butchering support: fresh animal corpses now give up clean, undamaged organs (heart, lung, kidney, liver, stomach, brain).
- Added a separate package identity and incompatibility with the original.
