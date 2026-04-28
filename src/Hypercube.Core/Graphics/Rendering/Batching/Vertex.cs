using System.Runtime.InteropServices;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Rendering.Batching;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Vertex
{
    public const int Size =
        3 * sizeof(float) + // position
        4 * sizeof(float) + // color
        2 * sizeof(float) + // uv
        3 * sizeof(float) + // normal
        1 * sizeof(int);    // model index
        
    public readonly Vector3 Position;
    public readonly Vector4 Color;
    public readonly Vector2 UVCoords;
    public readonly Vector3 Normal;
    public readonly int ModelIndex;

    public Vertex(Vector3 position, Vector2 uvCoords, Color color, Vector3 normal, int modelIndex = 0)
    {
        Position = position;
        UVCoords = uvCoords;
        Color = color.Vec4;
        Normal = normal;
        ModelIndex = modelIndex;
    }
    
    public Vertex(Vector2 position, Vector2 uvCoords, Color color, int modelIndex = 0)
    {
        Position = new Vector3(position);
        UVCoords = uvCoords;
        Color = color.Vec4;
        Normal = Vector3.Zero;
        ModelIndex = modelIndex;
    }
}
