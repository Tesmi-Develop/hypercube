namespace Hypercube.Core.Ecs;

[IdStruct(typeof(int))]
public readonly partial struct EntityId
{
    public const int None = -1;
}
