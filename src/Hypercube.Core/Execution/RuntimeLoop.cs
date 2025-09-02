using Hypercube.Utilities.Collections;
using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.Execution;

[UsedImplicitly]
public sealed class RuntimeLoop : IRuntimeLoop
{
    [Dependency] private readonly ILogger _logger = default!;
    
    private readonly OrderedActions<FrameEventArgs> _actions = [];
    
    public bool Running { get; private set; }

    public ISubscribableOrderedActions<FrameEventArgs> Actions => _actions;

    public void Run()
    {
        if (Running)
            throw new InvalidOperationException();

        _logger.Debug("Started run loop, suspend main thread");
        
        Running = true;
        
        while (Running)
        {
            OnRun();
        }
        
        _logger.Debug("Shutdown run loop, unsuspend main thread");
    }

    public void Shutdown()
    {
        Running = false;
    }
    
    private void OnRun()
    {
        _actions.InvokeAll(new FrameEventArgs(1));
    }
}