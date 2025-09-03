using Hypercube.Core.Graphics.Utilities.Helpers;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;
using StbImageWriteSharp;
using StbTrueTypeSharp;

namespace Hypercube.Core.Graphics.Fonts;

public static class FontAtlasGenerator
{
    public static unsafe Stream Gen(byte[] data, int width, int height, int padding, float spread = 8f)
    {
        var font = StbTrueTypeHelper.GetFont(data);
        StbTrueTypeHelper.GetFontVMetrics(font, out var ascent, out var descent, out var lineGap);
        StbTrueTypeHelper.GetFontBoundingBox(font, out var boundingBox);

        foreach (var @char in StbTrueTypeHelper.GetFontChars(font))
        {
            StbTrueTypeHelper.GetGlyphIndex(font, @char, out var glyphIndex);
            StbTrueTypeHelper.GetGlyphBitmap(font, glyphIndex, Vector2.One, out var bitmap, out var size, out var offset);
        }
        
        var info = new FontInfo();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    public static unsafe Stream Generate(byte[] fontData, out Dictionary<char, Glyph> glyphs, out int ascent, out int descent, out int lineGap, out float scale, int fontSize = 32)
    {
        const int padding = 2;

        glyphs = [];

        var fontInfo = new StbTrueType.stbtt_fontinfo();
        fixed (byte* fontPtr = fontData)
            StbTrueType.stbtt_InitFont(fontInfo, fontPtr, 0);

        scale = StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, fontSize);
        StbTrueTypeHelper.GetFontVMetrics(fontInfo, out ascent, out descent, out lineGap);
        
        var chars = new List<char>();
        for (var codepoint = 0; codepoint <= 0xFFFF; codepoint++)
        {
            if (StbTrueType.stbtt_FindGlyphIndex(fontInfo, (char) codepoint) == 0)
                continue;
            
            chars.Add((char) codepoint);
        }
        
        var charCount = chars.Count;
        var columns = (int)Math.Ceiling(Math.Sqrt(charCount));
        var cellSize = fontSize + padding;
        var rows = (int)Math.Ceiling(charCount / (float)columns);
        var atlasWidth = columns * cellSize;
        var atlasHeight = rows * cellSize;

        // RGBA image
        var pixelData = new byte[atlasWidth * atlasHeight * 4];
        int x = 0, y = 0;

        foreach (var c in chars)
        {
            var glyphIndex = StbTrueType.stbtt_FindGlyphIndex(fontInfo, c);
            int width, height, xOffset, yOffset;
            
            var bitmap = StbTrueType.stbtt_GetGlyphBitmap(
                fontInfo,
                scale,
                scale,
                glyphIndex,
                &width,
                &height,
                &xOffset,
                &yOffset
            );

            // Копируем глиф в RGBA image
            for (var j = 0; j < height; j++)
            {
                for (var i = 0; i < width; i++)
                {
                    var value = bitmap[j * width + i];
                    var dstX = x + i;
                    var dstY = y + j;
                    var dstIndex = (dstY * atlasWidth + dstX) * 4;
                    
                    pixelData[dstIndex + 0] = byte.MaxValue;
                    pixelData[dstIndex + 1] = byte.MaxValue;
                    pixelData[dstIndex + 2] = byte.MaxValue;
                    pixelData[dstIndex + 3] = value;
                }
            }

            StbTrueType.stbtt_FreeBitmap(bitmap, null);

            int advanceWidth, leftSideBearing;
            StbTrueType.stbtt_GetGlyphHMetrics(fontInfo, glyphIndex, &advanceWidth, &leftSideBearing);

            glyphs.Add(c, new Glyph
            {
                Character = c,
                SourceRect = new Rect2(x, y, x + width, y + height),
                Offset = new Vector2(xOffset, yOffset),
                Advance = advanceWidth
            });

            x += cellSize;
            
            if (x + cellSize <= atlasWidth)
                continue;
            
            x = 0;
            y += cellSize;
        }

        var stream = new MemoryStream();
        var writer = new ImageWriter();
        writer.WritePng(pixelData, atlasWidth, atlasHeight, ColorComponents.RedGreenBlueAlpha, stream);
        
        stream.Position = 0;
        return stream;
    }
}