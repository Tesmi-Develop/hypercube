using Hypercube.Core.Graphics.Rendering;
using Hypercube.Core.Viewports;
using Hypercube.Core.Windowing.Windows;

namespace Hypercube.Core.UI;

public readonly struct UIDrawPayload(DrawPayload payload)
{
    public readonly IWindow Window = payload.Window;
    public readonly ICamera Camera = payload.Camera;
}
