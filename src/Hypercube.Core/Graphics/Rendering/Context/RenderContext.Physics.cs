using Hypercube.Mathematics;
using Hypercube.Mathematics.Vectors;
using Hypercube.Physics.Shapes.Structs;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial class RenderContext
{
    public void DrawShapePolygonLine(in ShapePolygon polygon, Color color)
    {
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        var vertices = polygon.Vertices.AsSpan();
        
        var lastIndex = polygon.Count - 1;
        for (var i = 0; i < lastIndex; ++i)
        {
            DrawLine(vertices[i], vertices[i + 1], color);
        }
        
        DrawLine(vertices[lastIndex], vertices[0], color);
    }
    
    public void DrawShapePolygonLine(in ShapePolygon polygon, Vector2 position, Color color) =>
        DrawShapePolygonLine(polygon, new Physics.Mathematics.Transform(position, Vector2Angle.Zero), color);
    
    public void DrawShapePolygonLine(in ShapePolygon polygon, Vector2 position, Angle angle, Color color) =>
        DrawShapePolygonLine(polygon, new Physics.Mathematics.Transform(position, angle.VectorAngle), color);
    
    public void DrawShapePolygonLine(in ShapePolygon polygon, Physics.Mathematics.Transform transform, Color color)
    {
        var shape = polygon * transform;
        var vertices = shape.Vertices.AsSpan();

        var lastIndex = shape.Count - 1;
        for (var i = 0; i < lastIndex; ++i)
        {
            DrawLine(vertices[i], vertices[i + 1], color);
        }
        
        DrawLine(vertices[lastIndex], vertices[0], color);
    }
}