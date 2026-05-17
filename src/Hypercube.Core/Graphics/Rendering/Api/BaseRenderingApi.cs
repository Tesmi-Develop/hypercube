using Hypercube.Core.Graphics.Rendering.Api.Components;
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
using InitHandler = Hypercube.Core.Graphics.Rendering.Api.Handlers.InitHandler;

namespace Hypercube.Core.Graphics.Rendering.Api;

public abstract partial class BaseRenderingApi : IRenderingApi
{
    public abstract RenderingApi Type { get; }
    
    public event InitHandler? OnInit;
    public abstract event DrawHandler? OnDraw;
    public abstract event DebugInfoHandler? OnDebugInfo;

    public IShaderProgram? PrimitiveShaderProgram { get; protected set; }
    public IShaderProgram? TexturingShaderProgram { get; protected set; }
    
    [PublicAPI] public int BatchVerticesIndex { get; protected set; }
    [PublicAPI] public int BatchIndicesIndex { get; protected set; }
    
    [PublicAPI] protected readonly IWindowingApi WindowingApi;
    
    [PublicAPI] protected readonly List<Batch> Batches = [];
    
    protected readonly Vertex[] BatchVertices;
    protected readonly uint[] BatchIndices;
    
    private BatchData? _currentBatchData;
    private readonly List<RenderState> _renderStates = [];
    private RenderStateId _currentRenderStateId;

    protected Color ClearColor { get; private set; }

    public int BatchCount { get; private set; }
    public int VerticesCount { get; private set; }

    protected BaseRenderingApi(RenderingApiSettings settings, IWindowingApi windowingApi)
    {
        ClearColor = settings.ClearColor;
        BatchVertices = new Vertex[settings.MaxVertices];
        BatchIndices = new uint[settings.MaxIndices];
        WindowingApi = windowingApi;
        
        // Инициализируем дефолтным состоянием
        _renderStates.Add(RenderState.Default);
        _currentRenderStateId = new RenderStateId(0);
    }

    public void Init(IContextInfoProvider context)
    {
        if (!InternalInit(context))
            throw new Exception();
        
        OnInit?.Invoke(InternalInfo);
    }

    public void Load()
    {
        InternalLoad();
    }

    public void Terminate()
    {
        InternalTerminate();
    }

    public abstract void Render(IWindow window);

    public void EnsureBatch(PrimitiveTopology topology, uint shader, uint? texture)
    {
        EnsureBatch(topology, shader, texture, _currentRenderStateId);
    }
    
    public void EnsureBatch(PrimitiveTopology topology, uint shader, uint? texture, RenderStateId renderStateId)
    {
        if (_currentBatchData is not null)
        {
            // It's just similar batch,
            // we need changing nothing to render different things
            if (_currentBatchData.Value.Equals(topology, texture, shader, renderStateId))
                return;

            // Creating a real batch
            GenerateBatch();
        }
        
        _currentBatchData = new BatchData(BatchIndicesIndex, texture, shader, topology, renderStateId);
    }

    public void BreakCurrentBatch()
    {
        if (_currentBatchData is null)
            return;

        GenerateBatch();
        _currentBatchData = null;
    }
    
    public void PushVertex(Vertex vertex)
    {
        // TODO: Add clamping and warning for index
        BatchVertices[BatchVerticesIndex++] = vertex;
    }
    
    public void PushIndex(uint start,uint offset)
    {
        // TODO: Add clamping and warning for index
        BatchIndices[BatchIndicesIndex++] = start + offset;
    }
    
    public void PushIndex(int start, int index)
    {
        PushIndex((uint) start, (uint) index);
    }
    
    public IShaderProgram CreateShaderProgram(string source)
    {
        var sections = RenderingApiShaderLoader.ParseSections(source);
        var shaders = new IShader[sections.Count];

        var index = 0;
        foreach (var section in sections)
        {
            shaders[index++] = InternalCreateShader(section.Source, section.Type);
        }
        
        return InternalCreateShaderProgram(shaders);
    }

    public abstract void Scissor(bool value);
    public abstract void SetScissorRect(Rect2i rect);

    protected void UpdateBatchCount()
    {
        BatchCount = Batches.Count;
        VerticesCount = BatchVerticesIndex;
    }
    
    protected void Clear()
    {
        // TODO: optimize
        Array.Clear(BatchVertices, 0, BatchVerticesIndex);
        Array.Clear(BatchIndices, 0, BatchIndicesIndex);

        BatchVerticesIndex = 0;
        BatchIndicesIndex = 0;

        _currentBatchData = null;
        Batches.Clear();
        _renderStates.Clear();
        _renderStates.Add(RenderState.Default);
        _currentRenderStateId = new RenderStateId(0);
    }
    
    private void GenerateBatch()
    {
        if (_currentBatchData is null)
            throw new NullReferenceException();

        var data = _currentBatchData.Value;
        var currentIndex = BatchIndicesIndex;

        var batch = new Batch(
            data.Start,
            currentIndex - data.Start,
            data.Texture,
            data.PrimitiveTopology,
            Matrix4x4.Identity,
            WindowingApi.Context,
            data.RenderStateId
        );

        Batches.Add(batch);
    }
    
    public RenderStateId RegisterRenderState(RenderState state)
    {
        var id = new RenderStateId(_renderStates.Count);
        _renderStates.Add(state);
        return id;
    }
    
    public void SetRenderState(Matrix4x4 view, Matrix4x4 projection)
    {
        var newState = new RenderState(view, projection);
        
        var existingId = _renderStates.IndexOf(newState);
        if (existingId >= 0)
        {
            _currentRenderStateId = new RenderStateId(existingId);
            return;
        }
        
        _renderStates.Add(newState);
        
        _currentRenderStateId = new RenderStateId(_renderStates.Count - 1);
    }
    
    public void SetRenderState(ICameraManager cameraManager)
    {
        SetRenderState(cameraManager.MainCamera.View, cameraManager.MainCamera.Projection);
    }
    
    public RenderState GetRenderState(RenderStateId id)
    {
        return _renderStates[id.Value];
    }
    
    public RenderStateId GetCurrentRenderStateId()
    {
        return _currentRenderStateId;
    }
    
    public RenderState GetCurrentRenderState()
    {
        return _renderStates[_currentRenderStateId.Value];
    }
    
    public void SetRenderView(Matrix4x4 view)
    {
        var currentState = GetCurrentRenderState();
        SetRenderState(view, currentState.Projection);
    }
    
    public void SetRenderProjection(Matrix4x4 projection)
    {
        var currentState = GetCurrentRenderState();
        SetRenderState(currentState.View, projection);
    }
}
