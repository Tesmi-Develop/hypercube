using Hypercube.Core.Ecs.Core.Components;

namespace Hypercube.Core.Ecs.Core.Query;

[PublicAPI]
public ref struct QueryEnumerator<T>
    where T : struct, IComponent
{
    private readonly ComponentMapper<T> _mapper;

    public EntityId Entity { get; private set; } = EntityId.None;

    public ref T Component => ref _mapper.GetByIndex(Entity);
    
    public QueryEnumerator(ComponentMapper<T> mapper)
    {
        _mapper = mapper;
    }

    public bool MoveNext()
    {
        return ++Entity < _mapper.Count;
    }

    public bool MoveNext(out EntityId entity)
    {
        Entity++;
        entity = Entity;
        return Entity < _mapper.Count;
    }
    
    public bool MoveNext(out EntityId entity, out T component)
    {
        Entity++;
        entity = Entity;
        component = default;

        if (Entity < _mapper.Count)
            return false;
        
        component = ref _mapper.GetByIndex(Entity);
        return true;
    }
}

[PublicAPI]
public ref struct QueryEnumerator<T1, T2>
    where T1 : struct, IComponent
    where T2 : struct, IComponent
{
    private readonly ComponentMapper<T1> _mapper1;
    private readonly ComponentMapper<T2> _mapper2;
    private int _index;

    public EntityId Entity => _mapper1.GetEntityByIndex(_index);
    public ref T1 Component1 => ref _mapper1.GetByIndex(_index);
    public ref T2 Component2 => ref _mapper2.Get(Entity);
    
    public QueryEnumerator(ComponentMapper<T1> mapper1, ComponentMapper<T2> mapper2)
    {
        _index = -1;
        
        _mapper1 = mapper1;
        _mapper2 = mapper2;
    }
    
    public bool MoveNext()
    {
        while (++_index < _mapper1.Count)
        {
            var entity = _mapper1.GetEntityByIndex(_index);
            if (_mapper2.HasBoxed(entity))
                return true;
        }
        
        return false;
    }
    
    public bool MoveNext(ref T1 comp1, ref T2 comp2)
    {
        comp1 = default;
        comp2 = default;
        
        while (++_index < _mapper1.Count)
        {
            var id = _mapper1.GetEntityByIndex(_index);
            
            if (!_mapper2.HasBoxed(id))
                continue;

            comp1 = ref _mapper1.GetByIndex(_index);
            comp2 = ref _mapper2.Get(id);
            return true;
        }
        
        return false;
    }
    
    public bool MoveNext(out EntityId id, ref T1 comp1, ref T2 comp2)
    {
        id = EntityId.None;
        comp1 = default;
        comp2 = default;

        while (++_index < _mapper1.Count)
        {
            id = _mapper1.GetEntityByIndex(_index);
            
            if (!_mapper2.HasBoxed(id))
                continue;

            comp1 = ref _mapper1.GetByIndex(_index);
            comp2 = ref _mapper2.Get(id);
            return true;
        }

        return false;
    }
}
