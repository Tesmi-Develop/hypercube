namespace Hypercube.Core.UI.Elements.Containers.Abstract;

public abstract class Container : Element
{
    public override PositioningMode ChildrenPositioning => PositioningMode.Layout;
}
