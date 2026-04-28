using Hypercube.Mathematics;
using Hypercube.Mathematics.Matrices;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Rendering.Shaders;

/// <summary>
/// Represents a compiled and linked GPU shader program used for rendering.
/// Provides methods to bind the program and set uniform variables.
/// </summary>
public interface IShaderProgram : IDisposable
{
    /// <summary>
    /// Gets the handle for this shader program.
    /// </summary>
    ShaderProgramHandle Handle { get; }
    
    /// <summary>
    /// Activates this shader program for subsequent rendering operations.
    /// </summary>
    void Use();

    /// <summary>
    /// Deactivates the currently active shader program.
    /// </summary>
    void Stop();
    
    /// <summary>
    /// Sets an integer uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The integer value to assign.</param>
    void SetUniform(string name, int value);

    /// <summary>
    /// Sets a float uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The float value to assign.</param>
    void SetUniform(string name, float value);

    /// <summary>
    /// Sets a double uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The double value to assign.</param>
    void SetUniform(string name, double value);

    /// <summary>
    /// Sets a 2D vector uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The vector value to assign.</param>
    void SetUniform(string name, Vector2 value);

    /// <summary>
    /// Sets a 2D integer vector uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The vector value to assign.</param>
    void SetUniform(string name, Vector2i value);

    /// <summary>
    /// Sets a 3D vector uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The vector value to assign.</param>
    void SetUniform(string name, Vector3 value);

    /// <summary>
    /// Sets a 3D integer vector uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The vector value to assign.</param>
    void SetUniform(string name, Vector3i value);

    /// <summary>
    /// Sets a 4D vector uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The vector value to assign.</param>
    void SetUniform(string name, Vector4 value);

    /// <summary>
    /// Sets a 3x3 matrix uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The matrix value to assign.</param>
    /// <param name="transpose">
    /// Indicates whether the matrix should be transposed before uploading.
    /// </param>
    void SetUniform(string name, Matrix3x3 value, bool transpose = false);

    /// <summary>
    /// Sets a 4x4 matrix uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The matrix value to assign.</param>
    /// <param name="transpose">
    /// Indicates whether the matrix should be transposed before uploading.
    /// </param>
    void SetUniform(string name, Matrix4x4 value, bool transpose = false);
    
    void SetUniform(string name, Matrix4x4[] value, bool transpose = false);

    /// <summary>
    /// Sets a color uniform value.
    /// </summary>
    /// <param name="name">The name of the uniform variable.</param>
    /// <param name="value">The color value to assign.</param>
    void SetUniform(string name, Color value);

    /// <summary>
    /// Assigns a debug label to the shader program, if supported by the underlying API.
    /// </summary>
    /// <param name="name">The label to assign.</param>
    void Label(string name);
}
