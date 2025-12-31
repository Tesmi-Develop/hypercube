using System.Collections.ObjectModel;
using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Graphics.Texturing;
using Hypercube.Core.Graphics.Texturing.Gpu;
using Hypercube.Core.Resources.Loaders;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Resources;

public sealed class Texture : Resource, IImage
{
    public Vector2i Size { get; }
    public Rect2 UV { get; }
    public int Channels { get; }
    public IGpuTexture? Gpu { get; private set; }

    private readonly byte[] _data;

    public ReadOnlyCollection<byte> Data => _data.AsReadOnly();
    
    public Texture(Vector2i size, byte[] data, int channels, Rect2 uv)
    {
        _data = data;
        
        Size = size;
        Channels = channels;
        UV = uv;
    }

    public void GpuBind(IRenderingApi api)
    {
        Gpu?.Dispose();
        Gpu = new GpuTexture(api, Size.X, Size.Y, Channels, _data);
    }
    
    public ReadOnlySpan<byte> GetPixel(Vector2i position)
    {
        return GetPixel(position.X, position.Y);
    }

    public ReadOnlySpan<byte> GetPixel(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Size.X || y >= Size.Y)
            throw new ArgumentOutOfRangeException();

        var index = (y * Size.X + x) * Channels;
        return new ReadOnlySpan<byte>(_data, index, Channels);
    }
    
    public override void Dispose()
    {
        Gpu?.Dispose();
        Gpu = null;
    }

    public override string ToString()
    {
        return _data.GetHashCode().ToString();
    }
}