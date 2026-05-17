using System.Diagnostics;
using Hypercube.Core.Execution.LifeCycle;
using Hypercube.Core.Graphics.Rendering.Context;
using Hypercube.Core.UI.Manager;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Dimensions;

namespace Hypercube.Core.UI.Elements;

/// <summary>
/// Represents a base UI element in the hierarchy system.
/// Provides layouting, rendering, update lifecycle, and child management.
/// </summary>
/// <seealso cref="IUIManager"/>
[PublicAPI]
public class Element : IDisposable
{
    /// <summary>
    /// UI manager associated with this element.
    /// </summary>
    public IUIManager UI
    {
        get => _ui ?? throw new InvalidOperationException();
        internal set
        {
            if (_ui is not null)
                throw new InvalidOperationException();

            _ui = value;
        }
    }

    /// <summary>
    /// Parent element in the UI hierarchy.
    /// </summary>
    public Element? Parent
    {
        get;
        internal set;
    }

    /// <summary>
    /// Indicates whether the element has been initialized via startup.
    /// </summary>
    public bool Started
    {
        get;
        internal set;
    }

    /// <summary>
    /// Local position of the element.
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>. <br/>
    /// Only used in <see cref="PositioningMode.Manual"/>.
    /// </remarks>
    public HDim2 Position
    {
        get;
        set
        {
            if (LayoutMode)
                return;

            field = value;
            UpdateLayout();
        }
    } = HDim2.Zero;

    /// <summary>
    /// Local size of the element.
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>.
    /// </remarks>
    public HDim2 Size
    {
        get;
        set
        {
            field = value;
            UpdateLayout();
        }
    } =  HDim2.Zero;

    /// <summary>
    /// Local rotation of the element.
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>.
    /// </remarks>
    public HDim Rotation
    {
        get;
        set
        {
            field = value;
            UpdateLayout();
        }
    }
    
    /// <summary>
    /// Padding applied inside the element bounds.
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>.
    /// </remarks>
    public HDimRect Padding
    {
        get;
        set
        {
            field = value;
            UpdateLayout();
        }
    }

    /// <summary>
    /// Anchor point used for positioning (0..1 normalized space).
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>.
    /// </remarks>
    public Vector2 AnchorPoint
    {
        get;
        set
        {
            Debug.Assert(value.X is >= 0 and <= 1);
            Debug.Assert(value.Y is >= 0 and <= 1);
            field = value;
            UpdateLayout();
        }
    }

    /// <summary>
    /// Z-index used for rendering order.
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>.
    /// </remarks>
    public int ZIndex
    {
        get;
        set
        {
            field = value;
            UpdateLayout();
        }
    }

    /// <summary>
    /// Order used for layout processing among siblings.
    /// </summary>
    /// <remarks>
    /// Raise <see cref="UpdateLayout"/>. <br/>
    /// Only used in <see cref="PositioningMode.Layout"/>.
    /// </remarks>
    public int LayoutOrder
    {
        get;
        set
        {
            field = value;
            UpdateLayout();
        }
    }

    /// <summary>
    /// Position computed by layout system.
    /// </summary>
    public HDim2 LayoutPosition;

    /// <summary>
    /// Indicates whether the element is visible.
    /// </summary>
    public bool Visible = true;

    public bool Scissor;

    /// <summary>
    /// Position relative to parent element.
    /// </summary>
    public Vector2 RelativePosition 
    {
        get;
        private set;
    }

    /// <summary>
    /// Rotation relative to parent element.
    /// </summary>
    public Angle RelativeRotation
    {
        get;
        private set;
    }

    /// <summary>
    /// Absolute screen position.
    /// </summary>
    public Vector2 AbsolutePosition
    {
        get;
        private set;
    }

    /// <summary>
    /// Absolute size in screen space.
    /// </summary>
    public Vector2 AbsoluteSize
    {
        get;
        private set;
    }

    /// <summary>
    /// Absolute rotation in screen space.
    /// </summary>
    public Angle AbsoluteRotation
    {
        get;
        private set;
    }

    /// <summary>
    /// Position of the content area (inside padding).
    /// </summary>
    public Vector2 ContentPosition
    {
        get;
        private set;
    }

    /// <summary>
    /// Size of the content area (inside padding).
    /// </summary>
    public Vector2 ContentSize
    {
        get;
        private set;
    }

    private bool _disposed;
    
    private IUIManager? _ui;

    private readonly List<Element> _children = [];

    /// <summary>
    /// Defines how child elements are positioned.
    /// </summary>
    private PositioningMode _positioning = PositioningMode.Manual;

    /// <summary>
    /// Defines how child elements are positioned.
    /// </summary>
    public virtual PositioningMode ChildrenPositioning => PositioningMode.Manual;
    
