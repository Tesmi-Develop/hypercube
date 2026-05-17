using Hypercube.Mathematics;
using Hypercube.Physics.Mathematics;
using Hypercube.Physics.Utilities;
using Hypercube.Utilities.Collections;

namespace Hypercube.Physics.Shapes.Structs;

public struct ShapePolygon
{
    public FixedArray8<Vector2> Vertices;
    public FixedArray8<Vector2> Normals;
    public Vector2 Centroid;
    public float Radius;
    public int Count;

    // Ignore centroid
    public static ShapePolygon operator *(in ShapePolygon polygon, in Transform transform) =>
        transform.TransformPolygon(polygon);

    public static ShapePolygon CreateSquare(float halfSize) =>
        CreateRectangle(halfSize, halfSize);

    public static ShapePolygon CreateRectangle(Vector2 halfSize) =>
        CreateRectangle(halfSize.X, halfSize.Y);

    public static ShapePolygon CreateRectangle(float halfWidth, float halfHeight)
    {
        DebugPhysicsGuard.ValidatePositiveFloat(halfWidth);
        DebugPhysicsGuard.ValidatePositiveFloat(halfHeight);

        var shape = new ShapePolygon
        {
            Count = 4,
            Radius = 0,
            Centroid = Vector2.Zero,
        };
        
        // 3 --- 2
        // |     |
        // 0 --- 1
        
        shape.Vertices[0] = new Vector2(-halfWidth, -halfHeight); // bottom-left
        shape.Vertices[1] = new Vector2( halfWidth, -halfHeight); // bottom-right
        shape.Vertices[2] = new Vector2( halfWidth,  halfHeight); // top-right
        shape.Vertices[3] = new Vector2(-halfWidth,  halfHeight); // top-left
        
        shape.Normals[0] = new Vector2( 0f, -1f);
        shape.Normals[1] = new Vector2( 1f,  0f);
        shape.Normals[2] = new Vector2( 0f,  1f);
        shape.Normals[3] = new Vector2(-1f,  0f);

        return shape;
    }

    public static ShapePolygon CreateRectangleRounded(float halfWidth, float halfHeight, float radius)
    {
        DebugPhysicsGuard.ValidatePositiveFloat(radius);
        
        var shape = CreateRectangle(halfWidth, halfHeight);
        shape.Radius = radius;
        
        return shape;
    }
    
    public static ShapePolygon CreateEquilateralTriangleFromSide(float side)
    {
        DebugPhysicsGuard.ValidatePositiveFloat(side);
        return CreateEquilateralTriangle(side / float.Sqrt(3f));
    }
    
    public static ShapePolygon CreateEquilateralTriangle(float circumradius)
    {
        DebugPhysicsGuard.ValidatePositiveFloat(circumradius);

        var shape = new ShapePolygon
        {
            Count = 3,
            Radius = 0,
            Centroid = Vector2.Zero
        };
                
        const float a0 = -HyperMath.PiOver2F;
        const float step = HyperMath.Pi2Over3F;
        const float angleOffset = -HyperMath.PiOver2F;
        
        shape.Vertices[0] = new Vector2(
            float.Cos(angleOffset),
            float.Sin(angleOffset)
        ) * circumradius;

        shape.Vertices[1] = new Vector2(
            float.Cos(angleOffset + HyperMath.Pi2Over3F),
            float.Sin(angleOffset + HyperMath.Pi2Over3F)
        ) * circumradius;

        shape.Vertices[2] = new Vector2(
            float.Cos(angleOffset + HyperMath.PiOver3F),
            float.Sin(angleOffset + HyperMath.PiOver3F)
        ) * circumradius;
        
        shape.Vertices[0] = new Vector2(float.Cos(a0), float.Sin(a0)) * circumradius;
        shape.Vertices[1] = new Vector2(float.Cos(a0 + step), float.Sin(a0 + step)) * circumradius;
        shape.Vertices[2] = new Vector2(float.Cos(a0 + 2f * step), float.Sin(a0 + 2f * step)) * circumradius;

        return shape;
    }
    
    public static ShapePolygon ToLocalSpace(in ShapePolygon polygon, Vector2 origin)
    {
        var localCount = polygon.Count;
        var localPolygon = new ShapePolygon
        {
            Count = localCount,
            Radius = polygon.Radius,
            Centroid = Vector2.Zero
        };

        for (var i = 0; i < localCount; i++)
        {
            localPolygon.Vertices[i] = polygon.Vertices[i] - origin;
            localPolygon.Normals[i] = polygon.Normals[i];
        }

        return localPolygon;
    }
}