using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Graphics.Rendering.Shaders;
using Hypercube.Mathematics.Matrices;

namespace Hypercube.Core.Graphics.Rendering.Batching;

public readonly struct RenderState : IEquatable<RenderState>
{
    public static readonly RenderState Default = new(Matrix4x4.Identity, Matrix4x4.Identity, BlendMode.Alpha, null);
    
    public readonly Matrix4x4 View;
    public readonly Matrix4x4 Projection;
    public readonly BlendMode BlendMode;
    public readonly Surface? Surface;

    public RenderState(in Matrix4x4 view, in Matrix4x4 projection, BlendMode blendMode, Surface? surface)
    {
        View = view;
        Projection = projection;
        BlendMode = blendMode;
        Surface = surface;
    }

    public bool Equals(RenderState other)
        => View.Equals(other.View) && 
           Projection.Equals(other.Projection) &&
           BlendMode.Equals(other.BlendMode) &&
           Surface.Equals(other.Surface);

    public override bool Equals(object? obj)
        => obj is RenderState other &&
           Equals(other);

    public override int GetHashCode() => HashCode.Combine(View, Projection, BlendMode);

    public static bool operator ==(RenderState left, RenderState right) => left.Equals(right);

    public static bool operator !=(RenderState left, RenderState right) => !(left == right);
}
