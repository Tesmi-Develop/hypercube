using Hypercube.Core.Ecs;
using Hypercube.Mathematics;

namespace Hypercube.Core.Systems.Rendering;

[PublicAPI]
public struct PointLightComponent() : IComponent
{
    public float Radius;
    public float Intensity;
    public Color Color = Color.White;
}
