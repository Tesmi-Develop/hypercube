using Hypercube.Core.Graphics.Fonts;
using Hypercube.Core.Resources;
using Hypercube.Core.Resources.FileSystems;
using Hypercube.Core.Resources.Loaders;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;
using StbImageSharp;

namespace Hypercube.Core.Graphics.Resources;

public class FontResourceLoader : ResourceLoader<Font>
{
    private const int DefaultSize = 16;
    
    public override string[] Extensions => ["ttf", "otf"];
    public override bool SupportLoadArgs => true;

    public override bool CanLoad(ResourcePath path, IFileSystem fileSystem)
    {
        return Extensions.Contains(path.Extension, StringComparer.OrdinalIgnoreCase);
    }

    public override Font Load(ResourcePath path, IFileSystem fileSystem)
    {
        return Load(path, DefaultSize, fileSystem);
    }

    public override Font Load(ResourcePath path, IFileSystem fileSystem, ResourceLoadArg[] args)
    {
        var size = DefaultSize;
        foreach (var arg in args)
        {
            if (arg is { Key: "size", Value: int value })
                size = value;
        }
        
        return Load(path, size, fileSystem);
    }

    private static Font Load(ResourcePath path, int size, IFileSystem fileSystem)
    {
        var stream = fileSystem.OpenRead(path);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var fontData = memory.ToArray();
        
        var fontStream = FontAtlasGenerator.Generate(fontData, out var glyphs, size);
        var result = ImageResult.FromStream(fontStream, ColorComponents.RedGreenBlueAlpha);
        var texture = new Texture(new Vector2i(result.Width, result.Height), result.Data, (int) ColorComponents.RedGreenBlueAlpha, Rect2.UV);
        
        return new Font(texture, glyphs, size);
    }
}