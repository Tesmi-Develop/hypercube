using Hypercube.Core.Execution.LifeCycle;
using Hypercube.Core.Graphics.Patching;
using Hypercube.Core.Input.Handler;
using Hypercube.Core.Resources;
using Hypercube.Core.UI.Elements;
using Hypercube.Core.Viewports;
using Hypercube.Core.Windowing.Manager;
using Hypercube.Mathematics.Dimensions;
using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.UI.Manager;

public sealed class UIManager : IUIManager, IPostInject
{
    [PublicAPI] public readonly UIPatch Patch;
    
    [Dependency] private readonly IResourceManager _resourceManager = null!;
    [Dependency] private readonly IInputHandler _inputHandler = null!;

    [Dependency] private readonly IRuntimeLoop _runtimeLoop = null!;
    [Dependency] private readonly IPatchManager _patchManager = null!;
    [Dependency] private readonly ICameraManager _cameraManager = null!;
    [Dependency] private readonly IWindowingManager _windowingManager = null!;
    [Dependency] private readonly ILogger _logger = null!;

    public IDependenciesContainer DependenciesContainer { get; }
    public UIRoot Root { get; }

    public IResourceManager ResourceManager => _resourceManager;
    public IInputHandler InputHandler => _inputHandler;

    public Vector2i MousePosition => new(_inputHandler.MousePosition.X,  _windowingManager.MainWindow.Size.Y - _inputHandler.MousePosition.Y);
    public Vector2i ViewportSize => _cameraManager.MainCamera.Size;
    
    public void OnPostInject()
    {
        _patchManager.AddPatch(Patch);
        _runtimeLoop.Actions.Add(OnUpdate, EngineUpdatePriority.UIUpdate);

        _cameraManager.OnMainCameraResized += () =>
        {
            Root.UpdateLayout();
        };
    }

    public UIManager(IDependenciesContainer dependenciesContainer)
    {
        DependenciesContainer = new DependenciesContainer(dependenciesContainer);
        Patch = new UIPatch(this);

        Root = new UIRoot
        {
            Size = HDim2.ScalarOne
        };
        
        Root.UI = this;
        Root.Started = true;
    }

    public void AddElement(Element element)
    {
        Root.AddChild(element);
    }

    public void RemoveElement(Element element)
    {
        Root.RemoveChild(element);
    }

    private void OnUpdate(FrameEventArgs args)
    {
        Root.Update(args);
    }
}
