using Hypercube.Mathematics;
using Hypercube.Physics.Shapes.Structs;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial interface IRenderContext
{
    public void DrawShapePolygonLine(in ShapePolygon polygon, Color color);
    public void DrawShapePolygonLine(in ShapePolygon polygon, Vector2 position, Color color);
    public void DrawShapePolygonLine(in ShapePolygon polygon, Vector2 position, Angle angle, Color color);
    public void DrawShapePolygonLine(in ShapePolygon polygon, Physics.Mathematics.Transform transform, Color color);
}
