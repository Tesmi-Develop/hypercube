using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Mathematics.Vectors;
using Silk.NET.SDL;

namespace Hypercube.Core.Graphics.UI;

public class Element
{
    public bool Visible;
    public bool Clip;
    
    public int ChildCount => _children.Count;

    private readonly List<Element> _children = [];

    public Element AddChild(Element element)
    {
        _children.Add(element);
        return element;
    }
    
    public Element GetChild(int index)
    {
        return _children[index];
    }

    public virtual void Render(IRenderContext context, Vector2i renderPosition)
    {
    }

    public virtual void PreRender(IRenderContext context)
    {
    }

    public virtual void PostRender(IRenderContext context)
    {
    }
}