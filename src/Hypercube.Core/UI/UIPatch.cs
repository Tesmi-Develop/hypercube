using Hypercube.Core.Graphics.Patching;
using Hypercube.Core.Graphics.Rendering;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.UI.Manager;

namespace Hypercube.Core.UI;

public sealed class UIPatch : Patch
{
    private readonly UIManager _uiManager;
    
    public override int Priority => -1000;
    
    public UIPatch(UIManager uiManager)
    {
        _uiManager = uiManager;
    }
    
    public override void Draw(IRenderContext renderer, DrawPayload payload)
    {
        using (renderer.UseRenderState(payload.Window))
        {
            _uiManager.Root.Render(renderer, new UIDrawPayload(payload));
        }
    }
}
