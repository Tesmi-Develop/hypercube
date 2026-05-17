using Hypercube.Physics.Shapes;

namespace Hypercube.Physics;

public static class Constants
{
    public const int MaxWorlds = 128;
    public const int WorldsAllocation = 1;
    
    public const float UnitsPerMeter = 1.0f;
    public const float LinearSlop = 0.005f * UnitsPerMeter;
    public const float SpeculativeDistance = 4.0f * LinearSlop;
    public const float Epsilon = 1.1920929e-7f;
    
    public const int ShapeTypeCount = (int) (ShapeType.Count - 1);
}
