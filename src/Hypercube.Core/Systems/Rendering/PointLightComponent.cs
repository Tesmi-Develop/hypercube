using Hypercube.Core.Ecs;
using Hypercube.Core.Ecs.Attributes;
using Hypercube.Mathematics;

namespace Hypercube.Core.Systems.Rendering;

public class PointLightComponent : Component
{
    public float Radius;
    public float Intensity;
    public Color Color = Color.White;
}