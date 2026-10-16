# Onshape FeatureScript — Parametric Knurling

Parametric knurling for industrial design, built with Onshape FeatureScript and AI-assisted coding.

This project explores how a designer can turn a practical surface-detailing need into a reusable CAD tool. The focus is on editable geometry and controlled variation: comparing grip character, visual proportion, tactile intent, and surface detail during product development.

## Why Onshape

Dense knurling creates repeated faces, edges, and intersections. As the pattern becomes more complex, regeneration can become a demanding part of the modeling process on a local workstation.

Onshape's cloud-native approach is particularly valuable here: geometry regeneration runs in the cloud, while the designer works through a browser. This reduces dependence on a single workstation for modeling calculations and keeps the CAD workflow accessible across devices. Complex patterns still require computation; regeneration time depends on the geometry and feature implementation.

FeatureScript brings the pattern logic into a reusable custom feature, keeping adjustments within the normal Onshape modeling workflow.

## AI-Assisted Development

Knurling is an established application that can already be modeled parametrically. This case study explores how existing parametric modeling knowledge can guide AI-assisted coding and become a reusable part of the design workflow.

Existing parametric examples provide the reference: their geometry, parameters, and modeling logic help explain the intended behavior to AI. Through iterative guidance and testing in Onshape, that logic is translated into FeatureScript.

The aim is to bring established modeling methods into the design workflow more efficiently. A reusable custom feature lets the designer adjust parameters and compare variations while keeping control over the design intent and evaluating the resulting geometry.

## Design Workflow

1. Add the custom FeatureScript to Onshape.
2. Open the feature from the toolbar.
3. Adjust the knurling parameters.
4. Regenerate and compare variations.

## Source and Parameters

The FeatureScript source has not yet been added to this repository. The exact parameter names, units, defaults, and supported ranges will be documented against that source once it is available.

## Design Evaluation

Evaluate each variation for grip, tactile character, edge quality, and consistency with the product's form and material. Manufacturing suitability should be assessed for the intended process and dimensions.
