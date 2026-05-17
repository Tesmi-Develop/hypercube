using Hypercube.Physics.Manifolds;
using Hypercube.Physics.Mathematics;
using Hypercube.Physics.Shapes;

namespace Hypercube.Physics.Collision;

public static partial class Contacts
{
    private static Manifold CollideCircles(in ShapeUnion shapeA, in Transform transformA, in ShapeUnion shapeB, in Transform transformB)
    {
        var localA = shapeA.Circle;
        var localB = shapeB.Circle;
        return Collide.Circles(ref localA, transformA, ref localB, transformB);
    }

    private static Manifold CollidePolygons(in ShapeUnion shapeA, in Transform transformA, in ShapeUnion shapeB, in Transform transformB)
    {
        var localA = shapeA.Polygon;
        var localB = shapeB.Polygon;
        return Collide.Polygons(ref localA, transformA, ref localB, transformB);
    }

    private static Manifold CollidePolygonCircle(in ShapeUnion shapeA, in Transform transformA, in ShapeUnion shapeB, in Transform transformB)
    {
        var localA = shapeA.Polygon;
        var localB = shapeB.Circle;
        return Collide.PolygonCircle(ref localA, transformA, ref localB, transformB);  
    }
}
