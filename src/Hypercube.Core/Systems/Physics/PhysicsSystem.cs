using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;

namespace Hypercube.Core.Systems.Physics;

[RegisterEntitySystem]
public class PhysicsSystem : EntitySystem
{
    // [Dependency] private readonly TransformSystem _transform = default!;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
}