using Hypercube.Core.Resources.FileSystems;

namespace Hypercube.Core.Resources.Loaders;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IResourceLoader
{
    string[] Extensions { get; }
    Type ResourceType { get; }
    
    public bool CanLoad(ResourcePath path, IFileSystem fileSystem, ResourceLoadArg[] args);
    public Resource Load(ResourcePath path, IFileSystem fileSystem, ResourceLoadArg[] args);
}