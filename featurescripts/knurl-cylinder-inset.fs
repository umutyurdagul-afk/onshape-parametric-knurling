FeatureScript 2716;
import(path : "onshape/std/geometry.fs", version : "2716.0");

// KNURL CYLINDER v1.4 - per-curve normal plane / sketch / Sweep.
// Uses the user's curve-normal Sweep workflow; end treatments are last.
// Default sweep transport controls rotation about the tangent: verify groove
// depth along long helices before using this geometry for manufacture.
// Paste this entire file into a NEW Feature Studio, commit, then add
// "Knurl Cylinder" from the CURRENT DOCUMENT to the Part Studio toolbar.
// Uses the library version already loaded successfully for Model 14.
// API-reviewed and locally checked; not compiled/regenerated in Onshape.
// Geometry is cut/swept, not a texture and not a simulation of knurl forming.
// Z axis, bottom at world origin; the feature creates one new independent part.

export enum KCStyle
{
    annotation { "Name" : "Diamond / Baklava" } DIAMOND,
    annotation { "Name" : "Straight / Duz" } STRAIGHT,
    annotation { "Name" : "Helical / Helisel" } HELICAL,
    annotation { "Name" : "None / Tirtilsiz" } NONE
}
export enum KCEdge
{
    annotation { "Name" : "Chamfer / Pah" } CHAMFER,
    annotation { "Name" : "Fillet / Radyus" } FILLET,
    annotation { "Name" : "Sharp / Keskin" } SHARP
}
export enum KCDensity
{
    annotation { "Name" : "Groove count per direction" } COUNT,
    annotation { "Name" : "Requested circumferential pitch" } PITCH
}
export enum KCMode
{
    annotation { "Name" : "Cut into body" } CUT,
    annotation { "Name" : "Raised above body" } RAISED,
    annotation { "Name" : "Half raised / half cut" } BALANCED
}
export enum KCDiameter
{
    annotation { "Name" : "Body diameter" } BODY,
    annotation { "Name" : "Maximum knurl outside diameter" } OUTSIDE
}
export enum KCProfile
{
    annotation { "Name" : "V angle + depth" } ANGLE,
    annotation { "Name" : "Nominal crest land ratio + depth" } LAND
}

function kcCylinder(context is Context, id is Id, radius is ValueWithUnits,
                    z0 is ValueWithUnits, z1 is ValueWithUnits)
{
    fCylinder(context, id, {
        "bottomCenter" : vector(0 * millimeter, 0 * millimeter, z0),
        "topCenter" : vector(0 * millimeter, 0 * millimeter, z1),
        "radius" : radius
    });
}

function kcEnd(context is Context, id is Id, edges is Query, style is KCEdge,
               sizeValue is ValueWithUnits, angle is ValueWithUnits,
               flip is boolean)
{
    if (style == KCEdge.SHARP)
        return;
    if (style == KCEdge.FILLET)
        opFillet(context, id + "round", { "entities" : edges,
            "radius" : sizeValue, "tangentPropagation" : false });
    else
        opChamfer(context, id + "chamfer", {
            "entities" : edges, "chamferType" : ChamferType.OFFSET_ANGLE,
            "width" : sizeValue, "angle" : angle,
            "oppositeDirection" : flip, "tangentPropagation" : false
        });
}

function kcAllowance(style is KCEdge, s is ValueWithUnits,
                     angle is ValueWithUnits) returns ValueWithUnits
{
    if (style == KCEdge.SHARP)
        return 0 * millimeter;
    if (style == KCEdge.FILLET)
        return s;
    // Conservative clearance for either offset-angle reference direction.
    return max(s, max(s * tan(angle), s / tan(angle)));
}

// qEdgeConvexityTypeFilter is absent in library 2716.
// Evaluate each edge with the older, supported evaluator instead.
function kcConvexityFilter(context is Context, candidates is Query,
                          wanted is EdgeConvexityType) returns Query
{
    var matches = [];
    for (var candidate in evaluateQuery(context, candidates))
    {
        if (evEdgeConvexity(context, { "edge" : candidate }) == wanted)
            matches = append(matches, candidate);
    }
    return qUnion(matches);
}

