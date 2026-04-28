using System.Diagnostics;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Matrices;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Rendering.Shaders;

[DebuggerDisplay("ShaderProgram {Handle}")]
public abstract partial class BaseShaderProgram : IShaderProgram
{
    public ShaderProgramHandle Handle { get; }

    protected BaseShaderProgram(ShaderProgramHandle handle)
    {
        Handle = handle;
    }

    public void Use()
    {
        InternalUseProgram(Handle);
    }

    public void Stop()
    {
        InternalUseProgram(0);
    }

    public abstract void SetUniform(string name, int value);
    public abstract void SetUniform(string name, float value);
    public abstract void SetUniform(string name, double value);
    public abstract void SetUniform(string name, Vector2 value);
    public abstract void SetUniform(string name, Vector2i value);
    public abstract void SetUniform(string name, Vector3 value);
    public abstract void SetUniform(string name, Color value);
    public abstract void SetUniform(string name, Vector3i value);
    public abstract void SetUniform(string name, Vector4 value);
    public abstract void SetUniform(string name, Matrix3x3 value, bool transpose = false);
    public abstract void SetUniform(string name, Matrix4x4 value, bool transpose = false);
    public abstract void SetUniform(string name, Matrix4x4[] value, bool transpose = false);
    
    public void Label(string name)
    {
        InternalLabel(name);
    }

    public void Dispose()
    {
        InternalDelete(Handle);
    }
}