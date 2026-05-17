using Hypercube.Core.Ecs;
using Hypercube.Core.Graphics.Rendering.Manager;
using Hypercube.Core.Input.Handler;
using Hypercube.Core.Input.Manager;
using Hypercube.Core.UI;
using Hypercube.Core.UI.Manager;
using Hypercube.Utilities.Collections;

namespace Hypercube.Core.Execution.LifeCycle;

public enum EngineUpdatePriority
{
    #region Input
    
    /// <see cref="IInputHandler"/>
    InputHandler = 100,
        
    /// <see cref="IRenderManager"/>
    RendererUpdate = 200,
    
    /// <see cref="IInputManager"/>
    InputManager = 300,
    
    #endregion
    
    /// <see cref="IEntitySystemManager"/>
    EntitySystemUpdate = 400,
    
    /// <see cref="IUIManager"/>
    UIUpdate = 500,
    
    /// <see cref="IRenderManager"/>
    RendererRender = 600
}

public static class EngineUpdatePriorityExtensions
{
    public static void Add<T>(this ISubscribableOrderedActions<T> actions, Action<T> action, EngineUpdatePriority priority)
    {
        actions.Add(action, (int) priority);
    }
}
