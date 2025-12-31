using Hypercube.Core.Execution.LifeCycle;
using Hypercube.Core.Graphics.Patching;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Windowing;
using Hypercube.Core.Windowing.Manager;
using Hypercube.Core.Windowing.Settings;
using Hypercube.Utilities.Dependencies;
using Hypercube.Utilities.Extensions;

namespace Hypercube.Core.Graphics.Rendering;

[UsedImplicitly]
[EngineInternal]
public class Renderer : IRenderer, IPostInject
{
    [Dependency] private readonly IRuntimeLoop _runtimeLoop = default!;
    [Dependency] private readonly IWindowManager _windowManager = default!;
    [Dependency] private readonly IRenderManager _renderManager = default!;
    [Dependency] private readonly IRenderContext _renderContext = default!;
    [Dependency] private readonly IPatchManager _patchManager = default!;
    
    private readonly ManualResetEvent _readyEvent = new(false);
    
    private Thread? _thread;
    private RendererSettings _settings;
    private IWindow? _window;

    public void OnPostInject()
    {
        _runtimeLoop.Actions.Add(_ => Update(), EngineUpdatePriority.RendererUpdate);
        _runtimeLoop.Actions.Add(_ => Render(), EngineUpdatePriority.RendererRender);
    }

    public void Init(RendererSettings settings)
    {
        InitAsync(settings).Wait();
    }

    public void Load()
    {
        _renderManager.Load();
    }

    public void Shutdown()
    {
        _windowManager.Shutdown();
        _renderManager.Shutdown();
    }

    public void Update()
    {
        _windowManager.PollEvents();
    }

    public void Draw()
    {
        foreach (var patch in _patchManager.Patches)
        {
            patch.Draw(_renderContext);
        }   
    }

    public void Render()
    {
        if (_window is null)
            throw new Exception();
        
        _renderManager.Render(_window);
    }

    public IWindow CreateMainWindow(WindowCreateSettings settings)
    {
        if (_window is not null)
            throw new Exception();
        
        _window = _windowManager.Create(settings);
        _window.MakeCurrent();
        _renderManager.Init(_window, _settings.RenderingApi);

        return _window;
    }

    private Task InitAsync(RendererSettings settings)
    {
        _settings = settings;
        _renderManager.OnDraw += Draw;

        if (_settings.Thread is not { } thread)
        {
            _windowManager.Init(_settings.WindowingApi);
            _windowManager.WaitInit(_settings.ReadySleepDelay);
            
            return Task.CompletedTask;
        }
        
        _thread = new Thread(OnThreadStart, thread.StackSize)
        {
            Name = thread.Name,
            Priority = thread.Priority,
            IsBackground = false
        };
        
        _thread.Start();
        
        return _readyEvent.AsTask();
    }

    private void OnThreadStart()
    {
        if (_settings.Thread is not { } thread)
            throw new InvalidOperationException();
        
        _windowManager.Init(_settings.WindowingApi);
        _windowManager.WaitInit(_settings.ReadySleepDelay);
        
        _readyEvent.Set();
        
        _windowManager.EnterLoop();
    }
}