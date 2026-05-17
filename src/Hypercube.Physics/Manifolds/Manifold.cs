using Hypercube.Utilities.Collections;
using JetBrains.Annotations;

namespace Hypercube.Physics.Manifolds;

[PublicAPI]
public readonly struct Manifold
{
    public static readonly Manifold Empty = new(Vector2.Zero, 0f, FixedArray2<ManifoldPoint>.Empty, 0);
    
    public Vector2 Normal { get; init; }
    public float RollingImpulse { get; init; }
    public FixedArray2<ManifoldPoint> Points { get; init; }
    public int PointCount { get; init; }

    public bool IsEmpty => PointCount == 0;
    
    public Manifold(Vector2 normal, ManifoldPoint point)
    {
        Normal = normal;
        RollingImpulse = 0f;
        Points = new FixedArray2<ManifoldPoint>(point);
        PointCount = 1;
    }
    
    public Manifold(Vector2 normal, ManifoldPoint pointA, ManifoldPoint pointB)
    {
        Normal = normal;
        RollingImpulse = 0f;
        Points = new FixedArray2<ManifoldPoint>(pointA, pointB);
        PointCount = 2;
    }
    
    public Manifold(Vector2 normal, float rollingImpulse, FixedArray2<ManifoldPoint> points, int pointCount)
    {
        Normal = normal;
        RollingImpulse = rollingImpulse;
        Points = points;
        PointCount = pointCount;
    }
}
