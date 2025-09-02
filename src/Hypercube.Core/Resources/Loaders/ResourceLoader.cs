using Hypercube.Core.Resources.FileSystems;

namespace Hypercube.Core.Resources.Loaders;

public abstract class ResourceLoader<T> : IResourceLoader where T : Resource
{
    public abstract string[] Extensions { get; }
    public virtual bool SupportLoadArgs => false;
    public Type ResourceType => typeof(T);
        
    public abstract bool CanLoad(ResourcePath path, IFileSystem fileSystem);
    public abstract T Load(ResourcePath path, IFileSystem fileSystem);

    public virtual T Load(ResourcePath path, IFileSystem fileSystem, ResourceLoadArg[] args)
    {
        throw new NotImplementedException();
    }

    bool IResourceLoader.CanLoad(ResourcePath path, IFileSystem fileSystem, ResourceLoadArg[] args)
    {
        return CanLoad(path, fileSystem);
    }

    Resource IResourceLoader.Load(ResourcePath path, IFileSystem fileSystem, ResourceLoadArg[] args)
    {
        if (args.Length == 0 || !SupportLoadArgs)
            return Load(path, fileSystem);

        return Load(path, fileSystem, args);
    }
}