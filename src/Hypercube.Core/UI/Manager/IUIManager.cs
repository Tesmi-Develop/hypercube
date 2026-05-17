using Hypercube.Core.Input.Handler;
using Hypercube.Core.Resources;
using Hypercube.Core.UI.Elements;
using Hypercube.Utilities.Dependencies;

namespace Hypercube.Core.UI.Manager;

public interface IUIManager
{
    IDependenciesContainer DependenciesContainer { get; }
    
    IResourceManager ResourceManager { get; }
    IInputHandler InputHandler { get; }
    
    UIRoot Root { get; }

    Vector2i MousePosition { get; }
    Vector2i ViewportSize { get; }

    void AddElement(Element element);

    void RemoveElement(Element element);
}