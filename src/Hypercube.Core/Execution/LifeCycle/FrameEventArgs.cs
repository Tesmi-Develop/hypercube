namespace Hypercube.Core.Execution.LifeCycle;

public readonly struct FrameEventArgs
{
    public readonly TimeSpan Delta;
    
    public FrameEventArgs(TimeSpan delta)
    {
        Delta = delta;
    }
}
