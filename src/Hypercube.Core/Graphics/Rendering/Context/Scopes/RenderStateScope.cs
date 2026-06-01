using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Graphics.Rendering.Batching;
using Hypercube.Mathematics.Matrices;

namespace Hypercube.Core.Graphics.Rendering.Context.Scopes;

public readonly struct RenderStateScope : IDisposable
{
    private readonly IRenderingApi _api;
    private readonly RenderContext _context;

    private readonly RenderStateId _previousRenderStateId;

    public RenderStateScope(IRenderingApi api, RenderContext context, Matrix4x4 view, Matrix4x4 projection)
    {
        _api = api;
        _context = context;
        
        _previousRenderStateId = _api.GetCurrentRenderStateId();
        _api.SetRenderViewProjection(view, projection);
    }

    public void Dispose()
    {
        var previousState = _api.GetRenderState(_previousRenderStateId);
        _api.SetRenderViewProjection(previousState.View, previousState.Projection);
    }
}