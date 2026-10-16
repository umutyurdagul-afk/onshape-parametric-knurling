FeatureScript 2716;
import(path : "onshape/std/geometry.fs", version : "2716.0");

// KNURL PRISM - INSET v1.0
// New independent feature; cylinder variants are unchanged.
// Makes a centered square/rectangular prism along +Z. All four lateral
// faces are cut. Top/bottom caps have no independent knurl pattern.
// Base -> corner/end treatments -> cutters clipped to inset windows
// -> circular body patterns -> subtraction -> optional tooth rounding.
// Square: 90/180/270 degree copies; rectangle: two 180-degree face pairs.
// Density is measured around the unrounded developed envelope perimeter.
// Count is rounded to the nearest multiple of four to preserve pattern symmetry.
// Knurl windows stop after the corner/end treatment allowances plus
// user margins. Fillets, chamfers and border strips remain untextured.
// All margins are measured from the conservative treatment allowance.
// Separate feature: the original full-surface file is unchanged.
// API/parameter math checked locally. Not compiled or regenerated in Onshape.

export enum KCStyle
{
    annotation { "Name" : "Diamond / Baklava" } DIAMOND,
    annotation { "Name" : "Straight / Duz" } STRAIGHT,
    annotation { "Name" : "Diagonal / Capraz" } DIAGONAL,
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
export enum KCSizeReference
{
    annotation { "Name" : "Nominal body width / depth" } BODY,
    annotation { "Name" : "Maximum outside width / depth" } OUTSIDE
}
export enum KCProfile
{
    annotation { "Name" : "V angle + depth" } ANGLE,
    annotation { "Name" : "Nominal crest land ratio + depth" } LAND
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


// One planar face is normal to faceNormal; faceTangent advances around the
// developed perimeter. Parallel straight grooves are extruded together.
// Extra cutter height never intersects the opposite face: depth is bounded.
function rkFaceTools(context is Context, id is Id, faceNormal is Vector,
                     faceTangent is Vector, faceOffset is ValueWithUnits,
                     faceWidth is ValueWithUnits, height is ValueWithUnits,
                     depth is ValueWithUnits, halfTangent is number,
                     rootFlat is ValueWithUnits, beta is ValueWithUnits,
                     pitch is ValueWithUnits, phaseLength is ValueWithUnits,
                     developedStart is ValueWithUnits, extra is ValueWithUnits) returns Query
{
    var zAxis = vector(0, 0, 1);
    var direction = sin(beta) * faceTangent + cos(beta) * zAxis;
    var normalPitch = pitch * cos(beta);
    var over = max(0.2 * millimeter, depth);
    var naturalHalfWidth = rootFlat / 2 + (depth + over) * halfTangent;
    var halfWidthLimit = normalPitch * 0.49;
    var qSpan = (faceWidth * cos(beta) + height * abs(sin(beta))) / 2;
    var runHalf = (height * cos(beta) + faceWidth * abs(sin(beta))) / 2 +
                  naturalHalfWidth + depth + extra + 1 * millimeter;
    var origin = faceOffset * faceNormal + height / 2 * zAxis - runHalf * direction;
    var profilePlane = plane(origin, direction, faceNormal);
    var shift = (phaseLength - developedStart - faceWidth / 2) * cos(beta) +
                height / 2 * sin(beta);
    var first = floor((-qSpan - shift) / normalPitch) - 1;
    var last = ceil((qSpan - shift) / normalPitch) + 1;
    if (last - first + 1 > 160)
        throw regenError("More than 160 cutters on one face. Increase pitch or reduce height/angle.");

    var sid = id + "profile";
    var sk = newSketchOnPlane(context, sid, { "sketchPlane" : profilePlane });
    for (var k = first; k <= last; k += 1)
    {
        var q = shift + k * normalPitch;
        var points = [vector(-depth, q - rootFlat / 2)];
        // Limit cutter shoulders only OUTSIDE the uncut face. This prevents
        // adjacent profiles from intersecting in the shared sketch.
        if (naturalHalfWidth > halfWidthLimit)
        {
            var shoulder = (halfWidthLimit - rootFlat / 2) / halfTangent - depth;
            points = append(points, vector(shoulder, q - halfWidthLimit));
            points = append(points, vector(over, q - halfWidthLimit));
            points = append(points, vector(over, q + halfWidthLimit));
            points = append(points, vector(shoulder, q + halfWidthLimit));
        }
        else
        {
            points = append(points, vector(over, q - naturalHalfWidth));
            points = append(points, vector(over, q + naturalHalfWidth));
        }
        if (rootFlat > 0 * millimeter)
            points = append(points, vector(-depth, q + rootFlat / 2));
        points = append(points, points[0]);
        skPolyline(sk, "groove" ~ (k - first), { "points" : points });
    }
    skSolve(sk);
    opExtrude(context, id + "tools", {
        "entities" : qSketchRegion(sid), "direction" : direction,
        "endBound" : BoundingType.BLIND, "endDepth" : 2 * runHalf
    });
    opDeleteBodies(context, id + "cleanup", { "entities" : qCreatedBy(sid, EntityType.BODY) });
    return qCreatedBy(id + "tools", EntityType.BODY);
}

// Keep only cutter portions inside one face's rectangular knurl window.
// SUBTRACT_COMPLEMENT trims EVERY target to the union of the tools;
// unlike INTERSECTION, separate groove bodies are not intersected together.
function rkInsetTools(context is Context, id is Id, cutters is Query,
                      faceNormal is Vector, faceTangent is Vector,
                      faceOffset is ValueWithUnits, usableWidth is ValueWithUnits,
                      zStart is ValueWithUnits, zEnd is ValueWithUnits,
                      depth is ValueWithUnits) returns Query
{
    var radialInner = faceOffset - depth - 0.01 * millimeter;
    var radialOuter = faceOffset + max(0.2 * millimeter, depth) + 0.01 * millimeter;
    var p0 = radialInner * faceNormal - usableWidth / 2 * faceTangent;
    var p1 = radialOuter * faceNormal - usableWidth / 2 * faceTangent;
    var p2 = radialOuter * faceNormal + usableWidth / 2 * faceTangent;
    var p3 = radialInner * faceNormal + usableWidth / 2 * faceTangent;
    var sid = id + "windowSketch";
    var sk = newSketchOnPlane(context, sid, {
        "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, zStart), vector(0, 0, 1), vector(1, 0, 0))
    });
    skPolyline(sk, "window", { "points" : [
        vector(p0[0], p0[1]), vector(p1[0], p1[1]),
        vector(p2[0], p2[1]), vector(p3[0], p3[1]), vector(p0[0], p0[1])
    ] });
    skSolve(sk);
    opExtrude(context, id + "window", {
        "entities" : qSketchRegion(sid), "direction" : vector(0, 0, 1),
        "endBound" : BoundingType.BLIND, "endDepth" : zEnd - zStart
    });
    opDeleteBodies(context, id + "deleteSketch", { "entities" : qCreatedBy(sid, EntityType.BODY) });
    opBoolean(context, id + "trim", {
        "targets" : cutters, "tools" : qCreatedBy(id + "window", EntityType.BODY),
        "operationType" : BooleanOperationType.SUBTRACT_COMPLEMENT, "keepTools" : false
    });
    if (size(evaluateQuery(context, cutters)) == 0)
        throw regenError("No grooves cross the inset window. Reduce margins or pitch, or change phase.");
    return cutters;
}

annotation { "Feature Type Name" : "Knurl Prism - Inset v1.0" }
export const knurlPrismInset = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Square cross-section", "Default" : true }
        definition.square is boolean;
        annotation { "Name" : "Width X / square side" }
        isLength(definition.width, { (millimeter) : [2, 30, 500] } as LengthBoundSpec);
        if (!definition.square)
        {
            annotation { "Name" : "Depth Y" }
            isLength(definition.breadth, { (millimeter) : [2, 20, 500] } as LengthBoundSpec);
        }
        annotation { "Name" : "Prism height Z" }
        isLength(definition.length, { (millimeter) : [2, 40, 1000] } as LengthBoundSpec);
        annotation { "Name" : "Four vertical corners" }
        definition.corners is KCEdge;
        if (definition.corners != KCEdge.SHARP)
        {
            annotation { "Name" : "Corner fillet radius / equal chamfer offset" }
            isLength(definition.cornerSize, { (millimeter) : [0.01, 0.8, 100] } as LengthBoundSpec);
        }
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
            annotation { "Name" : "Bottom border after fillet / chamfer" }
            isLength(definition.bottomMargin, { (millimeter) : [0, 2, 200] } as LengthBoundSpec);
            annotation { "Name" : "Top border after fillet / chamfer" }
            isLength(definition.topMargin, { (millimeter) : [0, 2, 200] } as LengthBoundSpec);
            annotation { "Name" : "Side border after corner fillet / chamfer" }
            isLength(definition.sideMargin, { (millimeter) : [0, 2, 200] } as LengthBoundSpec);
            annotation { "Name" : "Knurl mode" }
            definition.mode is KCMode;
            annotation { "Name" : "Width and depth refer to" }
            definition.sizeReference is KCSizeReference;
            annotation { "Name" : "Density control" }
            definition.density is KCDensity;
            if (definition.density == KCDensity.COUNT)
            {
                annotation { "Name" : "Groove count around developed perimeter (multiple of 4)" }
                isInteger(definition.count, { (unitless) : [8, 48, 240] } as IntegerBoundSpec);
            }
            else
            {
                annotation { "Name" : "Requested developed-perimeter pitch" }
                isLength(definition.pitch, { (millimeter) : [0.2, 2.5, 50] } as LengthBoundSpec);
            }
            annotation { "Name" : "Groove depth normal to flat face" }
            isLength(definition.depth, { (millimeter) : [0.02, 0.45, 10] } as LengthBoundSpec);
            if (definition.style != KCStyle.STRAIGHT)
            {
                annotation { "Name" : "Diagonal angle from prism Z axis" }
                isAngle(definition.slopeAngle, { (degree) : [5, 30, 65] } as AngleBoundSpec);
                if (definition.style == KCStyle.DIAGONAL)
                {
                    annotation { "Name" : "Positive diagonal direction around perimeter", "Default" : true }
                    definition.rightLean is boolean;
                }
            }
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
                annotation { "Name" : "Phase along developed perimeter (360 deg = one perimeter)" }
                isAngle(definition.phase, { (degree) : [0, 0, 360] } as AngleBoundSpec);
                annotation { "Name" : "Additional cutter overrun at EACH end" }
                isLength(definition.extraExtension, { (millimeter) : [0, 1, 50] } as LengthBoundSpec);
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
        var width = definition.width;
        var breadth = width;
        if (!definition.square)
            breadth = definition.breadth;
        var envelopeW = width;
        var envelopeD = breadth;
        if (active)
        {
            var lift = 0;
            if (definition.mode == KCMode.RAISED)
                lift = 1;
            else if (definition.mode == KCMode.BALANCED)
                lift = 0.5;
            if (definition.sizeReference == KCSizeReference.BODY)
            {
                envelopeW += 2 * lift * definition.depth;
                envelopeD += 2 * lift * definition.depth;
            }
            if (definition.depth > min(envelopeW, envelopeD) * 0.15)
                throw regenError("Depth must be below 15 percent of the smaller cross-section dimension.", ["depth"]);
        }
        var baseSketch = id + "baseSketch";
        var sk = newSketchOnPlane(context, baseSketch, {
            "sketchPlane" : plane(vector(0, 0, 0) * millimeter, vector(0, 0, 1))
        });
        skRectangle(sk, "rectangle", {
            "firstCorner" : vector(-envelopeW / 2, -envelopeD / 2),
            "secondCorner" : vector(envelopeW / 2, envelopeD / 2)
        });
        skSolve(sk);
        opExtrude(context, id + "blank", {
            "entities" : qSketchRegion(baseSketch), "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND, "endDepth" : definition.length
        });
        opDeleteBodies(context, id + "deleteBaseSketch", { "entities" : qCreatedBy(baseSketch, EntityType.BODY) });
        var body = qCreatedBy(id + "blank", EntityType.BODY);

