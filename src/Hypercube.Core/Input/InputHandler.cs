using Hypercube.Core.Execution;
using Hypercube.Core.Windowing.Api;
using Hypercube.Core.Windowing.Manager;
using Hypercube.Utilities.Debugging.Logger;
using Hypercube.Utilities.Dependencies;
using Hypercube.Utilities.Extensions;

namespace Hypercube.Core.Input;

/// <summary>
/// A class that directly listens to window events and processes them to work correctly,
/// pre first level work with push handling, lower level would be <see cref="IWindowingApi"/>
/// which takes data directly from the current window handling API.
///
/// In most cases it is better to work with <see cref="IInputManager"/>,
/// which is an add-on on top of the current implementation.
/// </summary>
[UsedImplicitly]
public sealed class InputHandler : IInputHandler, IPostInject
{
    [Dependency] private readonly IWindowManager _window = default!;
    [Dependency] private readonly IRuntimeLoop _runtimeLoop = default!;
    [Dependency] private readonly ILogger _logger = default!;
    
    private readonly Dictionary<nint, Keys> _key = new();
    
    private IWindowingApi WindowingApi => _window.Api;
    
    public void PostInject()
    {
       _runtimeLoop.Actions.Add(OnUpdate, (int) EngineUpdatePriority.InputHandler); 
        
        WindowingApi.OnWindowKey += OnKeyUpdate;
        WindowingApi.OnWindowMouseButton += OnMouseButtonUpdate;
    }

    public void Clear()
    {
        foreach (var (_, keys) in _key)
        {
            keys.Pressed.Clear();
            keys.Released.Clear();
        }
    }
    
    public bool IsKeyState(nint window, Key key, KeyState state)
    {
        return _key.TryGetValue(window, out var keys) && keys[state].Contains(key);
    }
    
    public bool IsKeyHeld(nint window, Key key)
    {
        return IsKeyState(window, key, KeyState.Held);
    }

    public bool IsKeyPressed(nint window, Key key)
    {
        return IsKeyState(window, key, KeyState.Pressed);
    }

    public bool IsKeyReleased(nint window, Key key)
    {
        return IsKeyState(window, key, KeyState.Released);
    }

    public bool IsKeyState(Key key, KeyState state)
    {
        return IsKeyState(WindowingApi.ContextCurrent, key, state);
    }

    public bool IsKeyHeld(Key key)
    {
        return IsKeyState(key, KeyState.Held);
    }

    public bool IsKeyPressed(Key key)
    {
        return IsKeyState(key, KeyState.Pressed);
    }

    public bool IsKeyReleased(Key key)
    {
        return IsKeyState(key, KeyState.Pressed);
    }

    public void Simulate(KeyStateChangedArgs state)
    {
        Simulate(WindowingApi.ContextCurrent, state);
    }

    public void Simulate(nint window, KeyStateChangedArgs state)
    {
        OnKeyUpdate(window, state);
    }

    private void OnUpdate(FrameEventArgs args)
    {
        Clear();
    }
    
    private void OnKeyUpdate(nint window, KeyStateChangedArgs state)
    {
        var keys = _key.GetOrInstantiate(window);
        switch (state.State)
        {
            // Legacy shit, maybe will eat many ram and cpu
            // We made many shit because fucking Key rollover: https://en.wikipedia.org/wiki/Key_rollover
            case KeyState.Pressed:
                keys.Held.Add(state.Key);
                keys.Pressed.Add(state.Key);
                break;
            
            case KeyState.Released:
                keys.Held.Remove(state.Key);
                keys.Released.Add(state.Key);
                break;
            
            case KeyState.Held:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void OnMouseButtonUpdate(nint window, MouseButtonChangedArgs state)
    {
        
    }
    
    private class Keys
    {
        public readonly List<Key> Released = [];
        public readonly List<Key> Pressed = [];
        public readonly List<Key> Held = [];

        public List<Key> this[KeyState state] =>
            state switch
            {
                KeyState.Released => Released,
                KeyState.Pressed => Pressed,
                KeyState.Held => Held,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
    }
}