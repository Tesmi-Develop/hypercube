using Hypercube.Core.Ecs;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Systems.Physics;

public struct PhysicsComponent : IComponent
{
    public Vector2 LinearVelocity;
    public float AngularVelocity;
    public float Mass;
    public float Friction;
    public float Restitution;
}