        var cornerSize = 0 * millimeter;
        if (definition.corners != KCEdge.SHARP)
        {
            cornerSize = definition.cornerSize;
            if (2 * cornerSize >= min(envelopeW, envelopeD))
                throw regenError("Corner size must be smaller than half the cross-section.", ["cornerSize"]);
            var vertical = qGeometry(qNonCapEntity(id + "blank", EntityType.EDGE), GeometryType.LINE);
            kcEnd(context, id + "verticalCorners", vertical, definition.corners, cornerSize, 45 * degree, false);
        }
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
        if (bottomClear + topClear >= definition.length ||
            2 * max(bottomClear, topClear) >= min(envelopeW, envelopeD))
            throw regenError("End treatment is too large for the prism.");
        // Query complete cap boundaries, including arcs added by corner fillets.
        kcEnd(context, id + "bottom", qAdjacent(qCapEntity(id + "blank", CapType.START, EntityType.FACE),
              AdjacencyType.EDGE, EntityType.EDGE), bottomStyle, bottomSize, bottomAngle, bottomFlip);
        kcEnd(context, id + "top", qAdjacent(qCapEntity(id + "blank", CapType.END, EntityType.FACE),
              AdjacencyType.EDGE, EntityType.EDGE), topStyle, topSize, topAngle, topFlip);

