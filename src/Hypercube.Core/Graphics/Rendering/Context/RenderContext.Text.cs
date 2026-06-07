using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Graphics.Rendering.Batching;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Shapes;

namespace Hypercube.Core.Graphics.Rendering.Context;

public partial class RenderContext
{
    public void DrawText(string text, Font font, Vector2 position, Color color, float scale = 1f, Vector2 align = default)
    {
        if (_renderingApi.TexturingShaderProgram is null)
            throw new Exception("Texturing shader program is not initialized.");
        
        if (string.IsNullOrEmpty(text))
            return;

        if (font.Texture.Gpu is null)
            font.Texture.GpuBind(_renderingApi);
        
        // Starting position for rendering
        var size = font.Measure(text);

        var pen = new Vector2(
            position.X - size.X * align.X,
            position.Y - size.Y * align.Y + font.LineHeight * 0.5f * align.Y
        );

        // Using a texture shader and font as a texture
        _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, new ShaderSetup { ShaderProgram = _renderingApi.TexturingShaderProgram }, font.Texture.Gpu?.Handle);

        foreach (var c in text)
        {
            switch (c)
            {
                case '\r':
                    continue;
               
                case '\n':
                    // Move to next line
                    pen = new Vector2(position.X, pen.Y - font.LineHeight * scale) ;
                    // baselineY = pen.Y - font.Baseline * scale;
                    // prev = '\0';
                    continue;
            }

            if (!font.Glyphs.TryGetValue(c, out var glyph))
            {
                // Can be replaced by a space or skip
                if (!font.Glyphs.TryGetValue(Config.FontDefaultChar, out glyph))
                    continue;
            }

            // Calculate glyph size and position
            var glyphSize = glyph.SourceRect.Size * scale;
            var glyphPosition = new Vector2(
                pen.X + glyph.Offset.X,
                pen.Y - glyph.Offset.Y
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
            
            // DrawRectangle(quad, Color.Red, true);
            
            // Adding a glyph to a patch
            // _renderingApi.EnsureBatch(PrimitiveTopology.TriangleList, _renderingApi.TexturingShaderProgram.Handle, font.Texture.Gpu?.Handle);
            AddQuadTriangleBatch(_renderingApi.BatchVerticesIndex, quad, uv, color);

            // Shift the stylus to Advance (horizontal shift)
            pen = new Vector2(pen.X + glyph.Advance * scale, pen.Y);
        }
    }

    public float DrawChar(char symbol, Font font, Vector2 position, Color color, float scale = 1f)
    {
        return 0;
    }
}