using Hypercube.Physics.Mathematics;

namespace Hypercube.Physics.Shapes.Structs;

public struct ShapeCircle
{
    public Vector2 Center;
    public float Radius;
    
    public static ShapeCircle operator *(in ShapeCircle circle, in Transform transform) =>
        transform.TransformCircle(circle);
}
