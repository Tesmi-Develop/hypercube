using Hypercube.Core.Windowing.Windows;
using WindowHandle = Hypercube.Core.Windowing.Windows.WindowHandle;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial class RenderContext
{
    public IDisposable UseContextScope(IWindow window) => new WindowScope(this, window.Handle);
    
    public readonly struct WindowScope : IDisposable
    {
        private readonly RenderContext _context;
        private readonly WindowHandle _previousHandle;

        public WindowScope(RenderContext context, WindowHandle handle)
        {
            _context = context;
            _previousHandle = _context._windowingApi.Context;
            _context._windowingApi.Context = handle;
        }

        public void Dispose()
        {
            _context._windowingApi.Context = _previousHandle;
        }
    }
}