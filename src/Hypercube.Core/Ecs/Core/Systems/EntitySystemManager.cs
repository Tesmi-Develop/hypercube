using Hypercube.Core.Ecs.Utilities;
using Hypercube.Core.Execution.LifeCycle;
using Hypercube.Utilities.Collections;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.Ecs.Core.Systems;

[EngineInternal]
[UsedImplicitly]
public sealed class EntitySystemManager : IEntitySystemManager, IPostInject
{
    [Dependency] private readonly DependenciesContainer _container = default!;
    [Dependency] private readonly IRuntimeLoop _runtimeLoop = default!;
    
    public IWorld Main { get; private set; } = default!;

    private readonly WorldRegistrar _registrar = new();
    private readonly NumPool<int> _worldIdPool = new();

    private IWorld[] _worlds = [];
    
    public void OnPostInject()
    {
        _runtimeLoop.Actions.Add(OnUpdate, EngineUpdatePriority.EntitySystemManager);
    }

    public void CrateMainWorld()
    {
        if (Main is not null)
            throw new InvalidOperationException();

        Main = CreateWorld();
    }

    public IWorld CreateWorld()
    {
        var world = new World(_worldIdPool.Next, _registrar.GetTypes(), _container);
        
        if (_worlds.Length >= world.Id)
            Array.Resize(ref _worlds, world.Id + 1);

        _worlds[world.Id] = world;
        
        return world;
    }

    private void OnUpdate(FrameEventArgs args)
    {
        foreach (var world in _worlds)
            world.Update(args.DeltaSeconds);
    }
}