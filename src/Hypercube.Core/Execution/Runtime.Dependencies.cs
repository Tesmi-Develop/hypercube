using Hypercube.Core.Audio.Manager;
using Hypercube.Core.Ecs;
using Hypercube.Core.Execution.LifeCycle;
using Hypercube.Core.Execution.Timing;
using Hypercube.Core.Graphics.Patching;
using Hypercube.Core.Graphics.Rendering;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Input.Handler;
using Hypercube.Core.Input.Manager;
using Hypercube.Core.Resources;
using Hypercube.Core.UI;
using Hypercube.Core.UI.Manager;
using Hypercube.Core.Viewports;
using Hypercube.Core.Windowing.Manager;
using Hypercube.Utilities.Configuration;
using Hypercube.Utilities.Debugging.Logger;

namespace Hypercube.Core.Execution;

public partial class Runtime
{
    private void InitPrimaryDependencies()
    {
        _dependencies.RegisterSingleton<ILogger>(_logger);
        
        _dependencies.Register<IConfigManager, ConfigManager>();
        _dependencies.Register<ITime, Time>();
    }
    
    private void InitDependencies()
    {
        // Core
        _dependencies.Register<IRuntimeLoop, RuntimeLoop>();

        // Resources
        _dependencies.RegisterSingleton<IResourceManager>(new ResourceManager(container: _dependencies));
        
        // Windowing
        _dependencies.Register<IWindowingManager, WindowingManager>();
        
        // Graphics
        _dependencies.Register<ICameraManager, CameraManager>();
        _dependencies.Register<IRenderContext, RenderContext>();
        _dependencies.Register<IRenderManager, RenderManager>();
        _dependencies.Register<IPatchManager, PatchManager>();
        _dependencies.Register<IRenderer, Renderer>();

        // Audio
        _dependencies.Register<IAudioManager, AudioManager>();
        
        // ECS
        _dependencies.Register<IEntitySystemManager, EntitySystemManager>();
        
        _dependencies.ResolveAll();
        _dependencies.Inject(this);
    }
    
    private void InitDependentsDependencies()
    {
        // Input
        _dependencies.Register<IInputHandler, InputHandler>();
        _dependencies.Register<IInputManager, InputManager>();
        
        // UI
        _dependencies.Register<IUIManager, UIManager>();
        
        _dependencies.ResolveAll();
    }
}