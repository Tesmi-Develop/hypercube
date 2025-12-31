using Hypercube.Core.Ecs.Core.Components;

namespace Hypercube.Core.Ecs.Core.Query;

public delegate void RefAction<T>(EntityId id, ref T component);
public delegate void RefAction<T1, T2>(EntityId id, ref T1 component1, ref T2 component2);

public readonly struct Query<T>
    where T : struct, IComponent
{
    private readonly ComponentMapper<T> _mapper;
    
    public Query(ComponentMapper<T> mapper)
    {
        _mapper = mapper;
    }

    public void ForEach(RefAction<T> action)
    {
        var count = _mapper.Count;
        for (var i = 0; i < count; i++)
        {
            ref var c1 = ref _mapper.GetByIndex(i);
            var entity = _mapper.GetEntityByIndex(i);
            action(entity, ref c1);
        }
    }
}

public readonly struct Query<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    private readonly ComponentMapper<T1> _mapper1;
    private readonly ComponentMapper<T2> _mapper2;

    public Query(ComponentMapper<T1> mapper1, ComponentMapper<T2> mapper2)
    {
        _mapper1 = mapper1;
        _mapper2 = mapper2;
    }

    public void ForEach(RefAction<T1, T2> action)
    {
        for (var i = 0; i < _mapper1.Count; i++)
        {
            var entity = _mapper1.GetEntityByIndex(i);
            
            if (!_mapper2.HasBoxed(entity))
                continue;

            ref var comp1 = ref _mapper1.GetByIndex(i);
            ref var comp2 = ref _mapper2.Get(entity);

            action(entity, ref comp1, ref comp2);
        }
    }
}
