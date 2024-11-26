using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public static class Window
{
    private static IWindow window;

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

    public static void Create(int width, int height, string title)
    {
        var options = WindowOptions.Default;
        options.Size = new(width, height);
        options.Title = title;
        window = Silk.NET.Windowing.Window.Create(options);
    }

    public static void SetupCallbacks(Action OnLoad, Action<float> OnUpdate, Action<float> OnRender)
    {
        // game callbacks
        window.Load += OnLoad;
        window.Update += (double delta) => OnUpdate((float)delta);
        window.Render += (double delta) => OnRender((float)delta);

        // input callbacks
        window.Load += () => Input.Initialize(window);
        window.Update += (double delta) => Input.UpdateInputState();

        // drawing callbacks
        window.Load += () => Drawing.Initialize(window);
        window.FramebufferResize += (size) => Drawing.ResizeViewport(new System.Drawing.Size(size.X, size.Y));
    }

    public static void Run()
    {
        window.Run();
        window.Dispose();
    }
}

public static class Input
{
    private static IInputContext inputContext;
    private static List<Key> keysPressed = [];
    private static List<Key> keysDown = [];
    private static List<Key> keysUp = [];
    private static List<MouseButton> mouseButtonsPressed = [];
    private static List<MouseButton> mouseButtonsDown = [];
    private static List<MouseButton> mouseButtonsUp = [];
    private static List<Key> keysDownLastFrame = [];
    private static List<Key> keysUpLastFrame = [];
    private static List<MouseButton> mouseButtonsDownLastFrame = [];
    private static List<MouseButton> mouseButtonsUpLastFrame = [];

    public static bool GetKey(Key key) => keysPressed.Contains(key);
    public static bool GetKeyDown(Key key) => keysDown.Contains(key);
    public static bool GetKeyUp(Key key) => keysUp.Contains(key);
    public static bool GetMouseButton(MouseButton button) => mouseButtonsPressed.Contains(button);
    public static bool GetMouseButtonDown(MouseButton button) => mouseButtonsDown.Contains(button);
    public static bool GetMouseButtonUp(MouseButton button) => mouseButtonsUp.Contains(button);
    public static Vector2 GetMousePosition() => inputContext.Mice[0].Position;

    public static void Initialize(IWindow window)
    {
        inputContext = window.CreateInput();
        var keyboard = inputContext.Keyboards[0];
        var mouse = inputContext.Mice[0];
        keyboard.KeyDown += (kb, key, idk) => keysDownLastFrame.Add((Key)key);
        keyboard.KeyUp += (kb, key, idk) => keysUpLastFrame.Add((Key)key);
        mouse.MouseDown += (mouse, button) => mouseButtonsDownLastFrame.Add(button);
        mouse.MouseUp += (mouse, button) => mouseButtonsUpLastFrame.Add(button);
    }

    public static void UpdateInputState()
    {
        keysDown.Clear();
        keysUp.Clear();

        foreach (var key in keysDownLastFrame) if (!keysPressed.Contains(key))
        {
            keysDown.Add(key);
            keysPressed.Add(key);
        }

        foreach (var key in keysUpLastFrame) if (keysPressed.Contains(key))
        {
            keysUp.Add(key);
            keysPressed.Remove(key);
        }

        keysDownLastFrame.Clear();
        keysUpLastFrame.Clear();
        mouseButtonsDown.Clear();
        mouseButtonsUp.Clear();

        foreach (var button in mouseButtonsDownLastFrame) if (!mouseButtonsPressed.Contains(button))
        {
            mouseButtonsDown.Add(button);
            mouseButtonsPressed.Add(button);
        }

        foreach (var button in mouseButtonsUpLastFrame) if (mouseButtonsPressed.Contains(button))
        {
            mouseButtonsUp.Add(button);
            mouseButtonsPressed.Remove(button);
        }

        mouseButtonsDownLastFrame.Clear();
        mouseButtonsUpLastFrame.Clear();
    }
}

public static class Drawing
{
    // internal state
    private static GL opengl;
    private static Color currentColor = Color.Black;

    // window callbacks
    public static void Initialize(IWindow window) => opengl = GL.GetApi(window);
    public static void ResizeViewport(Size size) => opengl.Viewport(size);

    // use accesable drawing calls
    public static void SetColor(Color color) => currentColor = color;
    public static Color GetColor() => currentColor;

    public static void ClearWindow() => opengl.ClearColor(currentColor);

    public static void DrawLine(Vector2 start, Vector2 end){}

    public static void DrawRectangle(Vector2 position, Vector2 size){}
    public static void DrawRectangleFilled(Vector2 position, Vector2 size){}
    public static void DrawRectangleRounded(Vector2 position, Vector2 size, float cornerRadius){}
    public static void DrawRectangleFilledRounded(Vector2 position, Vector2 size, float cornerRadius){}

    public static void DrawCircle(Vector2 center, float radius){}
    public static void DrawCircleFilled(Vector2 center, float radius){}
    public static void DrawEllipse(Vector2 center, Vector2 radius){}
    public static void DrawEllipseFilled(Vector2 center, Vector2 radius){}

    public static void DrawSprite(Sprite sprite, Vector2 position){}

    public static void DrawText(string text, Vector2 position, int size){}
}

public static class Audio
{
    public static void PlayAudioClip(AudioClip clip){}
    public static void StopAudioClip(AudioClip clip){}
    public static void SetVolume(float volume){}
}

public class Sprite
{
    public Sprite(string path)
    {
        // load image file
    }
}

public class AudioClip
{
    public AudioClip(string path)
    {
        // load audio file
    }
}