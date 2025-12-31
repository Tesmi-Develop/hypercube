using Hypercube.Core.Ecs;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Systems.Transform;

public struct TransformComponent() : IComponent
{
    public Entity? Parent;
    public Vector2 LocalPosition;
    public Angle LocalRotation;
    public Vector2 LocalScale = Vector2.One;
}
