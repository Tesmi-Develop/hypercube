using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hypercube.Core.Ecs.Core;

namespace Hypercube.Core.Ecs;

[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Entity : IDisposable, IEquatable<Entity>
{
    public readonly EntityId Id;
    public readonly int WorldId;

    public Entity(EntityId id, int worldId)
    {
        Id = id;
        WorldId = worldId;
    }

    public Entity(EntityId id, World world) : this(id, world.Id)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        
    }

    public bool Equals(Entity other)
    {
        return Id == other.Id && WorldId == other.WorldId;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, WorldId);
    }
    
    public override string ToString()
    {
        return $"Entity {WorldId}:{Id}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Entity left, Entity right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Entity left, Entity right)
    {
        return !left.Equals(right);
    }
}