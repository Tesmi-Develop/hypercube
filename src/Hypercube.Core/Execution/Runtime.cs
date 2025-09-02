using System.Reflection;
using Hypercube.Core.Audio.Manager;
using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Core.Systems;
using Hypercube.Core.Execution.Attributes;
using Hypercube.Core.Execution.Enums;
using Hypercube.Core.Graphics.Patching;
using Hypercube.Core.Graphics.Rendering;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Graphics.Viewports;
using Hypercube.Core.Input;
using Hypercube.Core.Resources;
using Hypercube.Core.Utilities.Helpers;
using Hypercube.Core.Windowing.Manager;
using Hypercube.Core.Windowing.Settings;
using Hypercube.Utilities.Configuration;
using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;
using Hypercube.Utilities.Extensions;
using Hypercube.Utilities.Helpers;

namespace Hypercube.Core.Execution;

public sealed class Runtime
{
    private readonly DependenciesContainer _dependencies = new();

    [Dependency] private readonly IConfigManager _configManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;
    [Dependency] private readonly IRuntimeLoop _runtimeLoop = default!;
    [Dependency] private readonly IResourceManager _resourceManager = default!;
    [Dependency] private readonly IRenderer _renderer = default!;
    [Dependency] private readonly IAudioManager _audioManager = default!;

    private readonly ConsoleLogger _logger = new();
    private readonly Dictionary<EntryPointLevel, List<MethodInfo>> _entryPoints = [];
    
    public void Start()
    {
        Thread.CurrentThread.Name = "Main";
        
        InitPrimaryDependencies();
        
        _logger.Echo(EngineInfo.WelcomeMessage);
        _logger.Info("Dependency initialization...");
        
        InitDependencies();

        _configManager.Init();
        
        _logger.Info("Dependency initialization is complete!");
        _logger.Info("Preparing for the execution of entry points...");

        EntryPointsLoad();
        EntryPointsExecute(EntryPointLevel.BeforeInit);
        
        _resourceManager.Mount(Config.MountFolders);
        
        _logger.Info("The entry points are called!");
        _logger.Info("Initialization of internal modules...");
        
        _renderer.Init(new RendererSettings
        {
            Thread = WindowingThreadSettingsHelper.FromConfig(),
            WindowingApi = WindowingApiSettingsHelper.FromConfig(),
            RenderingApi = RenderingApiSettingsHelper.FromConfig(),
            ReadySleepDelay = Config.WindowingThreadReadySleepDelay
        });
        
        _renderer.CreateMainWindow(new WindowCreateSettings
        {
            Api = new ApiSettings
            {
                Api = ContextApi.OpenGl,
                Flags = ContextFlags.Debug,
                Profile = ContextProfile.Core,
                Version = new Version(4, 6)
            },
            Title = Config.MainWindowTitle,
            Resizable = Config.MainWindowResizable,
            Decorated = Config.MainWindowDecorated,
            Floating = Config.MainWindowFloating,
            Visible = Config.MainWindowVisible,
            TransparentFramebuffer = Config.MainWindowTransparentFramebuffer,
        });
        
        _audioManager.Init();
        
        _resourceManager.AddAllLoaders();
        
        _renderer.Load();

        InitDependentsDependencies();
        
        _entitySystemManager.CrateMainWorld();

        _logger.Info("Preparation is complete, start the main application cycle");
        EntryPointsExecute(EntryPointLevel.AfterInit);
        _runtimeLoop.Run();
    }

    private void InitPrimaryDependencies()
    {
        _dependencies.RegisterSingleton<ILogger>(_logger);
    }
    
    private void InitDependencies()
    {
        // Core
        _dependencies.Register<IConfigManager, ConfigManager>();
        _dependencies.Register<IRuntimeLoop, RuntimeLoop>();
        _dependencies.Register<IEntitySystemManager, EntitySystemManager>();

        // Resources
        _dependencies.RegisterSingleton<IResourceManager>(new ResourceManager(_dependencies));
        
        // Windowing
        _dependencies.Register<IWindowManager, WindowManager>();
        
        // Graphics
        _dependencies.Register<ICameraManager, CameraManager>();
        _dependencies.Register<IRenderContext, RenderContext>();
        _dependencies.Register<IRenderManager, RenderManager>();
        _dependencies.Register<IPatchManager, PatchManager>();
        _dependencies.Register<IRenderer, Renderer>();
        
        // Audio
        _dependencies.Register<IAudioManager, AudioManager>();

        _dependencies.ResolveAll();
        _dependencies.Inject(this);
    }
    
    private void InitDependentsDependencies()
    {
        // Input
        _dependencies.Register<IInputHandler, InputHandler>();
        _dependencies.ResolveAll();
    }

    private void EntryPointsLoad()
    {
        var methods = ReflectionHelper.GetExecutableMethodsWithAttributeFromAllAssemblies<EntryPointAttribute>(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var method in methods)
        {
            if (!method.IsStatic)
                throw new InvalidOperationException();
            
            var attribute = method.GetCustomAttribute<EntryPointAttribute>()!;
            _entryPoints.GetOrInstantiate(attribute.Level).Add(method);
            
            _logger.Debug($"Loaded {method.Name} entry point");
        }
    }
    
    private void EntryPointsExecute(EntryPointLevel level)
    {
        if (!_entryPoints.TryGetValue(level, out var entryPoints))
            return;
        
        foreach (var method in entryPoints)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 1)
            {
                if (parameters[0].ParameterType == typeof(DependenciesContainer))
                {
                    method.Invoke(null, [_dependencies]);
                    continue;
                }
                
                throw new InvalidOperationException();
            }
            
            method.Invoke(null, null);
        }
    }
}