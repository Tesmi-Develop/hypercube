namespace Hypercube.Core.Ecs.Core.Components;

[PublicAPI, EngineInternal]
public interface IComponentMapper
{
    event Action<int>? Added;
    event Action<int>? Removed;
    
    bool Empty { get; }
    int Count { get; }

    IEnumerable<EntityId> Entities { get; }
    
    bool HasBoxed(EntityId id);
    bool SetBoxed(EntityId id, ref IComponent component);
    ref IComponent GetBoxed(EntityId id);
}