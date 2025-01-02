using System;
using System.Numerics;
using System.Drawing;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Linq;

namespace Lean;

public static class Drawing
{
    private static GL opengl;
    public static GL GetOpenGL() => opengl;

    private static Shader prim_shader;
    private static uint prim_vao;
    private static uint prim_vbo;

    public static void Initialize(IWindow window)
    {
        opengl = GL.GetApi(window);
        opengl.Enable(GLEnum.Blend);
        opengl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        prim_shader = new Shader("res/glsl/prim-vert.glsl", "res/glsl/prim-frag.glsl");
        UpdatePrimitiveProjection(window.Size.X, window.Size.Y);
        prim_vao = opengl.GenVertexArray();
        prim_vbo = opengl.GenBuffer();
    }

    public static void UpdatePrimitiveProjection(int width, int height)
    {
        prim_shader.Use();
        prim_shader.SetUniformMatrix4("projection", Matrix4x4.CreateOrthographic(width, height, -1, 1));
    }

    public static void ResizeViewport(Size size)
    {
        UpdatePrimitiveProjection(size.Width, size.Height);
        opengl.Viewport(size);
    }
    
    public static void ClearWindow()
    {
        opengl.ClearColor(currentColor.R / 255f, currentColor.G / 255f, currentColor.B / 255f, currentColor.A / 255f);
        opengl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private static Color currentColor = Color.Blue;
    public static Color GetColor() => currentColor;
    public static void SetColor(Color color)
    {
        currentColor = color;
        prim_shader.Use();
        prim_shader.SetUniform4("col", new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
    }
    
    // basic shapes (positions are in pixel coordinates)
    public static void DrawLine(Vector2 start, Vector2 end)
    {
        DrawPrimitive([start.X, start.Y, end.X, end.Y], 2, PrimitiveType.Lines);
    }

    public static void DrawRectangle(Vector2 position, Vector2 size)
    {
        Vector2 half = size / 2;
        
        float[] vertices =
        [
            position.X - half.X, position.Y - half.Y,
            position.X + half.X, position.Y - half.Y,
            position.X + half.X, position.Y - half.Y,
            position.X + half.X, position.Y + half.Y,
            position.X + half.X, position.Y + half.Y,
            position.X - half.X, position.Y + half.Y,
            position.X - half.X, position.Y + half.Y,
            position.X - half.X, position.Y - half.Y
        ];

        DrawPrimitive(vertices, 8, PrimitiveType.Lines);
    }

    public static void DrawRectangleFilled(Vector2 position, Vector2 size)
    {
        Vector2 half = size / 2;

        float[] vertices =
        [
            position.X - half.X, position.Y - half.Y,
            position.X + half.X, position.Y - half.Y,
            position.X - half.X, position.Y + half.Y,
            position.X + half.X, position.Y - half.Y,
            position.X + half.X, position.Y + half.Y,
            position.X - half.X, position.Y + half.Y
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

    public static void DrawThickLine(Vector2 start, Vector2 end, float thickness)
    {
        Vector2 direction = Vector2.Normalize(end - start);
        Vector2 perp = new Vector2(-direction.Y, direction.X) * (thickness / 2);
        
        Vector2 v1 = start + perp; // tl
        Vector2 v2 = start - perp; // bl
        Vector2 v3 = end + perp;   // tr
        Vector2 v4 = end - perp;   // br

        float[] vertices =
        [
            v1.X, v1.Y,
            v2.X, v2.Y,
            v3.X, v3.Y,
            v3.X, v3.Y,
            v2.X, v2.Y,
            v4.X, v4.Y
        ];

        DrawPrimitive(vertices, 6, PrimitiveType.Triangles);
    }

    private unsafe static void DrawPrimitive(float[] vertices, int vertexCount, PrimitiveType primitiveType)
    {
        prim_shader.Use();
        opengl.BindVertexArray(prim_vao);
        opengl.BindBuffer(GLEnum.ArrayBuffer, prim_vbo);
        fixed (void* ptr = &vertices[0]) opengl.BufferData(GLEnum.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), ptr, GLEnum.StaticDraw);
        opengl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        opengl.EnableVertexAttribArray(0);
        opengl.DrawArrays(primitiveType, 0, (uint)vertexCount);
        opengl.DisableVertexAttribArray(0);
        opengl.BindBuffer(GLEnum.ArrayBuffer, 0);
        opengl.BindVertexArray(0);
    }

    public static unsafe void DrawSprite(Sprite sprite, Vector2 position, int scale = 1)
    {
        // use and bind
        sprite.shader.Use();
        sprite.texture.Bind();

        // uniforms
        sprite.shader.SetUniformTexture("tex", 0);

        // setup the quad
        Vector2 spritesize = new Vector2(sprite.texture.width, sprite.texture.height) * scale;
        Vector2 sprite_udc_mincorner = (position - spritesize / 2f) / Windowing.Resolution;
        Vector2 sprite_udc_maxcorner = (position + spritesize / 2f) / Windowing.Resolution;

        float[] vertices =
        [
            sprite_udc_mincorner.X, sprite_udc_mincorner.Y, 0, 0,  // bl
            sprite_udc_maxcorner.X, sprite_udc_mincorner.Y, 1, 0,  // br
            sprite_udc_maxcorner.X, sprite_udc_maxcorner.Y, 1, 1,  // tr
            sprite_udc_mincorner.X, sprite_udc_maxcorner.Y, 0, 1   // tl
        ];

        uint[] indices =
        [
            0, 1, 2, // first
            2, 3, 0  // second
        ];

        // vao and vbo
        var temp_vao = opengl.GenVertexArray();
        var temp_vbo = opengl.GenBuffer();
        opengl.BindVertexArray(temp_vao);
        opengl.BindBuffer(GLEnum.ArrayBuffer, temp_vbo);
        fixed (void* ptr = &vertices[0]) opengl.BufferData(GLEnum.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), ptr, GLEnum.StaticDraw);

        // vertex attributes (a_pos and a_uv)
        opengl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0); // mis 2 in plaats van 4
        opengl.EnableVertexAttribArray(0);
        opengl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        opengl.EnableVertexAttribArray(1);

        // ebo
        uint temp_ebo = opengl.GenBuffer();
        opengl.BindBuffer(BufferTargetARB.ElementArrayBuffer, temp_ebo);
        fixed (void* ptr = &indices[0]) opengl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), ptr, BufferUsageARB.StaticDraw);

        // draw
        opengl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

        // cleanup
        opengl.BindTexture(TextureTarget.Texture2D, 0);
        opengl.BindBuffer(GLEnum.ArrayBuffer, 0);
        opengl.BindVertexArray(0);
        opengl.UseProgram(0);
    }
}