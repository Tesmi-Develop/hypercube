namespace Hypercube.Core.Resources;

public readonly struct ResourceLoadArg
{
    public readonly string Key;
    public readonly object Value;

    public ResourceLoadArg(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public static implicit operator ResourceLoadArg((string key, object value) arg)
    {
        return new ResourceLoadArg(arg.key, arg.value);
    }
}