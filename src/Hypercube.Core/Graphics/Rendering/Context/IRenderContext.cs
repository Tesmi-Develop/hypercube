using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Core.Graphics.Resources;
using Hypercube.Mathematics;
using Hypercube.Mathematics.Quaternions;
using Hypercube.Mathematics.Shapes;
using Hypercube.Mathematics.Vectors;

namespace Hypercube.Core.Graphics.Rendering.Context;

/// <summary>
/// Defines a rendering context abstraction that provides high-level drawing 
/// and rendering operations on top of a low-level rendering API.
/// </summary>
public interface IRenderContext
{
    /// <summary>
    /// Initializes the rendering context with the given rendering API.
    /// </summary>
    /// <param name="api">An instance of the rendering API providing low-level drawing functionality.</param>
    void Init(IRenderingApi api);

    /// <summary>
    /// Draws a 3D model at a given position, with rotation, scale, and optional texture override.
    /// </summary>
    /// <param name="model">The 3D model to render.</param>
    /// <param name="position">The world-space position where the model will be drawn.</param>
    /// <param name="rotation">The rotation applied to the model.</param>
    /// <param name="scale">The scaling factor applied to the model.</param>
    /// <param name="color">The base color tint applied to the model.</param>
    /// <param name="texture">An optional texture that can override the model's default material texture.</param>
    void DrawModel(Model model, Vector3 position, Quaternion rotation, Vector3 scale, Color color, Texture? texture = null);
    
    /// <summary>
    /// Draws a string of text on the screen.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="font">The font to use for rendering the text.</param>
    /// <param name="position">The position on the screen where the text will be drawn.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The scale factor for the text.</param>
    void DrawText(string text, Font font, Vector2 position, Color color, float scale = 1f);
    
    /// <summary>
    /// Draws a rectangle on the screen.
    /// </summary>
    /// <param name="box">The bounding box of the rectangle.</param>
    /// <param name="color">The color to use for the rectangle.</param>
    /// <param name="outline">Whether to draw only the outline (true) or fill the rectangle (false).</param>
    void DrawRectangle(Rect2 box, Color color, bool outline = false);

    /// <summary>
    /// Draws a straight line between two points on the screen.
    /// </summary>
    /// <param name="start">The starting point of the line.</param>
    /// <param name="end">The ending point of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1f);
    
    /// <summary>
    /// Draws a circle on the screen.
    /// </summary>
    /// <param name="center">The center position of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    /// <param name="segments">The number of line segments used to approximate the circle. Higher values yield smoother circles.</param>
    void DrawCircle(Vector2 center, float radius, Color color, int segments = 32);
    
    /// <summary>
    /// Draws a texture on the screen with specified transformations.
    /// </summary>
    /// <param name="texture">The texture to draw.</param>
    /// <param name="position">The position on the screen where the texture will be drawn.</param>
    /// <param name="rotation">The rotation to apply to the texture.</param>
    /// <param name="scale">The scale to apply to the texture.</param>
    /// <param name="color">The color tint to apply to the texture.</param>
    void DrawTexture(Texture texture, Vector2 position, Angle rotation, Vector2 scale, Color color);

    void Scissor(bool value);
    void Scissor(Rect2i rect);
}