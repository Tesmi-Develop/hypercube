using Hypercube.Core.Graphics.Rendering.Api.Handlers;
using Hypercube.Core.Graphics.Rendering.Api.Settings;
using Hypercube.Core.Graphics.Rendering.Batching;
using Hypercube.Core.Graphics.Rendering.Shaders;
using Hypercube.Core.Viewports;
using Hypercube.Core.Windowing.Api;
using Hypercube.Core.Windowing.Windows;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Matrices;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;
using InitHandler = Hypercube.Core.Graphics.Rendering.Api.Handlers.InitHandler;
using TextureHandle = Hypercube.Core.Graphics.Objects.Texturing.TextureHandle;

namespace Hypercube.Core.Graphics.Rendering.Api.Realisations.Headless;

public sealed class HeadlessRenderingApi : IRenderingApi
{
    public RenderingApi Type => RenderingApi.Headless;
    
#pragma warning disable CS0067
    public event InitHandler? OnInit;
    public event DrawHandler? OnDraw;
    public event DebugInfoHandler? OnDebugInfo;
#pragma warning restore CS0067
    
    public IShaderProgram? PrimitiveShaderProgram => null;
    public IShaderProgram? TexturingShaderProgram => null;

    public int BatchVerticesIndex => 0;
    public int BatchIndicesIndex => 0;
    public int BatchCount => 0;
    public int VerticesCount => 0;

    public HeadlessRenderingApi(RenderingApiSettings settings, IWindowingApi windowingApi)
    {
        _ = settings;
        _ = windowingApi;
    }

    public void Init(IContextInfoProvider context)
    {
    }

    public void Load()
    {
    }

    public void Terminate()
    {
    }

    public void Render(IWindow window)
    {
    }

    public void EnsureBatch(PrimitiveTopology topology, ShaderSetup shaderSetup, uint? texture)
    {
        
    }

    public void EnsureBatch(PrimitiveTopology topology, IShaderProgram shader, uint? texture)
    {
    }

    public void BreakCurrentBatch()
    {
    }

    public void PushVertex(Vertex vertex)
    {
    }

    public void PushIndex(uint start, uint offset)
    {
    }

    public void PushIndex(int start, int index)
    {
    }

    public Surface CreateSurface(Vector2i size)
    {
        return default;
    }

    public Surface CreateSurface(int width, int height)
    {
        return new Surface(default, default, default);
    }

    public void BindSurface(Surface surface)
    {
        
    }

    public void UnbindSurface()
    {
        
    }

    public void ShaderSetup(Action<IShaderProgram> setup)
    {
        
    }

    public IShaderProgram CreateShaderProgram(string source)
    {
        return new ShaderProgram();
    }

    public TextureHandle CreateTexture(int width, int height, int channels, byte[] data)
    {
        return TextureHandle.Zero;
    }

    public void DeleteTexture(TextureHandle handle)
    {
    }

    public void Scissor(bool value)
    {
    }

    public void SetScissorRect(Rect2i rect)
    {
    }

    public void SetRenderState(Matrix4x4 view, Matrix4x4 projection, Surface? surface, BlendMode blendMode, Action<IShaderProgram> setupShader)
    {
        
    }

    public void SetRenderState(Matrix4x4 view, Matrix4x4 projection, Surface? surface, BlendMode blendMode = BlendMode.Alpha)
    {
        
    }

    public void SetRenderState(Matrix4x4 view, Matrix4x4 projection, BlendMode blendMode = BlendMode.Alpha)
    {
    }

    public void SetRenderState(ICameraManager cameraManager, BlendMode blendMode = BlendMode.Alpha)
    {
    }

    public RenderState GetRenderState(RenderStateId id)
    {
        return RenderState.Default;
    }

    public RenderStateId GetCurrentRenderStateId()
    {
        return new RenderStateId(0);
    }

    public RenderState GetCurrentRenderState()
    {
        return RenderState.Default;
    }

    public void SetRenderView(Matrix4x4 view)
    {
    }

    public void SetRenderViewProjection(Matrix4x4 view, Matrix4x4 projection)
    {
        
    }

    public void SetRenderBlendMode(BlendMode blendMode)
    {
        
    }

    public void SetRenderProjection(Matrix4x4 projection)
    {
    }

    private sealed class ShaderProgram : BaseShaderProgram
    {
        public ShaderProgram() : base(0)
        {
        }

        public override void SetUniform(string name, int value)
        {
        }

        public override void SetUniform(string name, float value)
        {
        }

        public override void SetUniform(string name, double value)
        {
        }

        public override void SetUniform(string name, Vector2 value)
        {
        }

        public override void SetUniform(string name, Vector2i value)
        {
        }

        public override void SetUniform(string name, Vector3 value)
        {
        }

        public override void SetUniform(string name, Vector3i value)
        {
        }

        public override void SetUniform(string name, Vector4 value)
        {
        }

        public override void SetUniform(string name, Matrix3x3 value, bool transpose = false)
        {
        }

        public override void SetUniform(string name, Matrix4x4 value, bool transpose = false)
        {
        }

        protected override void InternalUseProgram(ShaderProgramHandle handle)
        {
        }

        protected override void InternalLabel(string name)
        {
        }

        protected override void InternalDelete(ShaderProgramHandle handle)
        {
        }

        public override void SetUniform(string name, Color value)
        {
        }
    }
}