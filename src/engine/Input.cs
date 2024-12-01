using System.Collections.Generic;
using System.Numerics;

using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Lean;

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
