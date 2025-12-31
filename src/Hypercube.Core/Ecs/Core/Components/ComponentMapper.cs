using System.Runtime.CompilerServices;

namespace Hypercube.Core.Ecs.Core.Components;

[EngineInternal]
public sealed class ComponentMapper<T> : IComponentMapper
    where T : struct, IComponent
{
    private const int DefaultIndex = -1;
    private const int GrowthFactor = 2;

    public IEnumerable<EntityId> Entities { get; }

    public event Action<int>? Added; 
    public event Action<int>? Removed; 

    private T[] _components = [];
    private int[] _mapping = [];    

    private int _lastComponentIndex = DefaultIndex;

    public bool Empty => _lastComponentIndex == DefaultIndex;
    public int Count => _lastComponentIndex + 1;

    public ref T this[EntityId entity]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Get(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has(EntityId id)
    {
        return id < _mapping.Length && _mapping[id] != EntityId.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasBoxed(EntityId id)
    {
        return Has(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Set(EntityId id, in T component)
    {
        Resize(ref _mapping, id, EntityId.None);
        
        var isNew = true;
        ref var componentIndex = ref _mapping[id];
        
        if (componentIndex != DefaultIndex)
        {
            Remove(id);
            isNew = false;
        }
        
        componentIndex = ++_lastComponentIndex;

        Resize(ref _components, _lastComponentIndex);

        _components[_lastComponentIndex] = component;
        Added?.Invoke(id);
        
        return isNew;
    }

    public bool SetBoxed(EntityId id, ref IComponent component)
    {
        return Set(id, Unsafe.As<IComponent, T>(ref component));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Set(EntityId id, in IComponent component)
    {
        var casted = (T) component;
        return Set(id, in casted);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(EntityId entity)
    {
        if (entity >= _mapping.Length)
            return false;

        ref var index = ref _mapping[entity];
        if (index == DefaultIndex)
            return false;
        
        Removed?.Invoke(entity);
        index = DefaultIndex;
        return true;
    }

    /// <summary>
    /// Retrieves a reference to the component associated with the given entity.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>A reference to the component.</returns>
    /// <remarks>
    /// Use <c>ref</c> access to modify the component directly in-place for maximum performance.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(EntityId id)
    {
        return ref _components[_mapping[id]];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref IComponent GetBoxed(EntityId id)
    {
        return ref Unsafe.As<T, IComponent>(ref Get(id));
    }

    /// <summary>
    /// Retrieves a reference to a component by its dense array index.
    /// </summary>
    /// <param name="index">The index in the dense component array.</param>
    /// <returns>A reference to the component.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetByIndex(int index)
    {
        return ref _components[index];
    }

    /// <summary>
    /// Gets the entity ID corresponding to a dense array index.
    /// </summary>
    /// <param name="id">The index in the dense array.</param>
    /// <returns>The entity ID associated with this index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetEntityByIndex(EntityId id)
    {
        return _mapping[id];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGet(EntityId entity, ref T component)
    {
        if (!Has(entity))
            return false;

        component = Get(entity);
        return true;
    }

    private void Resize<TArg>(ref TArg[] array, int index, TArg? defaultValue = default)
    {
        var length = _components.Length;
        var size = Math.Max(index + 1, length * GrowthFactor);
        
        Array.Resize(ref array, size);
        
        if (defaultValue is null)
            return;
        
        Array.Fill(array, defaultValue, length, size - length);
    }
}