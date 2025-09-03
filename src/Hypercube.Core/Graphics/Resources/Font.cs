using Hypercube.Core.Graphics.Fonts;
using Hypercube.Core.Resources.Loaders;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Resources;

/// <summary>
/// Represents a GPU font with glyph atlas, metrics, and helpers.
/// </summary>
[PublicAPI]
public class Font : Resource
{
    private readonly Dictionary<char, Glyph> _glyphs;

    /// <summary>
    /// Font texture atlas.
    /// </summary>
    public readonly Texture Texture;

    /// <summary>
    /// Height above the baseline (in font units).
    /// </summary>
    public readonly int Ascent;

    /// <summary>
    /// Depth below the baseline (usually negative in font units).
    /// </summary>
    public readonly int Descent;

    /// <summary>
    /// Additional space between lines (line gap in font units).
    /// </summary>
    public readonly int LineGap;

    /// <summary>
    /// Scale factor from font units to pixels.
    /// </summary>
    public readonly float Scale;

    /// <summary>
    /// Nominal font size in pixels.
    /// </summary>
    public readonly int Size;

    /// <summary>
    /// Glyph dictionary.
    /// </summary>
    public IReadOnlyDictionary<char, Glyph> Glyphs => _glyphs;

    /// <summary>
    /// Total line height in pixels (ascent - descent + line gap).
    /// </summary>
    public float LineHeight => (Ascent - Descent + LineGap) * Scale;

    /// <summary>
    /// Distance from the top of the line to the baseline in pixels.
    /// </summary>
    public float Baseline => Ascent * Scale;

    public Font(Texture texture,
                Dictionary<char, Glyph> glyphs,
                int ascent,
                int descent,
                int lineGap,
                float scale,
                int size)
    {
        Texture = texture;
        Ascent = ascent;
        Descent = descent;
        LineGap = lineGap;
        Scale = scale;
        Size = size;

        _glyphs = glyphs;
    }

    /// <summary>
    /// Tries to get a glyph for a given character.
    /// </summary>
    public bool TryGetGlyph(char c, out Glyph glyph)
    {
        return _glyphs.TryGetValue(c, out glyph);
    }

    /// <summary>
    /// Returns the glyph for a character or null if not available.
    /// </summary>
    public Glyph? GetGlyphOrDefault(char c)
    {
        _glyphs.TryGetValue(c, out var glyph);
        return glyph;
    }

    /// <summary>
    /// Measures the width of a single-line string in pixels.
    /// </summary>
    public float MeasureTextWidth(string text, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        float width = 0;
        foreach (var c in text)
        {
            if (_glyphs.TryGetValue(c, out var glyph))
                width += glyph.Advance * scale;
        }
        
        return width;
    }

    /// <summary>
    /// Measures the width and height of a multi-line string in pixels.
    /// </summary>
    public Vector2 MeasureText(string text, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text))
            return Vector2.Zero;

        var width = 0f;
        var maxWidth = 0f;
        var height = LineHeight * scale;

        foreach (var c in text)
        {
            if (c == '\n')
            {
                maxWidth = MathF.Max(maxWidth, width);
                width = 0;
                height += LineHeight * scale;
                continue;
            }

            if (_glyphs.TryGetValue(c, out var glyph))
                width += glyph.Advance * scale;
        }

        maxWidth = MathF.Max(maxWidth, width);
        return new Vector2(maxWidth, height);
    }

    public override void Dispose()
    {
        Texture.Dispose();
    }
}
