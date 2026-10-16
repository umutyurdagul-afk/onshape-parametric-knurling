# Parametric Knurling for Onshape

[English](README.md) · [中文](README.zh-CN.md) · [Français](README.fr.md) · [Italiano](README.it.md) · [Türkçe](README.tr.md)

Four FeatureScript tools for editable knurling on cylindrical, square and rectangular parts—developed for industrial design with AI-assisted coding.

Created by [Umut Yurdagül](https://github.com/umutyurdagul-afk) · Industrial Design

## Demo

**[Watch the video](https://github.com/umutyurdagul-afk/onshape-parametric-knurling/blob/main/media/onshape-parametric-knurling-demo.mp4)** · [Download MP4](https://raw.githubusercontent.com/umutyurdagul-afk/onshape-parametric-knurling/main/media/onshape-parametric-knurling-demo.mp4)

A recorded demonstration of the project in Onshape.

## Feature variants

Each file creates a new independent part along the Z axis. Use a separate Feature Studio for each variant; the scripts share enum and helper names.

| File | Geometry and coverage |
| --- | --- |
| [Cylinder — Full Surface](featurescripts/knurl-cylinder-full-surface.fs) | Cylinder with no unknurled margins or axial bands. |
| [Cylinder — Inset](featurescripts/knurl-cylinder-inset.fs) | Cylinder with top/bottom margins and optional axial bands. |
| [Prism — Full Surface](featurescripts/knurl-prism-full-surface.fs) | Square or rectangular prism, patterned on all four lateral faces. |
| [Prism — Inset](featurescripts/knurl-prism-inset.fs) | Square or rectangular prism with top, bottom, and side borders. |

“Inset” refers to a pattern set back from the edges, rather than knurling an internal bore.

## Design workflow

1. Add the custom FeatureScript to Onshape.
2. Open the feature from the toolbar.
3. Adjust the knurling parameters.
4. Regenerate and compare variations.

## Parameters

The following values come from the uploaded FeatureScript 2716 parameter declarations. Values are shown as **default (declared range)**; geometry checks can further restrict valid combinations.

| Control | Default (declared range) |
| --- | --- |
| Cylinder diameter / prism width | 30 mm (2–500 mm) |
| Rectangular prism depth | 20 mm (2–500 mm); square section enabled by default |
| Body length / prism height | 40 mm (2–1000 mm) |
| Cylinder groove count, per direction | 32 (8–120) |
| Prism developed-perimeter groove count | 48 (8–240), rounded to a multiple of four |
| Requested pitch, as an alternative to count | 2.5 mm (0.2–50 mm) |
| Groove depth | 0.45 mm (0.02–10 mm) |
| Helix / diagonal angle from the body axis | 30° (5–65°) |
| Included V angle | 120° (30–150°) |
| Crest land ratio, as an alternative to V angle | 0.2 (0.05–0.85) |
| End chamfer offset / fillet radius | 0.8 mm (0.01–100 mm) |
| Chamfer angle | 45° (10–80°) |
| Prism corner fillet radius / chamfer offset | 0.8 mm (0.01–100 mm) |
| Inset cylinder top / bottom margins | 2 mm (0–500 mm) |
| Inset prism top / bottom / side borders | 2 mm (0–200 mm) |
| Inset cylinder axial bands | 1 (1–6); gap 3 mm (0.1–100 mm) |
| Pattern phase | 0° (0–360°) |
| Groove root flat width | 0 mm (0–5 mm) |
| Optional crest / root rounding radius | 0.05 mm (0.005–2 mm) |
| Cutter overrun, full-surface variants and inset prism | 1 mm (0–50 mm) at each end |

All variants offer diamond, straight, and unpatterned styles, plus helical styles for cylinders or diagonal styles for prisms. Knurl modes are cut, raised, or half raised / half cut. Size references distinguish nominal body dimensions from maximum outside dimensions. End treatments can be sharp, chamfered, or filleted.

Pitch is a requested value: the scripts derive an integer repeat count, so actual pitch can differ. Controls appear conditionally according to style, density, profile, and advanced settings.

## Why Onshape

Dense knurling creates repeated faces, edges, and intersections. As the pattern becomes more complex, regeneration can become a demanding part of the modeling process on a local workstation.

Onshape's cloud-native approach is particularly valuable here: geometry regeneration runs in the cloud, while the designer works through a browser. This reduces dependence on a single workstation for modeling calculations and keeps the CAD workflow accessible across devices. Complex patterns still require computation; regeneration time depends on the geometry and feature implementation.

FeatureScript brings the pattern logic into a reusable custom feature, keeping adjustments within the normal Onshape modeling workflow.

## AI-assisted development

This project explores how a designer can turn a practical surface-detailing need into a reusable CAD tool. The focus is on editable geometry and controlled variation: comparing grip character, visual proportion, tactile intent, and surface detail during product development.

Knurling is an established application that can already be modeled parametrically. This case study explores how existing parametric modeling knowledge can guide AI-assisted coding and become a reusable part of the design workflow.

Existing parametric examples provide the reference: their geometry, parameters, and modeling logic help explain the intended behavior to AI. Through iterative guidance and testing in Onshape, that logic is translated into FeatureScript.

The aim is to bring established modeling methods into the design workflow more efficiently. A reusable custom feature lets the designer adjust parameters and compare variations while keeping control over the design intent and evaluating the resulting geometry.

## Implementation status

The uploaded source is preserved unchanged, including feature identifiers and library version 2716. Repository checks confirm the file organization and parameter documentation; compilation and regeneration in Onshape have not been performed as part of this repository update.

## Design evaluation

Evaluate each variation for grip, tactile character, edge quality, and consistency with the product's form and material. Manufacturing suitability should be assessed for the intended process and dimensions.
