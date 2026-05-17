using Hypercube.Core.Graphics.Rendering.Context.Scopes;
using Hypercube.Core.Viewports;
using Hypercube.Core.Windowing.Windows;
using Hypercube.Mathematics.Matrices;
using Hypercube.Mathematics.Quaternions;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial class RenderContext
{
    public IDisposable UseRenderState(IWindow window)
    {
        var view = Matrix4x4.CreateTransformSRT(new Vector3(-window.Size.X, -window.Size.Y, 0) / 2f, Quaternion.Identity, Vector3.One);
        var projection = Matrix4x4.CreateOrthographic(window.Size, -1, 1);
        
        return UseRenderState(view, projection);
    }

    public IDisposable UseRenderState(ICamera camera)
    {
        var view = camera.View;
        var projection = camera.Projection;
        
        return UseRenderState(view, projection);
    }

    public IDisposable UseRenderState(Matrix4x4 view, Matrix4x4 projection) =>
        new RenderStateScope(_renderingApi, this, view, projection);
}
