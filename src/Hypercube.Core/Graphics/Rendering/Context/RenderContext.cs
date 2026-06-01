using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Graphics.Rendering.Shaders;
using Hypercube.Core.Windowing.Api;
using Hypercube.Mathematics.Shapes;

namespace Hypercube.Core.Graphics.Rendering.Context;

[EngineInternal, UsedImplicitly]
public partial class RenderContext : IRenderContext
{
    private IRenderingApi _renderingApi = null!;
    private IWindowingApi _windowingApi = null!;
    private IShaderProgram? _shaderProgram;
    
    public void Init(IRenderingApi renderingApi, IWindowingApi windowingApi)
    {
        _renderingApi = renderingApi;
        _windowingApi = windowingApi;
    }

    public void Scissor(bool value) => _renderingApi.Scissor(value);

    public void SetScissorRect(Rect2i rect) => _renderingApi.SetScissorRect(rect);

    public void SetBlendMode(BlendMode mode)
    {
        _renderingApi.SetRenderBlendMode(mode);
    }
    public void SetShader(IShaderProgram shader) {
        _shaderProgram?.Stop();
        _shaderProgram = shader;
        _shaderProgram.Use();
    }

    public void ClearShader()
    {
        _shaderProgram?.Stop();
        _shaderProgram = null;
    }

    public void BindSurface(Surface surface)
    {
        _renderingApi.BreakCurrentBatch();
        _renderingApi.BindSurface(surface);
    }

    public void UnbindSurface()
    {
        _renderingApi.BreakCurrentBatch();
        _renderingApi.UnbindSurface();
    }

    public Surface CreateSurface(Vector2i size)
    {
        return _renderingApi.CreateSurface(size);
    }
}