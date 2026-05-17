using System.Runtime.InteropServices;

namespace Hypercube.Physics.Shapes;

[StructLayout(LayoutKind.Explicit)]
public struct ShapeUnionTyped
{
    [FieldOffset(0)] public ShapeType Type;
    [FieldOffset(1)] public ShapeUnion Shape;
}
