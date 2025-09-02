using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
using Hypercube.Core.Ecs.Core.Query;
using Hypercube.Core.Ecs.Events;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;
using Hypercube.Core.Systems.Transform;
using Hypercube.Mathematics.Quaternions;
using Hypercube.Mathematics.Vectors;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.Systems.Rendering;

[RegisterEntitySystem]
public sealed class ModelSystem : PatchEntitySystem
{
    [Dependency] private readonly IResourceManager _resource = default!;
    
    private EntityQuery _query = default!;
    
    public override void Startup()
    {
        base.Startup();

        _query = EntityQueryBuilder
            .With<TransformComponent>()
            .With<ModelComponent>()
            .Build();
        
        Subscribe<ModelComponent, AddedEvent>(OnAdded);
    }

    private void OnAdded(ref Entity entity, ref ModelComponent component, ref AddedEvent args)
    {
        component.Model = _resource.Load<Model>(component.Path);
    }
    
    public override void Draw(IRenderContext renderer)
    {
        var enumerator = _query.GetEnumerator;
        while (enumerator.MoveNext(out var entity))
        {
            var transformComponent = GetComponent<TransformComponent>(entity);
            var modelComponent = GetComponent<ModelComponent>(entity);

            var position = transformComponent.LocalPosition;
  
            if (modelComponent.Model is null)
                continue;
            
            renderer.DrawModel(modelComponent.Model, position, modelComponent.Rotation, modelComponent.Scale, modelComponent.Color);
        }
    }
}