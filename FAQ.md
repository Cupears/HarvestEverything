# Frequently asked questions

## Where do animal parts come from?

Add **Butcher for animal parts** at a butcher table or butcher spot. It recovers healthy parts from fresh animal corpses and produces half the usual meat. Ordinary butchering keeps its normal products.

## What are the recovery chances?

At 100% butcher efficiency:

| Part group | Chance per eligible part |
| --- | --- |
| Fingers, toes and claws | 5% |
| Hooves, horns, antlers, tusks, beaks and antennae | 12.5% |
| Larger parts and organs | 25% |

Butcher efficiency scales these chances. Missing, damaged or artificial parts are excluded. Whole parts need intact, healthy children; a recovered parent does not also produce its child parts.

## Why is a transplant missing from the Operations menu?

The patient must have compatible anatomy and a suitable donor part must be available. Animal donors must match the patient's exact species and body-part type. The parent body part must still be present. Medicine and the normal surgery requirements also apply.

## Why does surgery fail with a skilled doctor?

The mod uses RimWorld's normal surgery rules. Doctor skill, sight, manipulation, medicine and the patient's bed affect the chance.

Animal sleeping spots apply a **0.7** surgery factor. Animal beds and sleeping boxes use **1.0** before other modifiers. Use a bed or sleeping box with suitable medicine; a small failure risk remains even under good conditions.

## Can tails be used on another species?

Yes, where the recipient has compatible natural tail anatomy. Newly recovered tails keep the original donor name through transplantation, save/load and later removal.

Use **remove tail** to recover a healthy natural animal tail.

## Why can some visible tails not be harvested?

A tail must exist as an anatomical body part. Cosmetic gene or xenotype tails may only change appearance and have no part to remove.

## How do fingers and toes work with Post Mortem?

Eligible fingers and toes use **20% of the normal natural-part recovery chance** during Post Mortem autopsies. For example, a 40% autopsy chance gives an 8% chance per eligible digit. Living surgery has no added recovery-loss roll.

## Does it support modded animals and hybrids?

Shared anatomy works automatically. Recognized body-part names and function tags cover supported custom organs and appendages. Unknown anatomy needs a specific mapping. Core body sections and unsupported mechanical anatomy are excluded.

See [Race support](RACE-COVERAGE.md) for the race modules included with the mod.

## What happens to older parts without donor data?

Their source cannot be reliably reconstructed. Items without the required donor identity are rejected for matching-animal transplants rather than guessed.

## Does it work with Bionic Icons?

Yes. When Bionic Icons is active, recovered parts use its artwork. Animal artwork is used where available; internal organs keep their organ-specific icons. Some parts use the closest available fallback.
