using System;
using System.Numerics;
using Silk.NET.Windowing;

using Hexa.NET.OpenAL;

namespace Lean;

public static class Windowing
{
    private static IWindow window;

    public unsafe static void SetupWindow(int width, int height, string title, Action OnLoad, Action<float> OnUpdate, Action<float> OnRender)
    {
        // create window
        var options = WindowOptions.Default;
        options.Size = new(width, height);
        options.Title = title;
        window = Window.Create(options);

        // engine initialize callbacks
        window.Load += () => Input.Initialize(window);
        window.Load += () => Drawing.Initialize(window);
        window.Load += () => 
        {
            ALCdevice* device = OpenAL.OpenDevice((byte*)null);
            ALCcontext* context = OpenAL.CreateContext(device);
            OpenAL.MakeContextCurrent(context);
        };

        // engine other callbacks
        window.Update += (delta) => Input.UpdateInputState();
        window.FramebufferResize += (size) => Drawing.ResizeViewport(new(size.X, size.Y));

        // game callbacks
        window.Load += OnLoad;
        window.Update += (delta) => OnUpdate((float)delta);
        window.Render += (delta) => OnRender((float)delta);

        // run window
        window.Run();
        window.Dispose();
    }

    public static string Title
    {
        get => window.Title;
        set => window.Title = value;
    }

    public static Vector2 Resolution
    {
        get => (Vector2)window.Size;
        set => window.Size = new((int)value.X, (int)value.Y);
    }

    public static bool Vsync
    {
        get => window.VSync;
        set => window.VSync = value;
    }

    public static bool Resizable
    {
        get => window.WindowBorder == WindowBorder.Resizable;
        set => window.WindowBorder = value ? WindowBorder.Resizable : WindowBorder.Fixed;
    }

    public static bool Fullscreen
    {
        get => window.WindowState == WindowState.Fullscreen;
        set => window.WindowState = value ? WindowState.Fullscreen : WindowState.Normal;
    }
}