    /// <summary>
    /// Resolves the current position depending on layout mode.
    /// </summary>
    public HDim2 ResolvedPosition => _positioning switch
    {
        PositioningMode.Manual => Position,
        PositioningMode.Layout => LayoutPosition,
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    /// Indicates whether <see cref="PositioningMode.Layout"/> is enabled.
    /// </summary>
    /// <seealso cref="PositioningMode"/>
    public bool LayoutMode => _positioning == PositioningMode.Layout;

    /// <summary>
    /// Read-only collection of child elements.
    /// </summary>
    public IReadOnlyList<Element> Children => _children;
    
    public bool Attached => _ui is not null;

    /// <summary>
    /// Finalizer ensures unmanaged cleanup if Dispose was not called.
    /// </summary>
    ~Element()
    {
        Dispose(false);
    }

    /// <summary>
    /// Renders this element and all its children.
    /// </summary>
    /// <seealso cref="OnRender"/>
    public void Render(IRenderContext renderer, UIDrawPayload payload)
    {
        if (!Visible)
            return;
        
        OnRender(renderer, payload);

        foreach (var element in _children)
            element.Render(renderer, payload);
    }
    
    /// <summary>
    /// Updates this element and all its children.
    /// </summary>
    /// <seealso cref="OnUpdate"/>
    public void Update(FrameEventArgs args)
    {
        OnUpdate(args);
        
        foreach (var element in _children)
            element.Update(args);
    }

    /// <summary>
    /// Disposes the element and releases resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Internal dispose method.
    /// </summary>
    protected void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        
        if (disposing)
            OnDisposeManaged();
        
        OnDisposeUnmanaged();
        _disposed = true;
    }
    
    /// <summary>
    /// Adds a child element to this element.
    /// </summary>
    public T AddChild<T>(T element) where T : Element
    {
        // Main header of element
        element.UI = UI;
        element.Parent = this;

        // Config local parameters
        element._positioning = ChildrenPositioning;
        
        OnChildAdded(element);
        
        _children.Add(element);
        
        element.OnStartup();
        element.Started = true;
        
        UpdateLayout();

        return element;
    }

    /// <summary>
    /// Removes a child element.
    /// </summary>
    public T RemoveChild<T>(T element) where T : Element
    {
        OnChildRemove(element);
        
        element.Parent = null!;
        
        _children.Remove(element);
        
        UpdateLayout();

        return element;
    }

    /// <summary>
    /// Removes a child element at the specified index.
    /// </summary>
    public void RemoveChildAtIndex(int index)
    {
        if (_children.Count <= index)
            return;
            
        RemoveChild(_children[index]);
    }
    
    /// <summary>
    /// Recalculates layout for this element and all children.
    /// </summary>
    public void UpdateLayout()
    {
        if (!Started)
            return;

        RelativePosition = Vector2.Zero;

        AbsoluteSize = Size.Resolve(UI.ViewportSize);
        AbsolutePosition = ResolvedPosition.Resolve(UI.ViewportSize) - AbsoluteSize * AnchorPoint;

        if (Parent is not null)
        {
            RelativePosition = ResolvedPosition.Resolve(Parent.ContentSize);

            AbsoluteSize = Size.Resolve(Parent.ContentSize);
            AbsolutePosition = Parent.ContentPosition + RelativePosition - AbsoluteSize * AnchorPoint;
        }

        var padding = Padding.Resolve(AbsoluteSize);
        
        ContentPosition = AbsolutePosition + padding.TopLeft;
        ContentSize = new Vector2(
            AbsoluteSize.X - padding.Left - padding.Right,
            AbsoluteSize.Y - padding.Top - padding.Bottom
        );
        
        OnUpdateLayout();

        foreach (var child in _children)
            child.UpdateLayout();
    }

    /// <summary>
    /// Called when the element starts.
    /// </summary>
    protected virtual void OnStartup()
    {
    }

    /// <summary>
    /// Called during rendering.
    /// </summary>
    protected virtual void OnRender(IRenderContext renderer, UIDrawPayload payload)
    {
    }

    /// <summary>
    /// Called during update tick.
    /// </summary>
    protected virtual void OnUpdate(FrameEventArgs args)
    {
    }
    
    /// <summary>
    /// Called when a child element is added.
    /// </summary>
    protected virtual void OnChildAdded(Element element)
    {
    }
    
    /// <summary>
    /// Called when a child element is removed.
    /// </summary>
    protected virtual void OnChildRemove(Element element)
    {
    }

    /// <summary>
    /// Called after layout is recalculated.
    /// </summary>
    protected virtual void OnUpdateLayout()
    {
    }

    /// <summary>
    /// Called when managed resources are disposed.
    /// </summary>
    protected virtual void OnDisposeManaged()
    {
    }

    /// <summary>
    /// Called when unmanaged resources are disposed.
    /// </summary>
    protected virtual void OnDisposeUnmanaged()
    {
    }
}
