using Hypercube.Core.Graphics.Rendering.Shaders;

namespace Hypercube.Core.Graphics.Rendering.Api;

public record struct ShaderSetup
{
    public IShaderProgram ShaderProgram;
    public Action<IShaderProgram>? Setup;
}