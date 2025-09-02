using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hypercube.Core.Graphics.Rendering.Batching;
using Hypercube.Core.Windowing;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Vectors;
using Silk.NET.OpenGL;
using ShaderType = Hypercube.Core.Graphics.Rendering.Shaders.ShaderType;
using SilkShaderType = Silk.NET.OpenGL.ShaderType;

namespace Hypercube.Core.Graphics.Utilities.Extensions;

[EngineInternal]
public static class SilkGLExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBlend(this GL gl, bool value)
    {
        gl.SetEnableCap(EnableCap.Blend, value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetScissor(this GL gl, bool value)
    {
        gl.SetEnableCap(EnableCap.ScissorTest, value);
    }
    
    public static void Viewport(this GL gl, IWindow window)
    {
        Viewport(gl, window.Size);
    }
    
    [PublicAPI]
    public static void Viewport(this GL gl, Vector2i size)
    {
        gl.Viewport(0, 0, (uint) size.X, (uint) size.Y);
    }
    
    public static uint CreateShader(this GL gl, ShaderType type)
    {
        return gl.CreateShader(ToShaderType(type));
    }
    
    public static unsafe string GetStringExt(this GL gl, StringName name)
    {
        var pointer = gl.GetString(name);
        return Marshal.PtrToStringUTF8((nint) pointer) ?? string.Empty;
    }

    public static void ObjectLabel(this GL gl, ObjectIdentifier identifier, uint handle, string name)
    {
        gl.ObjectLabel(identifier, handle, (uint) name.Length, name);
    }

    public static bool HasErrors(this GL gl)
    {
        return gl.GetError() != (int) ErrorCode.NoError;
    }

    public static void ClearColor(this GL gl, Color color)
    {
        gl.ClearColor(color.R, color.G, color.B, color.A);
    }

    public static unsafe void DrawElements(this GL gl, PrimitiveTopology topology, int count, DrawElementsType type, int indices)
    {
        var pointer = (void*) indices;
        gl.DrawElements(ToPrimitiveType(topology), (uint) count, type, pointer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetEnableCap(this GL gl, EnableCap cap, bool value)
    {
        if (value)
        {
            gl.Enable(cap);
            return;
        }
        
        gl.Disable(cap);
    }

    private static PrimitiveType ToPrimitiveType(PrimitiveTopology primitiveTopology)
    {
        return primitiveTopology switch
        {
            PrimitiveTopology.PointList => PrimitiveType.Points,
            PrimitiveTopology.LineList => PrimitiveType.Lines,
            PrimitiveTopology.LineStrip => PrimitiveType.LineStrip,
            PrimitiveTopology.LineLoop => PrimitiveType.LineLoop,
            PrimitiveTopology.TriangleList => PrimitiveType.Triangles,
            PrimitiveTopology.TriangleFan => PrimitiveType.TriangleFan,
            PrimitiveTopology.TriangleStrip => PrimitiveType.TriangleStrip,
            _ => throw new ArgumentOutOfRangeException(nameof(primitiveTopology), primitiveTopology, null)
        };
    }

    private static SilkShaderType ToShaderType(ShaderType type)
    {
        return type switch
        {
            ShaderType.Vertex => SilkShaderType.VertexShader,
            ShaderType.Fragment => SilkShaderType.FragmentShader,
            ShaderType.Geometry => SilkShaderType.GeometryShader,
            ShaderType.Compute => SilkShaderType.ComputeShader,
            ShaderType.Tessellation => SilkShaderType.TessEvaluationShader,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}