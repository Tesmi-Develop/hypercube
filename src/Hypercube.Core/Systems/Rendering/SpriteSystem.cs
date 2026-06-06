using Hypercube.Core.Graphics.Rendering;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;
using Hypercube.Core.Systems.Transform;
using Hypercube.Core.Viewports;
using Hypercube.Core.Windowing.Windows;
using Hypercube.Ecs;
using Hypercube.Ecs.Lifetime;
using Hypercube.Ecs.Queries;
using Hypercube.Mathematics.Matrices;
using Hypercube.Mathematics.Vectors;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.Systems.Rendering;

public sealed class SpriteSystem : PatchEntitySystem
{
    [Dependency] private readonly IRenderManager _render = null!;
    [Dependency] private readonly IResourceManager _resource = null!;

    private Query _spriteQuery = null!;
    
    public override void Initialize()
    {
        _spriteQuery = CreateQuery(new QueryMeta()
            .WithAll<TransformComponent>()
            .WithAll<SpriteComponent>()
        );
        
        Subscribe<SpriteComponent, AddedEvent>(OnAdded);
    }
    
    private void OnAdded(Entity entity, ref SpriteComponent component, ref AddedEvent args)
    {
        if (component.Texture is not null)
        {
            if (component.Texture.Gpu is null)
                component.Texture.GpuBind(_render.Api);
            
            return;
        }
        
        if (string.IsNullOrEmpty(component.Path))
            return;
        
        component.Texture = _resource.Load<Texture>(component.Path);
        
        if (component.Texture.Gpu is null)
            component.Texture.GpuBind(_render.Api);
    }

    public override void Draw(IRenderContext renderer, DrawPayload payload)
    {
        var camera = payload.Camera;
        
        var halfWidthPx = camera.Size.X * 0.5f;
        var halfHeightPx = camera.Size.Y * 0.5f;
        
        var worldUnitsPerPixelX = 1.0f / camera.Scale.X;
        var worldUnitsPerPixelY = 1.0f / camera.Scale.Y;
        
        var halfWidthWorld = halfWidthPx * worldUnitsPerPixelX;
        var halfHeightWorld = halfHeightPx * worldUnitsPerPixelY;
        
        var camX = camera.Position.X;
        var camY = camera.Position.Y;
        
        var left = camX - halfWidthWorld;
        var right = camX + halfWidthWorld;
        var bottom = camY - halfHeightWorld;
        var top = camY + halfHeightWorld;

        const float padding = 10.0f;

        _spriteQuery.With<TransformComponent, SpriteComponent>((_, ref transform, ref sprite) =>
        {
            if (sprite.Texture is null)
                return;

            if (sprite.Texture.Gpu is null)
                sprite.Texture.GpuBind(_render.Api);

            var position = transform.LocalPosition + sprite.Offset;
            
            var halfSpriteW = (sprite.Scale.X * transform.LocalScale.X) * 0.5f;
            var halfSpriteH = (sprite.Scale.Y * transform.LocalScale.Y) * 0.5f;

            var spriteLeft = position.X - halfSpriteW;
            var spriteRight = position.X + halfSpriteW;
            var spriteBottom = position.Y - halfSpriteH;
            var spriteTop = position.Y + halfSpriteH;

            if (spriteRight < left - padding ||
                spriteLeft > right + padding ||
                spriteTop < bottom - padding ||
                spriteBottom > top + padding)
            {
                return;
            }

            var rotation = transform.LocalRotation.ToEuler().Z + sprite.Rotation;
            var scale = transform.LocalScale * sprite.Scale;

            if (sprite.Shader is not null)
                renderer.SetShader(sprite.Shader);
            
            renderer.DrawTexture(sprite.Texture, position.Xy, rotation, scale.Xy, sprite.Color, sprite.Uv);
            renderer.ClearShader();
        });
    }
}