        if (active)
        {
            var zStart = bottomClear + definition.bottomMargin;
            var zEnd = definition.length - topClear - definition.topMargin;
            var sideInset = cornerSize + definition.sideMargin;
            if (zEnd - zStart <= 0.01 * millimeter)
                throw regenError("Top/bottom borders leave no knurl height.", ["bottomMargin", "topMargin"]);
            if (min(envelopeW, envelopeD) - 2 * sideInset <= 0.01 * millimeter)
                throw regenError("Side borders leave no knurl width.", ["sideMargin", "cornerSize"]);
            var beta = 0 * degree;
            if (definition.style != KCStyle.STRAIGHT)
                beta = definition.slopeAngle;
            if (definition.style == KCStyle.DIAGONAL && !definition.rightLean)
                beta = -beta;
            var perimeter = 2 * (envelopeW + envelopeD);
            var n = 48;
            if (definition.density == KCDensity.COUNT)
                n = definition.count;
            else
                n = round(perimeter / definition.pitch);
            // Opposite-face rotations require an even half-perimeter repeat;
            // multiples of four also provide 90-degree symmetry on squares.
            n = 4 * round(n / 4);
            if (n < 8 || n > 240)
                throw regenError("Density must give 8 to 240 perimeter repeats.");
            var actualPitch = perimeter / n;
            var normalPitch = actualPitch * cos(beta);
            var rootFlat = 0 * millimeter;
            var phaseLength = 0 * millimeter;
            var extra = 1 * millimeter;
            if (definition.advanced)
            {
                rootFlat = definition.rootFlat;
                phaseLength = perimeter * (definition.phase / (360 * degree));
                extra = definition.extraExtension;
            }
            var halfTangent = 1;
            if (definition.profile == KCProfile.ANGLE)
                halfTangent = tan(definition.vAngle / 2);
            else
                halfTangent = (normalPitch * (1 - definition.landRatio) - rootFlat) / (2 * definition.depth);
            var mouth = rootFlat + 2 * definition.depth * halfTangent;
            if (halfTangent <= 0 || mouth >= normalPitch * 0.98)
                throw regenError("Grooves overlap. Reduce depth, count, root flat or V angle.");
            var square = abs(envelopeW - envelopeD) < 0.000001 * millimeter;
            var sourceFaces = 2;
            if (square)
                sourceFaces = 1;
            var families = 1;
            if (definition.style == KCStyle.DIAMOND)
                families = 2;
            var cutEdges = [];
            for (var side = 0; side < sourceFaces; side += 1)
            {
                var sid = id + ("side" ~ side);
                var faceNormal = vector(0, -1, 0);
                var faceTangent = vector(1, 0, 0);
                var faceOffset = envelopeD / 2;
                var faceWidth = envelopeW;
                var developedStart = 0 * millimeter;
                if (side == 1)
                {
                    faceNormal = vector(1, 0, 0);
                    faceTangent = vector(0, 1, 0);
                    faceOffset = envelopeW / 2;
                    faceWidth = envelopeD;
                    developedStart = envelopeW;
                }
                for (var f = 0; f < families; f += 1)
                {
                    var fid = sid + ("family" ~ f);
                    var localBeta = beta;
                    if (f == 1)
                        localBeta = -beta;
                    var seed = rkFaceTools(context, fid + "seed", faceNormal, faceTangent,
                        faceOffset, faceWidth, definition.length, definition.depth,
                        halfTangent, rootFlat, localBeta, actualPitch, phaseLength, developedStart, extra);
                    seed = rkInsetTools(context, fid + "inset", seed, faceNormal, faceTangent,
                        faceOffset, faceWidth - 2 * sideInset, zStart, zEnd, definition.depth);
                    var transforms = [];
                    var names = [];
                    if (square)
                    {
                        for (var k = 1; k <= 3; k += 1)
                        {
                            transforms = append(transforms, rotationAround(
                                line(vector(0, 0, 0) * millimeter, vector(0, 0, 1)), k * 90 * degree));
                            names = append(names, "face" ~ k);
                        }
                    }
                    else
                    {
                        transforms = [rotationAround(line(vector(0, 0, 0) * millimeter, vector(0, 0, 1)), 180 * degree)];
                        names = ["oppositeFace"];
                    }
                    opPattern(context, fid + "circular", { "entities" : seed,
                        "transforms" : transforms, "instanceNames" : names });
                    opBoolean(context, fid + "cut", { "targets" : body,
                        "tools" : qUnion([seed, qCreatedBy(fid + "circular", EntityType.BODY)]),
                        "operationType" : BooleanOperationType.SUBTRACTION, "keepTools" : false });
                    cutEdges = append(cutEdges, qCreatedBy(fid + "cut", EntityType.EDGE));
                    cutEdges = append(cutEdges, qAdjacent(qCreatedBy(fid + "cut", EntityType.FACE), AdjacencyType.EDGE, EntityType.EDGE));
                }
            }
            if (definition.advanced && (definition.roundCrests || definition.roundRoots))
            {
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
                        throw regenError("No crest edges found to round.");
                    opFillet(context, id + "crestRounding", { "entities" : crests,
                        "radius" : definition.crestRadius, "tangentPropagation" : false });
                }
                if (definition.roundRoots)
                {
                    var roots = kcConvexityFilter(context, qUnion(longitudinal), EdgeConvexityType.CONCAVE);
                    if (size(evaluateQuery(context, roots)) == 0)
                        throw regenError("No root edges found to round.");
                    opFillet(context, id + "rootRounding", { "entities" : roots,
                        "radius" : definition.rootRadius, "tangentPropagation" : false });
                }
            }
            reportFeatureInfo(context, id, "Developed-perimeter repeats: " ~ n ~
                "; actual pitch: " ~ actualPitch ~ "; outside width: " ~ envelopeW ~
                "; outside depth: " ~ envelopeD ~ "; knurl height: " ~ (zEnd - zStart));
        }
        if (size(evaluateQuery(context, body)) != 1)
            throw regenError("Expected one connected part. Reduce groove depth or edge radii.");
        setProperty(context, { "entities" : body, "propertyType" : PropertyType.NAME,
                               "value" : "Knurl Prism - Inset v1.0" });
    });
