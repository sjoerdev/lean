using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

public static class Engine
{
    private static GL opengl;
    private static IWindow window;
    private static IInputContext input;

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
    private static Vector2 mousePosition;

    // window settings
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
        set => window.WindowState = WindowState.Normal;
    }

    public static void SetupWindow(Vector2 resolution, string title)
    {
        var options = WindowOptions.Default;
        options.Size = new((int)resolution.X, (int)resolution.Y);
        options.Title = title;
        window = Window.Create(options);
    }

    public static void SetupCallbacks(Action OnLoad, Action<float> OnUpdate, Action<float> OnRender)
    {
        // game callbacks
        window.Load += OnLoad;
        window.Update += (double delta) => OnUpdate((float)delta);
        window.Render += (double delta) => OnRender((float)delta);

        // internal callbacks
        window.Load += SetupInput;
        window.Load += () => opengl = GL.GetApi(window);
        window.FramebufferResize += (size) => opengl.Viewport(size);

        void SetupInput()
        {
            input = window.CreateInput();
            var keyboard = input.Keyboards[0];
            var mouse = input.Mice[0];
            keyboard.KeyDown += (kb, key, idk) => keysDownLastFrame.Add((Key)key);
            keyboard.KeyUp += (kb, key, idk) => keysUpLastFrame.Add((Key)key);
            mouse.MouseDown += (mouse, button) => mouseButtonsDownLastFrame.Add(button);
            mouse.MouseUp += (mouse, button) => mouseButtonsUpLastFrame.Add(button);
            mouse.MouseMove += (mouse, position) => mousePosition = position;
            window.Update += (double delta) => UpdateInputState();
        }

        void UpdateInputState()
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

    public static void RunWindow()
    {
        window.Run();
        window.Dispose();
    }

    // input
    public static bool GetKey(Key key) => keysPressed.Contains(key);
    public static bool GetKeyDown(Key key) => keysDown.Contains(key);
    public static bool GetKeyUp(Key key) => keysUp.Contains(key);
    public static bool GetMouseButton(MouseButton button) => mouseButtonsPressed.Contains(button);
    public static bool GetMouseButtonDown(MouseButton button) => mouseButtonsDown.Contains(button);
    public static bool GetMouseButtonUp(MouseButton button) => mouseButtonsUp.Contains(button);
    public static Vector2 GetMousePosition() => mousePosition;

    /*

    // audio
    public static void PlayAudioClip(AudioClip clip){}
    public static void StopAudioClip(AudioClip clip){}
    public static void SetVolume(float volume){}

    // Drawing
    private static Color currentColor = Color.Black;

    public static void SetColor(Color color) => currentColor = color;
    public static Color GetColor() => currentColor;

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

    */
}

public enum Key
{
    Unknown = -1,
    Space = 32,
    Apostrophe = 39,
    Comma = 44,
    Minus = 45,
    Period = 46,
    Slash = 47,
    Number0 = 48,
    D0 = Number0,
    Number1 = 49,
    Number2 = 50,
    Number3 = 51,
    Number4 = 52,
    Number5 = 53,
    Number6 = 54,
    Number7 = 55,
    Number8 = 56,
    Number9 = 57,
    Semicolon = 59,
    Equal = 61,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    LeftBracket = 91,
    BackSlash = 92,
    RightBracket = 93,
    GraveAccent = 96,
    World1 = 161,
    World2 = 162,
    Escape = 256,
    Enter = 257,
    Tab = 258,
    Backspace = 259,
    Insert = 260,
    Delete = 261,
    Right = 262,
    Left = 263,
    Down = 264,
    Up = 265,
    PageUp = 266,
    PageDown = 267,
    Home = 268,
    End = 269,
    CapsLock = 280,
    ScrollLock = 281,
    NumLock = 282,
    PrintScreen = 283,
    Pause = 284,
    F1 = 290,
    F2 = 291,
    F3 = 292,
    F4 = 293,
    F5 = 294,
    F6 = 295,
    F7 = 296,
    F8 = 297,
    F9 = 298,
    F10 = 299,
    F11 = 300,
    F12 = 301,
    F13 = 302,
    F14 = 303,
    F15 = 304,
    F16 = 305,
    F17 = 306,
    F18 = 307,
    F19 = 308,
    F20 = 309,
    F21 = 310,
    F22 = 311,
    F23 = 312,
    F24 = 313,
    F25 = 314,
    Keypad0 = 320,
    Keypad1 = 321,
    Keypad2 = 322,
    Keypad3 = 323,
    Keypad4 = 324,
    Keypad5 = 325,
    Keypad6 = 326,
    Keypad7 = 327,
    Keypad8 = 328,
    Keypad9 = 329,
    KeypadDecimal = 330,
    KeypadDivide = 331,
    KeypadMultiply = 332,
    KeypadSubtract = 333,
    KeypadAdd = 334,
    KeypadEnter = 335,
    KeypadEqual = 336,
    ShiftLeft = 340,
    ControlLeft = 341,
    AltLeft = 342,
    SuperLeft = 343,
    ShiftRight = 344,
    ControlRight = 345,
    AltRight = 346,
    SuperRight = 347,
    Menu = 348
}