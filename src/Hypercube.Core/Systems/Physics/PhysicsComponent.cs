using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Systems.Physics;

public sealed class PhysicsComponent : Component
{
    public Vector2 LinearVelocity;
    public float AngularVelocity;
    public float Mass;
    public float Friction;
    public float Restitution;
}
