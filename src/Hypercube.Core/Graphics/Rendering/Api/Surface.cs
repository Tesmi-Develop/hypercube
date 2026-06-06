using System.Diagnostics.CodeAnalysis;
using Hypercube.Core.Graphics.Objects.Texturing;
using Hypercube.Mathematics;

namespace Hypercube.Core.Graphics.Rendering.Api;

public struct Surface : IEquatable<Surface>
{
    public readonly uint Fbo;
    public readonly TextureHandle FboTextureHandle;
    public readonly Vector2i Size;
    public Color Color = Mathematics.Color.Black;

    public Surface(uint fbo, TextureHandle fboTextureHandle, Vector2i size)
    {
        Fbo = fbo;
        FboTextureHandle = fboTextureHandle;
        Size = size;
    }

    public bool Equals(Surface other)
    {
        return Fbo == other.Fbo && FboTextureHandle.Equals(other.FboTextureHandle) && Size.Equals(other.Size);
    }

    public override bool Equals(object? obj)
    {
        return obj is Surface other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Fbo, FboTextureHandle, Size);
    }
}