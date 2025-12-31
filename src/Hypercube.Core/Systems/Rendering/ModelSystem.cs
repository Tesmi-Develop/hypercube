using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
using Hypercube.Core.Ecs.Events;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;
using Hypercube.Core.Systems.Transform;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.Systems.Rendering;

[RegisterEntitySystem]
public sealed class ModelSystem : PatchEntitySystem
{
    [Dependency] private readonly IResourceManager _resource = default!;
    public override void Startup()
    {
        base.Startup();
        
        Subscribe<ModelComponent, AddedEvent>(OnAdded);
    }

    private void OnAdded(ref Entity entity, ref ModelComponent component, ref AddedEvent args)
    {
        component.Model = _resource.Load<Model>(component.Path);
    }
    
    public override void Draw(IRenderContext renderer)
    {
        Query((EntityId _, ref TransformComponent transformComponent, ref ModelComponent modelComponent) =>
        {
            var position = transformComponent.LocalPosition;
            if (modelComponent.Model is null)
                return;
            
            renderer.DrawModel(modelComponent.Model, position, modelComponent.Rotation, modelComponent.Scale, modelComponent.Color);
        });
    }
}