function kcSeed(context is Context, id is Id, radius is ValueWithUnits,
                z0 is ValueWithUnits, z1 is ValueWithUnits,
                depth is ValueWithUnits, halfTangent is number,
                rootFlat is ValueWithUnits, beta is ValueWithUnits,
                clockwise is boolean, phase is ValueWithUnits) returns Query
{
    var over = max(0.2 * millimeter, depth);
    var halfWidth = rootFlat / 2 + (depth + over) * halfTangent;
    var extension = halfWidth + depth + 0.5 * millimeter;
    var startZ = z0 - extension;
    var endZ = z1 + extension;
    var rootRadius = radius - depth;
    var radial = vector(cos(phase), sin(phase), 0);
    var origin = rootRadius * radial + vector(0 * millimeter, 0 * millimeter, startZ);
    var normal = vector(0, 0, 1);
    var cleanup = [];
    var pathEdges = qNothing();
    if (beta > 0 * degree)
    {
        var helixPitch = 2 * PI * radius / tan(beta);
        opHelix(context, id + "curve", {
            "direction" : vector(0, 0, 1),
            "axisStart" : vector(0 * millimeter, 0 * millimeter, startZ),
            "startPoint" : origin, "interval" : [0, (endZ - startZ) / helixPitch],
            "clockwise" : clockwise, "helicalPitch" : helixPitch,
            "spiralPitch" : 0 * millimeter
        });
        pathEdges = qCreatedBy(id + "curve", EntityType.EDGE);
        var curveTangent = evEdgeTangentLine(context, { "edge" : pathEdges, "parameter" : 0 });
        // Use the lower endpoint even if the kernel reverses edge orientation.
        if (curveTangent.origin[2] > startZ + 0.001 * millimeter)
            curveTangent = evEdgeTangentLine(context, { "edge" : pathEdges, "parameter" : 1 });
        origin = curveTangent.origin;
        normal = curveTangent.direction;
        radial = normalize(vector(origin[0], origin[1], 0 * millimeter));
        cleanup = append(cleanup, qCreatedBy(id + "curve", EntityType.BODY));
    }
    // An explicit plane per curve: its normal IS the curve tangent.
    // No face lock and no fixed global profile orientation.
    var profilePlane = plane(origin, normal, radial);
    var sid = id + "profile";
    var sk = newSketchOnPlane(context, sid, { "sketchPlane" : profilePlane });
    var points = [];
    if (rootFlat > 0 * millimeter)
        points = [vector(0 * millimeter, -rootFlat / 2),
                  vector(depth + over, -halfWidth), vector(depth + over, halfWidth),
                  vector(0 * millimeter, rootFlat / 2), vector(0 * millimeter, -rootFlat / 2)];
    else
        points = [vector(0, 0) * millimeter,
                  vector(depth + over, -halfWidth), vector(depth + over, halfWidth),
                  vector(0, 0) * millimeter];
    skPolyline(sk, "cutter", { "points" : points });
    skSolve(sk);
    cleanup = append(cleanup, qCreatedBy(sid, EntityType.BODY));
    if (beta == 0 * degree)
        opExtrude(context, id + "seed", {
            "entities" : qSketchRegion(sid), "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND, "endDepth" : endZ - startZ
        });
    else
        opSweep(context, id + "seed", {
            "profiles" : qSketchRegion(sid), "path" : pathEdges,
            "keepProfileOrientation" : false
        });
    opDeleteBodies(context, id + "cleanup", { "entities" : qUnion(cleanup) });
    kcCylinder(context, id + "clip", radius + over + 2 * halfWidth, z0, z1);
    opBoolean(context, id + "trim", {
        "tools" : qUnion([qCreatedBy(id + "seed", EntityType.BODY),
                            qCreatedBy(id + "clip", EntityType.BODY)]),
        "operationType" : BooleanOperationType.INTERSECTION
    });
    return qUnion([qCreatedBy(id + "seed", EntityType.BODY),
                   qCreatedBy(id + "clip", EntityType.BODY),
                   qCreatedBy(id + "trim", EntityType.BODY)]);
}

