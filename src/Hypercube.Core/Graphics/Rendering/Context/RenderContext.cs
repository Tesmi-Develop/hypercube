using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Mathematics.Shapes;

namespace Hypercube.Core.Graphics.Rendering.Context;

[EngineInternal, UsedImplicitly]
public partial class RenderContext : IRenderContext
{
    private IRenderingApi _renderingApi = default!;
    
    public void Init(IRenderingApi api)
    {
        _renderingApi = api;    
    }

    public void Scissor(bool value)
    {
        _renderingApi.SetScissor(value);
    }

    public void Scissor(Rect2i rect)
    {
        _renderingApi.SetScissorRect(rect);
    }
}