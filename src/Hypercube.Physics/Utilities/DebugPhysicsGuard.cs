using System.Diagnostics;
using Hypercube.Physics.Shapes;
using Hypercube.Physics.Shapes.Structs;

namespace Hypercube.Physics.Utilities;

public static class DebugPhysicsGuard
{
    [Conditional("DEBUG")]
    public static void ValidateShapeType(ShapeType type)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((int) type, Constants.ShapeTypeCount);
        ArgumentOutOfRangeException.ThrowIfLessThan((int) type, 0);
    }
    
    [Conditional("DEBUG")]
    public static void ValidatePolygonIndex(in ShapePolygon polygon, int value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, polygon.Count);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
    }
    
    [Conditional("DEBUG")]
    public static void ValidatePositiveFloat(float value)
    {
        if (float.IsNaN(value))
            throw new ArgumentException("Float value cannot be NaN.");
        
        if (float.IsInfinity(value))
            throw new ArgumentException("Float value cannot be infinite.");
        
        if (value <= 0)
            throw new ArgumentException("Float value cannot be zero or negative.");
    }
}