using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
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
    [Dependency] private readonly IRenderManager _render = null!;
    [Dependency] private readonly IResourceManager _resource = null!;

    public override void Startup()
    {
        base.Startup();
        
        Subscribe<SpriteComponent, AddedEvent>(OnAdded);
    }

    private void OnAdded(ref Entity entity, ref SpriteComponent _, ref AddedEvent args)
    {
        ref var component = ref GetComponent<SpriteComponent>(entity);
        component.Texture = _resource.Load<Texture>(component.Path);
        
        if (component.Texture.Gpu is null)
            component.Texture.GpuBind(_render.Api);
    }

    public override void Draw(IRenderContext renderer)
    {
        Query((EntityId _, ref TransformComponent transformComponent, ref SpriteComponent spriteComponent) =>
        {
            var position = transformComponent.LocalPosition + spriteComponent.Offset;
            var rotation = transformComponent.LocalRotation + spriteComponent.Rotation;
            var scale = transformComponent.LocalScale * spriteComponent.Scale;
            
            if (spriteComponent.Texture is null)
                return;
            
            renderer.DrawTexture(spriteComponent.Texture, position, rotation, scale, spriteComponent.Color); 
            Logger.Debug($"p: {position}; r: {rotation}; s: {scale}; c: {spriteComponent.Color}");
        });
    }
}