namespace Hypercube.Physics.Manifolds;

public readonly struct ManifoldPoint
{
    public Vector2 Point { get; init; }
    public Vector2 AnchorA { get; init; }
    public Vector2 AnchorB { get; init; }
    
    public float Separation { get; init; }
    public float NormalImpulse { get; init; }
    public float TangentImpulse { get; init; }
    public float TotalNormalImpulse { get; init; }
    public float NormalVelocity { get; init; }
    
    public ushort Id { get; init; }
    
    public bool Persisted { get; init; }

    public ManifoldPoint(Vector2 point, Vector2 anchorA, Vector2 anchorB, float separation, ushort id)
    {
        Point = point;
        AnchorA = anchorA;
        AnchorB = anchorB;
        
        Separation = separation;
        NormalImpulse = 0;
        TangentImpulse = 0;
        TotalNormalImpulse = 0;
        NormalVelocity = 0;
        
        Id = id;
        
        Persisted = false;
    }

    public static ushort MakeId(int a, int b) => (ushort) ((byte) a << 8 | (byte) b);
}
