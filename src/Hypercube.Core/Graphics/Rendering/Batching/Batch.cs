using System.Runtime.InteropServices;
using Hypercube.Core.Graphics.Rendering.Shaders;
using Hypercube.Mathematics.Matrices;
using WindowHandle = Hypercube.Core.Windowing.Windows.WindowHandle;

namespace Hypercube.Core.Graphics.Rendering.Batching;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Batch : IEquatable<Batch>
{
    public readonly int Start;
    public readonly int Size;
    public readonly uint? TextureHandle;
    public readonly IShaderProgram Shader;
    public readonly PrimitiveTopology PrimitiveTopology;
    public readonly Matrix4x4 Model;
    public readonly WindowHandle Window;
    public readonly RenderStateId RenderStateId;

    public Batch(int start, int size, uint? textureHandle, IShaderProgram shader, PrimitiveTopology primitiveTopology, Matrix4x4 model, WindowHandle window, RenderStateId renderStateId)
    {
        Start = start;
        Size = size;
        TextureHandle = textureHandle;
        Shader = shader;
        PrimitiveTopology = primitiveTopology;
        Model = model;
        Window = window;
        RenderStateId = renderStateId;
    }

    public override bool Equals(object? obj)
    {
        return obj is Batch other && Equals(other);
    }

    public bool Equals(Batch other)
    {
        return Start == other.Start &&
               Size == other.Size &&
               TextureHandle == other.TextureHandle &&
               PrimitiveTopology == other.PrimitiveTopology &&
               Model == other.Model &&
               Window == other.Window &&
               RenderStateId == other.RenderStateId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, Size, TextureHandle, (int) PrimitiveTopology, Model, Window, RenderStateId);
    }

    public static bool operator ==(Batch left, Batch right) => left.Equals(right);

    public static bool operator !=(Batch left, Batch right) => !(left == right);
}