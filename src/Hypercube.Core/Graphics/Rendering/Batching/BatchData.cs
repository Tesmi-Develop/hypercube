using System.Runtime.InteropServices;
using Hypercube.Core.Graphics.Rendering.Shaders;

namespace Hypercube.Core.Graphics.Rendering.Batching;

[StructLayout(LayoutKind.Sequential)]
public readonly struct BatchData : IEquatable<BatchData>
{
    public readonly int Start;
    public readonly uint? Texture;
    public readonly IShaderProgram Shader;
    public readonly PrimitiveTopology PrimitiveTopology;
    public readonly RenderStateId RenderStateId;

    public BatchData(int start, uint? texture, IShaderProgram shader, PrimitiveTopology primitiveTopology, RenderStateId renderStateId)
    {
        Start = start;
        Texture = texture;
        Shader = shader;
        PrimitiveTopology = primitiveTopology;
        RenderStateId = renderStateId;
    }
    
    public bool Equals(PrimitiveTopology topology, uint? texture, IShaderProgram shader, RenderStateId renderStateId)
    {
        return Texture == texture &&
               Shader.Handle == shader.Handle &&
               PrimitiveTopology == topology &&
               RenderStateId == renderStateId;
    }
    
    public bool Equals(BatchData other)
    {
        return Start == other.Start &&
               Texture == other.Texture &&
               Shader.Handle == other.Shader.Handle &&
               PrimitiveTopology == other.PrimitiveTopology &&
               RenderStateId == other.RenderStateId;
    }

    public override bool Equals(object? obj)
    {
        return obj is BatchData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, Texture, Shader, (int)PrimitiveTopology, RenderStateId);
    }
    
    public static bool operator ==(BatchData a, BatchData b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(BatchData a, BatchData b)
    {
        return !a.Equals(b);
    }
}