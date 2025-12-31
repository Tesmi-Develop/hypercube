using Hypercube.Core.Ecs;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;
using Hypercube.Core.Serialization;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Quaternions;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Systems.Rendering;

public struct ModelComponent() : IComponent
{
    public Model? Model = null;

    [DataField("model")]
    public ResourcePath Path = ResourcePath.Empty;

    [DataField]
    public Quaternion Rotation = Quaternion.FromEuler(Vector3.Zero);

    [DataField]
    public Vector3 Scale = Vector3.One;
    
    [DataField]
    public Color Color = Color.White;
}