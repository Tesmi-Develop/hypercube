using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Windowing.Api;
using Hypercube.Mathematics.Shapes;

namespace Hypercube.Core.Graphics.Rendering.Context;

[EngineInternal, UsedImplicitly]
public partial class RenderContext : IRenderContext
{
    private IRenderingApi _renderingApi = null!;
    private IWindowingApi _windowingApi = null!;
    
    public void Init(IRenderingApi renderingApi, IWindowingApi windowingApi)
    {
        _renderingApi = renderingApi;
        _windowingApi = windowingApi;
    }

    public void Scissor(bool value) => _renderingApi.Scissor(value);

    public void SetScissorRect(Rect2i rect) => _renderingApi.SetScissorRect(rect);
}