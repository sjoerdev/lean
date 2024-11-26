using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public static class Windowing
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

    public static void CreateWindow(int width, int height, string title, Action OnLoad, Action<float> OnUpdate, Action<float> OnRender)
    {
        // create window
        var options = WindowOptions.Default;
        options.Size = new(width, height);
        options.Title = title;
        window = Window.Create(options);

        // game callbacks
        window.Load += OnLoad;
        window.Update += (double delta) => OnUpdate((float)delta);
        window.Render += (double delta) => OnRender((float)delta);

        // engine callbacks
        window.Load += () => Input.Initialize(window);
        window.Load += () => Drawing.Initialize(window);
        window.Update += (double delta) => Input.UpdateInputState();
        window.FramebufferResize += (size) => Drawing.ResizeViewport(new(size.X, size.Y));

        // run window
        window.Run();
        window.Dispose();
    }

    public static void SwapBuffers() => window.SwapBuffers();
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
    private static GL opengl;
    private static uint program;
    private static uint vao;
    private static uint vbo;

    public static void Initialize(IWindow window)
    {
        opengl = GL.GetApi(window);
        program = CreateShaderProgram();
        vao = opengl.GenVertexArray();
        vbo = opengl.GenBuffer();
        SetProjectionMatrix(window.Size.X, window.Size.Y);
    }

    private static uint CreateShaderProgram()
    {
        string vertSource = 
        @"
            #version 330 core

            layout(location = 0) in vec2 aPos;

            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * vec4(aPos, 0.0, 1.0);
            }
        ";

        string fragSource = 
        @"
            #version 330 core

            uniform vec4 color;

            out vec4 FragColor;

            void main()
            {
                FragColor = color;
            }
        ";

        uint vertShader = CreateShader(vertSource, ShaderType.VertexShader);
        uint fragShader = CreateShader(fragSource, ShaderType.FragmentShader);

        uint shaderProgram = opengl.CreateProgram();
        opengl.AttachShader(shaderProgram, vertShader);
        opengl.AttachShader(shaderProgram, fragShader);
        opengl.LinkProgram(shaderProgram);
        opengl.DeleteShader(vertShader);
        opengl.DeleteShader(fragShader);

        return shaderProgram;
    }

    private static uint CreateShader(string source, ShaderType type)
    {
        uint shader = opengl.CreateShader(type);
        opengl.ShaderSource(shader, source);
        opengl.CompileShader(shader);
        return shader;
    }

    public static void SetProjectionMatrix(int width, int height)
    {
        opengl.UseProgram(program);
        var projectionMatrix = Matrix4x4.CreateOrthographic(width, height, -1, 1);

        // convert the matrix to a float array in colum major order
        float[] projectionArray =
        [
            projectionMatrix.M11,
            projectionMatrix.M21,
            projectionMatrix.M31,
            projectionMatrix.M41,
            projectionMatrix.M12,
            projectionMatrix.M22,
            projectionMatrix.M32,
            projectionMatrix.M42,
            projectionMatrix.M13,
            projectionMatrix.M23,
            projectionMatrix.M33,
            projectionMatrix.M43,
            projectionMatrix.M14,
            projectionMatrix.M24,
            projectionMatrix.M34,
            projectionMatrix.M44,
        ];

        opengl.UniformMatrix4(opengl.GetUniformLocation(program, "projection"), 1, false, ref projectionArray[0]);
    }
    
    public static void ClearWindow()
    {
        opengl.ClearColor(currentColor.R / 255f, currentColor.G / 255f, currentColor.B / 255f, currentColor.A / 255f);
        opengl.Clear(ClearBufferMask.ColorBufferBit);
    }
    public static void ResizeViewport(Size size)
    {
        SetProjectionMatrix(size.Width, size.Height);
        opengl.Viewport(size);
    }

    private static Color currentColor = Color.Blue;
    public static void SetColor(Color color)
    {
        currentColor = color;
        opengl.UseProgram(program);
        opengl.Uniform4(opengl.GetUniformLocation(program, "color"), color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }
    public static Color GetColor() => currentColor;
    
    // basic shapes (positions are in pixel coordinates)
    public static void DrawLine(Vector2 start, Vector2 end, int width)
    {
        DrawPrimitive([start.X, start.Y, end.X, end.Y], 2, PrimitiveType.Lines);
    }

    public static void DrawRectangle(Vector2 position, Vector2 size)
    {
        float[] vertices =
        [
            position.X, position.Y,
            position.X + size.X, position.Y,
            position.X + size.X, position.Y,
            position.X + size.X, position.Y + size.Y,
            position.X + size.X, position.Y + size.Y,
            position.X, position.Y + size.Y,
            position.X, position.Y + size.Y,
            position.X, position.Y
        ];

        DrawPrimitive(vertices, 8, PrimitiveType.Lines);
    }

    public static void DrawRectangleFilled(Vector2 position, Vector2 size)
    {
        float[] vertices =
        [
            position.X, position.Y,
            position.X + size.X, position.Y,
            position.X, position.Y + size.Y,
            position.X + size.X, position.Y,
            position.X + size.X, position.Y + size.Y,
            position.X, position.Y + size.Y
        ];

        DrawPrimitive(vertices, 6, PrimitiveType.Triangles);
    }

    public static void DrawCircle(Vector2 center, float radius, int segments = 32)
    {
        var vertices = new float[segments * 2];

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)(i * 2 * Math.PI / segments);
            vertices[i * 2] = center.X + radius * (float)Math.Cos(angle);
            vertices[i * 2 + 1] = center.Y + radius * (float)Math.Sin(angle);
        }

        DrawPrimitive(vertices, segments, PrimitiveType.LineLoop);
    }

    public static void DrawCircleFilled(Vector2 center, float radius, int segments = 32)
    {
        var vertices = new float[(segments + 2) * 2];

        vertices[0] = center.X;
        vertices[1] = center.Y;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)(i * 2 * Math.PI / segments);
            vertices[(i + 1) * 2] = center.X + radius * (float)Math.Cos(angle);
            vertices[(i + 1) * 2 + 1] = center.Y + radius * (float)Math.Sin(angle);
        }

        vertices[(segments + 1) * 2] = vertices[2];
        vertices[(segments + 1) * 2 + 1] = vertices[3];

        DrawPrimitive(vertices, segments + 2, PrimitiveType.TriangleFan);
    }

    public static void DrawEllipse(Vector2 center, Vector2 radius, int segments = 32)
    {
        var vertices = new float[segments * 2];

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)(i * 2 * Math.PI / segments);
            vertices[i * 2] = center.X + radius.X * (float)Math.Cos(angle);
            vertices[i * 2 + 1] = center.Y + radius.Y * (float)Math.Sin(angle);
        }

        DrawPrimitive(vertices, segments, PrimitiveType.LineLoop);
    }

    public static void DrawEllipseFilled(Vector2 center, Vector2 radius, int segments = 32)
    {
        var vertices = new float[(segments + 2) * 2];

        vertices[0] = center.X;
        vertices[1] = center.Y;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)(i * 2 * Math.PI / segments);
            vertices[(i + 1) * 2] = center.X + radius.X * (float)Math.Cos(angle);
            vertices[(i + 1) * 2 + 1] = center.Y + radius.Y * (float)Math.Sin(angle);
        }

        vertices[(segments + 1) * 2] = vertices[2];
        vertices[(segments + 1) * 2 + 1] = vertices[3];

        DrawPrimitive(vertices, segments + 2, PrimitiveType.TriangleFan);
    }

    public static void DrawTriangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        DrawPrimitive([point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y], 3, PrimitiveType.LineLoop);
    }

    public static void DrawTriangleFilled(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        DrawPrimitive([point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y], 3, PrimitiveType.Triangles);
    }

    private unsafe static void DrawPrimitive(float[] vertices, int vertexCount, PrimitiveType primitiveType)
    {
        opengl.BindVertexArray(vao);
        opengl.BindBuffer(GLEnum.ArrayBuffer, vbo);
        fixed (void* ptr = &vertices[0]) opengl.BufferData(GLEnum.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), ptr, GLEnum.StaticDraw);
        opengl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        opengl.EnableVertexAttribArray(0);
        opengl.DrawArrays(primitiveType, 0, (uint)vertexCount);
        opengl.DisableVertexAttribArray(0);
        opengl.BindBuffer(GLEnum.ArrayBuffer, 0);
        opengl.BindVertexArray(0);
    }

    // complex stuff
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