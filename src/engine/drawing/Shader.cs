using System;
using System.IO;
using System.Numerics;

using Silk.NET.OpenGL;

namespace Lean;

public class Shader
{
    private GL opengl => Drawing.GetOpenGL();

    public uint handle;

    public Shader(string vertpath, string fragpath)
    {
        // read
        string vertsource = File.ReadAllText(vertpath);
        string fragsource = File.ReadAllText(fragpath);

        // compile
        uint vertshader = CompileShader(ShaderType.VertexShader, vertsource);
        uint fragshader = CompileShader(ShaderType.FragmentShader, fragsource);

        // combine
        handle = opengl.CreateProgram();
        opengl.AttachShader(handle, vertshader);
        opengl.AttachShader(handle, fragshader);
        opengl.LinkProgram(handle);

        // errors
        opengl.GetProgram(handle, GLEnum.LinkStatus, out int status);
        var log = opengl.GetProgramInfoLog(handle);
        if (status == 0) throw new Exception(log);

        // cleanup
        opengl.DeleteShader(vertshader);
        opengl.DeleteShader(fragshader);
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = opengl.CreateShader(type);
        opengl.ShaderSource(shader, source);
        opengl.CompileShader(shader);
        
        opengl.GetShader(shader, GLEnum.CompileStatus, out int status);
        var log = opengl.GetShaderInfoLog(shader);
        if (status == 0) throw new Exception(log);

        return shader;
    }

    public void Use()
    {
        opengl.UseProgram(handle);
    }

    public void SetUniform1(string name, float value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform1(location, value);
    }

    public void SetUniform2(string name, Vector2 value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform2(location, value.X, value.Y);
    }

    public void SetUniform3(string name, Vector3 value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetUniform4(string name, Vector4 value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    public unsafe void SetUniformMatrix4(string name, Matrix4x4 matrix)
    {
        int location = opengl.GetUniformLocation(handle, name);

        float[] converted =
        [
            matrix.M11, matrix.M21, matrix.M31, matrix.M41,
            matrix.M12, matrix.M22, matrix.M32, matrix.M42,
            matrix.M13, matrix.M23, matrix.M33, matrix.M43,
            matrix.M14, matrix.M24, matrix.M34, matrix.M44,
        ];

        fixed (float* ptr = &converted[0]) opengl.UniformMatrix4(location, 1, false, ptr);
    }

    public unsafe void SetUniformTexture(string name, int unit)
    {
        opengl.ActiveTexture(TextureUnit.Texture0 + unit);
        opengl.Uniform1(opengl.GetUniformLocation(handle, name), unit);
    }
}