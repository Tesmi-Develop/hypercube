using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;
using Hypercube.Core.Serialization;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Systems.Rendering;

public sealed class SpriteComponent : Component
{
    public Texture? Texture;

    [DataField("texture")]
    public ResourcePath Path = ResourcePath.Empty;

    [DataField]
    public Vector2 Scale = Vector2.One;

    [DataField]
    public Angle Rotation = Angle.Zero;

    [DataField]
    public Vector2 Offset = Vector2.Zero;

    [DataField]
    public Color Color = Color.White;
}