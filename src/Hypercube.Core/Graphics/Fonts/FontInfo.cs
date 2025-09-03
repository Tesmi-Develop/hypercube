namespace Hypercube.Core.Graphics.Fonts;

public readonly struct FontInfo
{
    public readonly Dictionary<char, Glyph> Glyphs;

    #region head (font header table)

    public readonly int UnitsPerEm;

    #endregion
    
    #region hhea (horizontal header table)

    public readonly int Ascent;
    public readonly int Descent;
    public readonly int LineGap;

    #endregion
}