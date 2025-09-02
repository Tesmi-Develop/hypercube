using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Resources;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.UI.Elements;

public class Label : Element
{
    public ResourcePath FontPath;
    public string Text;

    public override void Render(IRenderContext context, Vector2i renderPosition)
    {
        //context.DrawText(Text, renderPosition, Color.White);
    } 
}