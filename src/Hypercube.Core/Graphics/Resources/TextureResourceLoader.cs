using Hypercube.Core.Resources;
using Hypercube.Core.Resources.FileSystems;
using Hypercube.Core.Resources.Loaders;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;
using StbImageSharp;

namespace Hypercube.Core.Graphics.Resources;

public sealed class TextureResourceLoader : ResourceLoader<Texture>
{
    public override string[] Extensions =>
    [
        "png", "jpg", "jpeg", "bmp", "tga"
    ];

    public override bool CanLoad(ResourcePath path, IFileSystem fileSystem)
    {
        return true;
    }

    public override Texture Load(ResourcePath path, IFileSystem fileSystem)
    {
        var tempFlipGlobal = StbImage.stbi__vertically_flip_on_load_global;

        try
        {
            using var stream = fileSystem.OpenRead(path);
            // TODO: flip setting by ResourceLoadArg[]
            
            var result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            return new Texture(new Vector2i(result.Width, result.Height), result.Data, (int) ColorComponents.RedGreenBlueAlpha, Rect2.UV);
        }
        finally
        {
            StbImage.stbi__vertically_flip_on_load_global = tempFlipGlobal;   
        }
    }
}