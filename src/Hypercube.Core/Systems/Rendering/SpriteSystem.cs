using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
using Hypercube.Core.Ecs.Core.Query;
using Hypercube.Core.Ecs.Events;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;
using Hypercube.Core.Systems.Transform;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.Systems.Rendering;

[RegisterEntitySystem]
public sealed class SpriteSystem : PatchEntitySystem
{
    [Dependency] private readonly IRenderManager _render = default!;
    [Dependency] private readonly IResourceManager _resource = default!;
    
    private EntityQuery _spriteQuery = default!;
    
    public override void Startup()
    {
        base.Startup();

        _spriteQuery = EntityQueryBuilder
            .With<TransformComponent>()
            .With<SpriteComponent>()
            .Build();
        
        Subscribe<SpriteComponent, AddedEvent>(OnAdded);
    }

    private void OnAdded(ref Entity entity, ref SpriteComponent component, ref AddedEvent args)
    {
        component.Texture = _resource.Load<Texture>(component.Path);
        
        if (component.Texture.Gpu is null)
            component.Texture.GpuBind(_render.Api);
    }

    public override void Draw(IRenderContext renderer)
    {
        var enumerator = _spriteQuery.GetEnumerator;
        while (enumerator.MoveNext(out var entity))
        {
            var transformComponent = GetComponent<TransformComponent>(entity);
            var spriteComponent = GetComponent<SpriteComponent>(entity);

            var position = transformComponent.LocalPosition + spriteComponent.Offset;
            var rotation = transformComponent.LocalRotation + spriteComponent.Rotation;
            var scale = transformComponent.LocalScale * spriteComponent.Scale;
            
            if (spriteComponent.Texture is null)
                continue;
            
            renderer.DrawTexture(spriteComponent.Texture, position, rotation, scale, spriteComponent.Color);
        }
    }
}