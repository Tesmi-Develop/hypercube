using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.UI.Elements;
using Hypercube.Core.Windowing;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.UI;

public class UIManager : IUIManager
{
    private Dictionary<IWindow, Root> _roots = new();

    private void CreateRoot(IWindow window)
    {
        var root = new Root
        {
            Window = window,
        };
        
        _roots.Add(window, root);
    }

    private void DrawElement(IRenderContext context, Element element, Vector2i position)
    {
        if (!element.Visible)
            return;

        if (element.Clip)
        {
            context.Scissor(true);
        }
        
        element.PreRender(context);

        for (var index = 0; index < element.ChildCount; index++)
        {
            var child = element.GetChild(index);
            
            var renderPosition = position;
            child.Render(context, renderPosition);
        }
        
        element.PostRender(context);
    }
}