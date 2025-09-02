using Hypercube.Core.Graphics.Rendering.Batching;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Matrices;
using Hypercube.Mathematics.Quaternions;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial class RenderContext
{
    public void DrawModel(Model model, Vector3 position, Quaternion rotation, Vector3 scale, Color color, Texture? texture)
    {
        var shader = texture is null ? _renderingApi.PrimitiveShaderProgram : _renderingApi.TexturingShaderProgram;
        if (shader is null)
            throw new Exception("Model shader program is not initialized.");

        if (texture is not null && texture.Gpu is null)
            texture.GpuBind(_renderingApi);
        
        _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, shader.Handle, texture?.Gpu?.Handle);

        var start = _renderingApi.BatchVerticesIndex;
        var matrix = Matrix4x4.CreateTransform(position, rotation, scale);
        
        foreach (var (v, vt, vn) in model.Indices)
        {
            var pos = matrix.Transform(model.Vertices[v]);
            
            var uv = vt >= 0 && vt < model.UVs.Length ? model.UVs[vt] : Vector2.Zero;;
            var normal = vn >= 0 && vn < model.Normals.Length
                ? matrix.Transform(model.Normals[vn]).Normalized
                : Vector3.UnitZ;
    
            _renderingApi.PushVertex(new Vertex(pos, uv, color, normal));
        }
        
        for (var i = 0; i < model.Indices.Length; i += 3)
        {
            _renderingApi.PushIndex(start, i + 0);
            _renderingApi.PushIndex(start, i + 1);
            _renderingApi.PushIndex(start, i + 2);
        }
    }
    
    public void DrawText(string text, Font font, Vector2 position, Color color, float scale = 1f)
    {
        if (_renderingApi.TexturingShaderProgram is null)
            throw new Exception("Texturing shader program is not initialized.");
        
        if (string.IsNullOrEmpty(text))
            return;

        if (font.Texture.Gpu is null)
            font.Texture.GpuBind(_renderingApi);
        
        // Starting position for rendering
        var penPosition = position;

        // Using a texture shader and font as a texture
        _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, _renderingApi.TexturingShaderProgram.Handle, font.Texture.Gpu?.Handle);

        foreach (var c in text)
        {
            if (!font.Glyphs.TryGetValue(c, out var glyph))
            {
                // Can be replaced by a space or skip
                continue;
            }

            // Calculate glyph size and position
            var glyphSize = glyph.SourceRect.Size * scale;
            var glyphPosition = new Vector2(
                penPosition.X + glyph.Offset.X * scale,
                penPosition.Y
            );

            // Glyph square in local coordinates
            var quad = new Rect2(
                new Vector2(glyphPosition.X, glyphPosition.Y + glyphSize.Y),
                new Vector2(glyphPosition.X + glyphSize.X, glyphPosition.Y)
            );
            
            var textureSize = font.Texture.Size; 
            var uv = new Rect2(
                new Vector2(glyph.SourceRect.TopLeft.X / textureSize.X, glyph.SourceRect.TopLeft.Y / textureSize.Y),
                new Vector2(glyph.SourceRect.BottomRight.X / textureSize.X, glyph.SourceRect.BottomRight.Y / textureSize.Y)
            );

            // Adding a glyph to a patch
            AddQuadTriangleBatch(_renderingApi.BatchVerticesIndex, quad, uv, color);

            // Shift the stylus to Advance (horizontal shift)
            penPosition = new Vector2(penPosition.X + glyph.Advance * scale, penPosition.Y);
        }
    }
    
    public void DrawTexture(Texture texture, Vector2 position, Angle rotation, Vector2 scale, Color color)
    {
        if (_renderingApi.TexturingShaderProgram is null)
            throw new Exception();

        var halfSize = texture.Size / 2;
        var rect = new Rect4(
            new Vector2(-halfSize.X, -halfSize.Y),
            new Vector2(halfSize.X, -halfSize.Y),
            new Vector2(halfSize.X, halfSize.Y),
            new Vector2(-halfSize.X, halfSize.Y)
        );
        
        var matrix =
            Matrix4x4.CreateScale(scale) *
            Matrix4x4.CreateRotationZ((float) rotation) *
            Matrix4x4.CreateTranslation(position);
        
        _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, _renderingApi.TexturingShaderProgram.Handle, texture.Gpu?.Handle);
        AddQuadTriangleBatch(_renderingApi.BatchVerticesIndex, matrix.Transform(rect), Rect2.UV, color);
    }
    
    public void DrawRectangle(Rect2 box, Color color, bool outline = false)
    {
        if (_renderingApi.PrimitiveShaderProgram is null)
            throw new Exception();
        
        _renderingApi.EnsureBatch(outline ? PrimitiveTopology.LineList : PrimitiveTopology.TriangleList, _renderingApi.PrimitiveShaderProgram.Handle, null);
        AddQuadTriangleBatch(_renderingApi.BatchVerticesIndex, Matrix4x4.Identity.Transform(box), Rect2.UV, color);
    }
    
    public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1f)
    {
        if (_renderingApi.PrimitiveShaderProgram is null)
            throw new InvalidOperationException("Primitive shader program is not initialized");

        var direction = (end - start).Normalized;
        var normal = new Vector2(-direction.Y, direction.X) * thickness / 2f;

        _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, _renderingApi.PrimitiveShaderProgram.Handle, null);

        var startIndex = _renderingApi.BatchVerticesIndex;
        
        _renderingApi.PushVertex(new Vertex(start - normal, Vector2.Zero, color));
        _renderingApi.PushVertex(new Vertex(start + normal, Vector2.Zero, color));
        _renderingApi.PushVertex(new Vertex(end - normal, Vector2.Zero, color));
        _renderingApi.PushVertex(new Vertex(end + normal, Vector2.Zero, color));
        
        _renderingApi.PushIndex(startIndex, 0);
        _renderingApi.PushIndex(startIndex, 1);
        _renderingApi.PushIndex(startIndex, 2);
        _renderingApi.PushIndex(startIndex, 1);
        _renderingApi.PushIndex(startIndex, 3);
        _renderingApi.PushIndex(startIndex, 2);
    }

    public void DrawCircle(Vector2 center, float radius, Color color, int segments = 32)
    {
        if (_renderingApi.PrimitiveShaderProgram is null)
            throw new InvalidOperationException("Primitive shader program is not initialized");

        _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, _renderingApi.PrimitiveShaderProgram.Handle, null);

        var startIndex = _renderingApi.BatchVerticesIndex;
        _renderingApi.PushVertex(new Vertex(center, Vector2.Zero, color));
        
        for (var i = 0; i <= segments; i++)
        {
            var angle = (float)i / segments * MathF.PI * 2;
            var point = center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
            _renderingApi.PushVertex(new Vertex(point, Vector2.Zero, color));
        }
        
        for (var i = 1; i <= segments; i++)
        {
            _renderingApi.PushIndex(startIndex, 0);
            _renderingApi.PushIndex(startIndex, i);
            _renderingApi.PushIndex(startIndex, i % segments + 1);
        }
    }

    private void AddLineBatch(int start, Rect2 box2, Color color)
    {
        _renderingApi.PushVertex(new Vertex(box2.TopRight, Vector2.Zero, color));
        _renderingApi.PushVertex(new Vertex(box2.BottomLeft, Vector2.Zero, color));
        _renderingApi.PushIndex(start, 0);
        _renderingApi.PushIndex(start, 1);
    }
    
    private void AddPointBatch(int start, Vector2 point, Color color)
    {
        _renderingApi.PushVertex(new Vertex(point, Vector2.Zero, color));
        _renderingApi.PushIndex(start, 0);
    }

    private void AddQuadTriangleBatch(int start, Rect4 rect, Rect2 uv, Color color)
    {
        _renderingApi.PushVertex(new Vertex(rect.Point0, uv.TopLeft, color));
        _renderingApi.PushVertex(new Vertex(rect.Point1, uv.TopRight, color));
        _renderingApi.PushVertex(new Vertex(rect.Point2, uv.BottomRight, color));
        _renderingApi.PushVertex(new Vertex(rect.Point3, uv.BottomLeft, color));
        
        _renderingApi.PushIndex(start, 0);
        _renderingApi.PushIndex(start, 1);
        _renderingApi.PushIndex(start, 3);
        _renderingApi.PushIndex(start, 1);
        _renderingApi.PushIndex(start, 2);
        _renderingApi.PushIndex(start, 3);
    }

    private void AddQuadTriangleBatch(int start, Rect2 rect, Rect2 uv, Color color)
    {
        _renderingApi.PushVertex(new Vertex(rect.TopRight, uv.TopRight, color));
        _renderingApi.PushVertex(new Vertex(rect.BottomRight, uv.BottomRight, color));
        _renderingApi.PushVertex(new Vertex(rect.BottomLeft, uv.BottomLeft, color));
        _renderingApi.PushVertex(new Vertex(rect.TopLeft, uv.TopLeft, color));
        
        _renderingApi.PushIndex(start, 0);
        _renderingApi.PushIndex(start, 1);
        _renderingApi.PushIndex(start, 3);
        _renderingApi.PushIndex(start, 1);
        _renderingApi.PushIndex(start, 2);
        _renderingApi.PushIndex(start, 3);
    }
}