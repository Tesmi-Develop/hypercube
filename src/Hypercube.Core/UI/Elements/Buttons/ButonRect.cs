using Hypercube.Mathematics.Dimensions;

namespace Hypercube.Core.UI.Elements.Buttons;

[PublicAPI]
public class ButonRect : Button
{
    public Rectangle Fill { get; private set; } = null!;

    protected override void OnStartup()
    {
        base.OnStartup();
        
        Fill = AddChild(new Rectangle
        {
            Position = HDim2.Zero,
            Size = HDim2.ScalarOne,
        });
    }
}
