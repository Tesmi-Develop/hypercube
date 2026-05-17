using Hypercube.Core.Windowing.Windows;
using Hypercube.Mathematics.Shapes;
using WindowHandle = Hypercube.Core.Windowing.Windows.WindowHandle;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial class RenderContext
{
    public IDisposable UseScissor(Rect2i rect) => new ScissorCope(this, rect);

    public readonly struct ScissorCope : IDisposable
    {
        private readonly RenderContext _context;
        
        public ScissorCope(RenderContext context, Rect2i rect)
        {
            _context = context;
            
            _context.Scissor(true);
            _context.SetScissorRect(rect);
        }
        
        public void Dispose()
        {
            _context.Scissor(false);
        }
    }
}