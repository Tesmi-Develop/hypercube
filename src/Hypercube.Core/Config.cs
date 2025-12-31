using Hypercube.Core.Graphics;
using Hypercube.Core.Graphics.Rendering.Api;
using Hypercube.Mathematics;
using Hypercube.Utilities.Configuration;
using Hypercube.Utilities.Debugging.Logger;

namespace Hypercube.Core;

[Config("engine.json")]
public static class Config
{
    /**
     * Mounting
     */

    public static readonly ConfigField<Dictionary<string, string>> MountFolders =
        new("MountFolders", new Dictionary<string, string> 
        {
            { ".", "/" },
            { "resources", "/" },
            { "resources/audio", "/audio/" },
            { "resources/textures", "/textures/" },
            { "resources/shaders", "/shaders/" },
            { "resources/fonts", "/fonts/" },
            { "resources/models", "/models/" },
        });
    
    /** 
     * Rendering
     */
    
    public static readonly ConfigField<RenderingApi> Rendering =
        new("Rendering", RenderingApi.OpenGl);

    public static readonly Color RenderingClearColor = Color.Black;
    
    // public static readonly ConfigField<Color> RenderingClearColor =
    //     new("RenderingClearColor", Color.Black);
    
    public static readonly ConfigField<int> RenderingMaxVertices =
        new("RenderingMaxVertices", 65532);    
    
    public static readonly ConfigField<int> RenderingIndicesPerVertex =
        new("RenderingIndicesPerVertex", 6);
    
    /**
     * Windowing
     */
    
    public static readonly ConfigField<WindowingApi> Windowing =
        new("Windowing", WindowingApi.Glfw);
    
    public static readonly ConfigField<int> WindowingWaitEventsTimeout =
        new("WindowingWaitEventsTimeout", 0);

    /**
     * Windowing threading
     */
    
    public static readonly ConfigField<bool> WindowingThreading =
        new("WindowingThreading", true);

    public static readonly ConfigField<string> WindowingThreadName =
        new("WindowingThreadName", "Windowing");

    public static readonly ConfigField<ThreadPriority> WindowingThreadPriority =
        new("WindowingThreadPriority", ThreadPriority.AboveNormal);

    public static readonly ConfigField<int> WindowingThreadStackSize =
        new("WindowingThreadStackSize", 8 * 1024 * 1024); // 8MByte

    public static readonly ConfigField<int> WindowingThreadReadySleepDelay =
        new("WindowingThreadReadySleepDelay", 10);

    public static readonly ConfigField<int> WindowingThreadEventBridgeBufferSize =
        new("WindowingThreadEventBridgeBufferSize", 32);
    
    /**
     * Fonts
     */
    
    public static readonly ConfigField<char> FontDefaultChar =
        new("FontDefaultChar", '\ufffd');
    
    /**
     * Main window
     */

    public static readonly ConfigField<string> MainWindowTitle =
        new("MainWindowTitle", "Hypercube window");
    
    public static readonly ConfigField<bool> MainWindowResizable =
        new("MainWindowResizable", true);
    
    public static readonly ConfigField<bool> MainWindowVisible =
        new("MainWindowVisible", true);
    
    public static readonly ConfigField<bool> MainWindowTransparentFramebuffer =
        new("MainWindowTransparentFramebuffer", false);
    
    public static readonly ConfigField<bool> MainWindowDecorated =
        new("MainWindowDecorated", true);
    
    public static readonly ConfigField<bool> MainWindowFloating =
        new("MainWindowFloating", true);
    
    //public static readonly ConfigField<Vector2i> MainWindowSize =
    //    new("MainWindowSize", new Vector2i(800, 600));

    /**
     * Cameras
     */
    
    // public static readonly ConfigField<Vector2>
    
    /**
     * Logging
     */

    public static readonly ConfigField<LogLevel> LoggingLevel =
        new("LoggingLevel", LogLevel.Trace);
}