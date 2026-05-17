using System.Runtime.CompilerServices;
using Hypercube.Physics.Manifolds;
using Hypercube.Physics.Mathematics;
using Hypercube.Physics.Shapes;
using Hypercube.Physics.Utilities;
using JetBrains.Annotations;

namespace Hypercube.Physics.Collision;

[PublicAPI]
public static partial class Contacts
{
    private static readonly ResolveRegister[,] Resolvers = new ResolveRegister[Constants.ShapeTypeCount, Constants.ShapeTypeCount];

    public static void Initialize()
    {
        Add(CollideCircles, ShapeType.Circle, ShapeType.Circle);
        Add(CollidePolygons, ShapeType.Polygon, ShapeType.Polygon);
        Add(CollidePolygonCircle, ShapeType.Polygon, ShapeType.Circle);
    }
    
    public static Manifold Resolve(
        in ShapeUnion shapeA, ShapeType typeA, in Transform transformA,
        in ShapeUnion shapeB, ShapeType typeB, in Transform transformB)
    {
        DebugPhysicsGuard.ValidateShapeType(typeA);
        DebugPhysicsGuard.ValidateShapeType(typeB);
        
        var register = Resolvers[(int) typeA, (int) typeB];
        
        var function = register.Function;
        if (function is null)
            return Manifold.Empty; // Note: May be use guard?
        
        return register.Primary
            ? function(shapeA, transformA, shapeB, transformB)
            : function(shapeB, transformB, shapeA, transformB);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Manifold Resolve(
        in ShapeUnionTyped shapeA, in Transform transformA,
        in ShapeUnionTyped shapeB, in Transform transformB) =>
        Resolve(
            shapeA.Shape, shapeA.Type, transformA,
            shapeB.Shape, shapeB.Type, transformB
        );

    public static void Add(ManifoldResolver function, ShapeType shapeA, ShapeType shapeB)
    {
        DebugPhysicsGuard.ValidateShapeType(shapeA);
        DebugPhysicsGuard.ValidateShapeType(shapeB);
        
        Resolvers[(int) shapeA, (int) shapeB].Function = function;
        Resolvers[(int) shapeA, (int) shapeB].Primary = true;
        
        if (shapeA == shapeB)
            return;
        
        Resolvers[(int) shapeB, (int) shapeA].Function = function;
        Resolvers[(int) shapeB, (int) shapeA].Primary = false;
    }
    
    private struct ResolveRegister(ManifoldResolver function, bool primary)
    {
        public ManifoldResolver? Function = function;
        public bool Primary = primary;
    }
}