annotation { "Feature Type Name" : "Knurl Cylinder v1.4" }
export const knurlCylinder = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Cylinder diameter" }
        isLength(definition.diameter, { (millimeter) : [2, 30, 500] } as LengthBoundSpec);
        annotation { "Name" : "Cylinder length" }
        isLength(definition.length, { (millimeter) : [2, 40, 1000] } as LengthBoundSpec);
        annotation { "Name" : "Knurl style" }
        definition.style is KCStyle;

        annotation { "Name" : "Bottom edge" }
        definition.bottomEdge is KCEdge;
        if (definition.bottomEdge != KCEdge.SHARP)
        {
            annotation { "Name" : "Bottom chamfer offset / fillet radius" }
            isLength(definition.bottomSize, { (millimeter) : [0.01, 0.8, 100] } as LengthBoundSpec);
            if (definition.bottomEdge == KCEdge.CHAMFER)
            {
                annotation { "Name" : "Bottom chamfer angle" }
                isAngle(definition.bottomAngle, { (degree) : [10, 45, 80] } as AngleBoundSpec);
                annotation { "Name" : "Flip bottom chamfer reference", "Default" : false }
                definition.bottomFlip is boolean;
            }
        }
        annotation { "Name" : "Use same edge settings at both ends", "Default" : true }
        definition.sameEnds is boolean;
        if (!definition.sameEnds)
        {
            annotation { "Name" : "Top edge" }
            definition.topEdge is KCEdge;
            if (definition.topEdge != KCEdge.SHARP)
            {
                annotation { "Name" : "Top chamfer offset / fillet radius" }
                isLength(definition.topSize, { (millimeter) : [0.01, 0.8, 100] } as LengthBoundSpec);
                if (definition.topEdge == KCEdge.CHAMFER)
                {
                    annotation { "Name" : "Top chamfer angle" }
                    isAngle(definition.topAngle, { (degree) : [10, 45, 80] } as AngleBoundSpec);
                    annotation { "Name" : "Flip top chamfer reference", "Default" : false }
                    definition.topFlip is boolean;
                }
            }
        }

        if (definition.style != KCStyle.NONE)
        {
            annotation { "Name" : "Knurl mode" }
            definition.mode is KCMode;
            annotation { "Name" : "Diameter refers to" }
            definition.diameterReference is KCDiameter;
            annotation { "Name" : "Density control" }
            definition.density is KCDensity;
            if (definition.density == KCDensity.COUNT)
            {
                annotation { "Name" : "Grooves around circumference (per direction)" }
                isInteger(definition.count, { (unitless) : [8, 32, 120] } as IntegerBoundSpec);
            }
            else
            {
                annotation { "Name" : "Requested circumferential pitch" }
                isLength(definition.pitch, { (millimeter) : [0.2, 2.5, 50] } as LengthBoundSpec);
            }
            annotation { "Name" : "Radial peak-to-valley depth" }
            isLength(definition.depth, { (millimeter) : [0.02, 0.45, 10] } as LengthBoundSpec);
            if (definition.style != KCStyle.STRAIGHT)
            {
                annotation { "Name" : "Helix angle from cylinder axis" }
                isAngle(definition.helixAngle, { (degree) : [5, 30, 65] } as AngleBoundSpec);
                if (definition.style == KCStyle.HELICAL)
                {
                    annotation { "Name" : "Clockwise viewed along +Z", "Default" : true }
                    definition.clockwise is boolean;
                }
            }
            annotation { "Name" : "Bottom unknurled margin" }
            isLength(definition.bottomMargin, { (millimeter) : [0, 2, 500] } as LengthBoundSpec);
            annotation { "Name" : "Top unknurled margin" }
            isLength(definition.topMargin, { (millimeter) : [0, 2, 500] } as LengthBoundSpec);
            annotation { "Name" : "Groove profile control" }
            definition.profile is KCProfile;
            if (definition.profile == KCProfile.ANGLE)
            {
                annotation { "Name" : "Included V angle" }
                isAngle(definition.vAngle, { (degree) : [30, 120, 150] } as AngleBoundSpec);
            }
            else
            {
                annotation { "Name" : "Nominal crest land ratio (0.1 = 10 percent)" }
                isReal(definition.landRatio, { (unitless) : [0.05, 0.2, 0.85] } as RealBoundSpec);
            }
            annotation { "Name" : "Advanced settings", "Default" : false }
            definition.advanced is boolean;
            if (definition.advanced)
            {
                annotation { "Name" : "Pattern rotation about Z" }
                isAngle(definition.phase, { (degree) : [0, 0, 360] } as AngleBoundSpec);
                annotation { "Name" : "Number of axial bands" }
                isInteger(definition.bands, { (unitless) : [1, 1, 6] } as IntegerBoundSpec);
                annotation { "Name" : "Gap between bands (ignored for one band)" }
                isLength(definition.gap, { (millimeter) : [0.1, 3, 100] } as LengthBoundSpec);
                annotation { "Name" : "Groove root flat width (0 = V point)" }
                isLength(definition.rootFlat, { (millimeter) : [0, 0, 5] } as LengthBoundSpec);
                annotation { "Name" : "Round crest edges", "Default" : false }
                definition.roundCrests is boolean;
                if (definition.roundCrests)
                {
                    annotation { "Name" : "Crest edge radius" }
                    isLength(definition.crestRadius, { (millimeter) : [0.005, 0.05, 2] } as LengthBoundSpec);
                }
                annotation { "Name" : "Round groove roots", "Default" : false }
                definition.roundRoots is boolean;
                if (definition.roundRoots)
                {
                    annotation { "Name" : "Root edge radius" }
                    isLength(definition.rootRadius, { (millimeter) : [0.005, 0.05, 2] } as LengthBoundSpec);
                }
            }
        }
    }
    {
        var active = definition.style != KCStyle.NONE;
        var radius = definition.diameter / 2;
        var envelope = radius;
        var bodyRadius = radius;
        if (active)
        {
            var liftFraction = 0;
            if (definition.mode == KCMode.RAISED)
                liftFraction = 1;
            else if (definition.mode == KCMode.BALANCED)
                liftFraction = 0.5;
            if (definition.diameterReference == KCDiameter.BODY)
                envelope = radius + liftFraction * definition.depth;
            else
                bodyRadius = radius - liftFraction * definition.depth;
            if (envelope - definition.depth <= 0 * millimeter || bodyRadius <= 0 * millimeter)
                throw regenError("Depth is too large for the selected diameter.", ["depth", "diameter"]);
            if (definition.depth > envelope * 0.15)
                throw regenError("Use a knurl depth below 15 percent of the outside radius.", ["depth"]);
        }

        // Build the blank with an explicit extrude so cap queries are stable.
        var baseSketch = id + "baseSketch";
        var sk = newSketchOnPlane(context, baseSketch, {
            "sketchPlane" : plane(vector(0, 0, 0) * millimeter, vector(0, 0, 1))
        });
        skCircle(sk, "blank", { "center" : vector(0, 0) * millimeter, "radius" : bodyRadius });
        skSolve(sk);
        opExtrude(context, id + "blank", {
            "entities" : qSketchRegion(baseSketch), "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND, "endDepth" : definition.length
        });
        opDeleteBodies(context, id + "deleteBaseSketch", { "entities" : qCreatedBy(baseSketch, EntityType.BODY) });
        var body = qCreatedBy(id + "blank", EntityType.BODY);

        var bottomStyle = definition.bottomEdge;
        var bottomSize = 0 * millimeter;
        var bottomAngle = 45 * degree;
        var bottomFlip = false;
        if (bottomStyle != KCEdge.SHARP)
            bottomSize = definition.bottomSize;
        if (bottomStyle == KCEdge.CHAMFER)
        {
            bottomAngle = definition.bottomAngle;
            bottomFlip = definition.bottomFlip;
        }
        var topStyle = bottomStyle;
        var topSize = bottomSize;
        var topAngle = bottomAngle;
        var topFlip = bottomFlip;
        if (!definition.sameEnds)
        {
            topStyle = definition.topEdge;
            topSize = 0 * millimeter;
            topAngle = 45 * degree;
            topFlip = false;
            if (topStyle != KCEdge.SHARP)
                topSize = definition.topSize;
            if (topStyle == KCEdge.CHAMFER)
            {
                topAngle = definition.topAngle;
                topFlip = definition.topFlip;
            }
        }
        var bottomClear = kcAllowance(bottomStyle, bottomSize, bottomAngle);
        var topClear = kcAllowance(topStyle, topSize, topAngle);
        if (bottomClear + topClear >= definition.length || max(bottomClear, topClear) >= bodyRadius)
            throw regenError("End treatment is too large for the cylinder.");
        if (active)
        {
            // Keep knurling out of end fillets/chamfers automatically.
            var z0 = max(definition.bottomMargin, bottomClear + 0.01 * millimeter);
            var z1 = definition.length - max(definition.topMargin, topClear + 0.01 * millimeter);
            var bands = 1;
            var gap = 0 * millimeter;
            var phase = 0 * degree;
            var rootFlat = 0 * millimeter;
            if (definition.advanced)
            {
                bands = definition.bands;
                phase = definition.phase;
                rootFlat = definition.rootFlat;
                if (bands > 1)
                    gap = definition.gap;
            }
            var bandLength = (z1 - z0 - (bands - 1) * gap) / bands;
            if (bandLength <= 0.1 * millimeter)
                throw regenError("Margins and band gaps leave no usable knurl length.");
            var beta = 0 * degree;
            if (definition.style != KCStyle.STRAIGHT)
                beta = definition.helixAngle;
            var circumference = 2 * PI * envelope;
            var n = 32;
            if (definition.density == KCDensity.COUNT)
                n = definition.count;
            else
                n = round(circumference / definition.pitch);
            if (n < 8 || n > 120)
                throw regenError("Pitch must give 8 to 120 grooves per direction.", ["pitch"]);
            var families = 1;
            if (definition.style == KCStyle.DIAMOND)
                families = 2;
            if (n * families * bands > 384)
                throw regenError("More than 384 groove instances. Reduce count or bands.");
            var actualPitch = circumference / n;
            var normalPitch = actualPitch * cos(beta);
            var halfTangent = 1;
            if (definition.profile == KCProfile.ANGLE)
                halfTangent = tan(definition.vAngle / 2);
            else
                halfTangent = (normalPitch * (1 - definition.landRatio) - rootFlat) / (2 * definition.depth);
            var mouth = rootFlat + 2 * definition.depth * halfTangent;
            if (halfTangent <= 0 || mouth >= normalPitch * 0.98)
                throw regenError("Grooves overlap. Reduce depth, V angle, root flat, or groove count.");

            var cutEdges = [];
            for (var b = 0; b < bands; b += 1)
            {
                var bid = id + ("band" ~ b);
                var bandStart = z0 + b * (bandLength + gap);
                var bandEnd = bandStart + bandLength;
                if (envelope > bodyRadius)
                {
                    kcCylinder(context, bid + "raisedBlank", envelope, bandStart, bandEnd);
                    opBoolean(context, bid + "join", {
                        "tools" : qUnion([body, qCreatedBy(bid + "raisedBlank", EntityType.BODY)]),
                        "operationType" : BooleanOperationType.UNION
                    });
                }
                for (var f = 0; f < families; f += 1)
                {
                    var fid = bid + ("family" ~ f);
                    var clockwise = (f == 0);
                    if (definition.style == KCStyle.HELICAL)
                        clockwise = definition.clockwise;
                    var cutters = [];
                    if (beta == 0 * degree)
                    {
                        // Straight mode retains the previously working solid pattern.
                        var seed = kcSeed(context, fid + "tool", envelope, bandStart, bandEnd,
                                          definition.depth, halfTangent, rootFlat, beta, clockwise, phase);
                        var transforms = [];
                        var names = [];
                        for (var k = 1; k < n; k += 1)
                        {
                            transforms = append(transforms, rotationAround(
                                line(vector(0, 0, 0) * millimeter, vector(0, 0, 1)), k * 360 * degree / n));
                            names = append(names, "groove" ~ k);
                        }
                        opPattern(context, fid + "pattern", {
                            "entities" : seed, "transforms" : transforms, "instanceNames" : names
                        });
                        cutters = [seed, qCreatedBy(fid + "pattern", EntityType.BODY)];
                    }
                    else
                    {
                        // Every groove has its OWN helix, normal plane, sketch and sweep.
                        for (var k = 0; k < n; k += 1)
                        {
                            var grooveId = fid + ("groove" ~ k);
                            cutters = append(cutters, kcSeed(context, grooveId, envelope, bandStart, bandEnd,
                                definition.depth, halfTangent, rootFlat, beta, clockwise,
                                phase + k * 360 * degree / n));
                        }
                    }
                    opBoolean(context, fid + "cut", {
                        "targets" : body,
                        "tools" : qUnion(cutters),
                        "operationType" : BooleanOperationType.SUBTRACTION,
                        "keepTools" : false
                    });
                    cutEdges = append(cutEdges, qCreatedBy(fid + "cut", EntityType.EDGE));
                    cutEdges = append(cutEdges, qAdjacent(qCreatedBy(fid + "cut", EntityType.FACE), AdjacencyType.EDGE, EntityType.EDGE));
                }
            }

            if (definition.advanced && (definition.roundCrests || definition.roundRoots))
            {
                // Ignore planar band-boundary edges; round longitudinal edges.
                var longitudinal = [];
                for (var edge in evaluateQuery(context, qUnion(cutEdges)))
                {
                    var edgeBounds = evBox3d(context, { "topology" : edge, "tight" : true });
                    if (edgeBounds.maxCorner[2] - edgeBounds.minCorner[2] > 0.001 * millimeter)
                        longitudinal = append(longitudinal, edge);
                }
                if (definition.roundCrests)
                {
                    var crests = kcConvexityFilter(context, qUnion(longitudinal), EdgeConvexityType.CONVEX);
                    if (size(evaluateQuery(context, crests)) == 0)
                        throw regenError("No convex crest edges found to round.");
                    opFillet(context, id + "crestRounding", {
                        "entities" : crests, "radius" : definition.crestRadius,
                        "tangentPropagation" : false
                    });
                }
                if (definition.roundRoots)
                {
                    var roots = kcConvexityFilter(context, qUnion(longitudinal), EdgeConvexityType.CONCAVE);
                    if (size(evaluateQuery(context, roots)) == 0)
                        throw regenError("No concave root edges found to round.");
                    opFillet(context, id + "rootRounding", {
                        "entities" : roots, "radius" : definition.rootRadius,
                        "tangentPropagation" : false
                    });
                }
            }
            reportFeatureInfo(context, id, "Grooves per direction: " ~ n ~
                "; actual circumferential pitch: " ~ actualPitch ~
                "; actual band length: " ~ bandLength ~
                "; maximum knurl diameter: " ~ (2 * envelope));
        }
        // End treatment is deliberately applied after all knurl geometry.
        // Protected axial margins keep the original circular cap edges intact.
        kcEnd(context, id + "bottom", qCapEntity(id + "blank", CapType.START, EntityType.EDGE),
              bottomStyle, bottomSize, bottomAngle, bottomFlip);
        kcEnd(context, id + "top", qCapEntity(id + "blank", CapType.END, EntityType.EDGE),
              topStyle, topSize, topAngle, topFlip);
        if (size(evaluateQuery(context, body)) != 1)
            throw regenError("Expected one connected solid. Reduce depth or rounding.");
        setProperty(context, { "entities" : body, "propertyType" : PropertyType.NAME,
                               "value" : "Knurl Cylinder v1.4" });
    });
