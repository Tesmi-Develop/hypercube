using Hypercube.Physics.Mathematics;
using Hypercube.Physics.Shapes;

namespace Hypercube.Physics.Manifolds;

public delegate Manifold ManifoldResolver(
    in ShapeUnion shapeA,
    in Transform transformA,
    in ShapeUnion shapeB,
    in Transform transformB
);
