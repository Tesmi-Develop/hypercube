using System.Runtime.CompilerServices;
using Hypercube.Mathematics;
using Hypercube.Physics.Shapes.Structs;
using JetBrains.Annotations;

namespace Hypercube.Physics.Mathematics;

[PublicAPI]
public readonly struct Transform
{
    public readonly Vector2 Position;
    public readonly Vector2Angle Rotation;

    public Transform(Vector2 position)
    {
        Position = position;
        Rotation = Vector2Angle.Zero;
    }
    
    public Transform(Vector2 position, Angle rotation)
    {
        Position = position;
        Rotation = rotation.VectorAngle;
    }
    
    public Transform(Vector2 position, Vector2Angle rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Transform ToLocalSpaceOf(Transform transformB) => GetRelativeTransform(this, transformB);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TranslateVector(Vector2 vector) => TranslateVector(this, vector);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Rotate(Vector2 vector) => Rotate(this, vector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 InvRotate(Vector2 vector) => InvRotate(this, vector);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ShapeCircle TransformCircle(in ShapeCircle circle) => TransformCircle(this, circle);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ShapePolygon TransformPolygon(in ShapePolygon polygon) => TransformPolygon(this, polygon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 TransformPoint(Vector2 vector) => TransformPoint(this, vector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transform GetRelativeTransform(Transform transformA, Transform transformB)
    {
        var position = transformA.Rotation.InvRotate(transformB.Position - transformA.Position);
        var rotation = transformB.Rotation - transformA.Rotation;
        return new Transform(position, rotation);
    }
     
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 TranslateVector(Transform transform, Vector2 vector) => transform.Position + vector;

    /// <summary>
    /// Rotate vector CW.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Rotate(Transform transform, Vector2 vector) =>
        transform.Rotation.Rotate(vector);

    /// <summary>
    /// Rotate vector CCW.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 InvRotate(Transform transform, Vector2 vector) =>
        transform.Rotation.InvRotate(vector);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 TransformPoint(Transform transform, Vector2 vector)
    {
        var x = vector.X;
        var y = vector.Y;
        var cos = transform.Rotation.Cos;
        var sin = transform.Rotation.Sin;
        
        // result = Rotate(vector) + Position
        return new Vector2(cos * x - sin * y + transform.Position.X, sin * x + cos * y + transform.Position.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ShapeCircle TransformCircle(Transform transform, in ShapeCircle circle)
    {
        var result = new ShapeCircle
        {
            Center = transform.TransformPoint(circle.Center),
            Radius = circle.Radius
        };
        
        return result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ShapePolygon TransformPolygon(Transform transform, in ShapePolygon polygon)
    {
        var result = new ShapePolygon
        {
            Count = polygon.Count,
            Radius = polygon.Radius
        };
     
        var count = polygon.Count;
        for (var i = 0; i < count; i++)
        {
            result.Vertices[i] = transform.TransformPoint(polygon.Vertices[i]);
            result.Normals[i] = transform.Rotate(polygon.Normals[i]);
        }

        result.Centroid = transform.TranslateVector(polygon.Centroid);
        
        return result;
    }
}
