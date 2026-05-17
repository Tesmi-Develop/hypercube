using Hypercube.Core.UI.Elements.Containers.Abstract;
using Hypercube.Mathematics.Dimensions;

namespace Hypercube.Core.UI.Elements.Containers;

[PublicAPI]
public class LayoutContainer : Container
{
    public Orientation Orientation = Orientation.Vertical;
    
    public Alignment HAlignment = Alignment.Start;
    
    public Alignment VAlignment = Alignment.Start;
    
    public Direction Direction = Direction.Forward;
    
    public HDim Spacing = HDim.Zero;

    protected float GetPrimarySize(Vector2 size) =>
        Orientation == Orientation.Horizontal ? size.X : size.Y;

    protected float GetSecondarySize(Vector2 size) =>
        Orientation == Orientation.Horizontal ? size.Y : size.X;

    protected Vector2 SetPrimary(Vector2 pos, float value) =>
        Orientation == Orientation.Horizontal ? new Vector2(value, pos.Y) : new Vector2(pos.X, value);
    
    protected Vector2 SetSecondary(Vector2 pos, float value) =>
        Orientation == Orientation.Horizontal ? new Vector2(pos.X, value) : new Vector2(value, pos.Y);

    protected float GetContentPrimarySize(Vector2 contentSize) =>
        Orientation == Orientation.Horizontal ? contentSize.X : contentSize.Y;

    protected float ResolveSpacing() => Spacing.Resolve(GetContentPrimarySize(ContentSize));

    protected override void OnUpdateLayout()
    {
        base.OnUpdateLayout();
        
        var children = Children.OrderBy(x => x.LayoutOrder).ToArray();
        if (children.Length == 0)
            return;

        var spacing = Spacing.Resolve(GetContentPrimarySize(ContentSize));
        var totalPrimary = children.Sum(c => GetPrimarySize(c.Size.Resolve(ContentSize)));

        totalPrimary += spacing * (children.Length - 1);
        
        var start = CalculateStartOffset(totalPrimary, spacing, children.Length);

        Layout(children, start, spacing);
    }

    private float CalculateStartOffset(float total, float spacing, int count)
    {
        var content = GetContentPrimarySize(ContentSize);

        return VAlignment switch
        {
            Alignment.Start => 0f,
            Alignment.Center => (content - total) / 2f,
            Alignment.End => content - total,
            _ => 0f
        };
    }

    private void Layout(Element[] children, float start, float spacing)
    {
        var ordered = Direction == Direction.Forward
            ? children
            : children.Reverse().ToArray();

        var dynamicSpacing = spacing;
        var totalPrimary = ordered.Sum(c =>
            GetPrimarySize(c.Size.Resolve(ContentSize)));

        switch (VAlignment)
        {
            case Alignment.SpaceBetween:
                if (ordered.Length - 1 <= 0)
                    break;
                
                dynamicSpacing = (GetContentPrimarySize(ContentSize) - totalPrimary) / (ordered.Length - 1);
                break;

            case Alignment.SpaceEvenly:
                dynamicSpacing = (GetContentPrimarySize(ContentSize) - totalPrimary) / (ordered.Length + 1);
                start = dynamicSpacing;
                break;

            case Alignment.SpaceAround:
                dynamicSpacing = (GetContentPrimarySize(ContentSize) - totalPrimary) / ordered.Length;
                start = dynamicSpacing / 2f;
                break;
            
            case Alignment.Start:
            case Alignment.Center:
            case Alignment.End:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        var offset = start;

        foreach (var child in ordered)
        {
            var size = child.Size.Resolve(ContentSize);
            var secondary = HAlignment switch
            {
                Alignment.Start => 0f,
                Alignment.Center => (GetSecondarySize(ContentSize) - GetSecondarySize(size)) / 2f,
                Alignment.End => GetSecondarySize(ContentSize) - GetSecondarySize(size),
                _ => 0f
            };

            var pos = Vector2.Zero;
            pos = SetPrimary(pos, offset);
            pos = SetSecondary(pos, secondary);

            child.LayoutPosition = new HDim2(
                new HDim(0f, pos.X),
                new HDim(0f, pos.Y)
            );

            offset += GetPrimarySize(size) + dynamicSpacing;
        }
    }
}
