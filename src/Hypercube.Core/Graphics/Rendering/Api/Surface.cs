using Hypercube.Core.Graphics.Objects.Texturing;

namespace Hypercube.Core.Graphics.Rendering.Api;

public record struct Surface
{
    public readonly uint Fbo;
    public readonly TextureHandle FboTextureHandle;
    public Vector2i Size { get; set; }

    public Surface(uint fbo, TextureHandle fboTextureHandle, Vector2i size)
    {
        Fbo = fbo;
        FboTextureHandle = fboTextureHandle;
        Size = size;
    }

}