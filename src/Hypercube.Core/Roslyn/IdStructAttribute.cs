namespace Hypercube.Core.Roslyn;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class IdStructAttribute : Attribute
{
    [UsedImplicitly]
    public readonly Type UnderlyingType;

    public IdStructAttribute(Type underlyingType)
    {
        UnderlyingType = underlyingType;
    }
}
