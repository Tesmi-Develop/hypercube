using System.Runtime.InteropServices;
using Hypercube.Physics.Shapes.Structs;

namespace Hypercube.Physics.Shapes;

[StructLayout(LayoutKind.Explicit)]
public struct ShapeUnion
{
    [FieldOffset(0)] public ShapeCircle Circle;
    [FieldOffset(0)] public ShapeCapsule Capsule;
    [FieldOffset(0)] public ShapePolygon Polygon;
    [FieldOffset(0)] public ShapeSegment Segment;
}
