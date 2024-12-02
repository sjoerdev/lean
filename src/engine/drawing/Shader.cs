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
        // read shader source code
        string vertsource = File.ReadAllText(vertpath);
        string fragsource = File.ReadAllText(fragpath);

        // compile vert and frag
        uint vertexshader = CompileShader(GLEnum.VertexShader, vertsource);
        uint fragmentshader = CompileShader(GLEnum.FragmentShader, fragsource);

        // combine into shader program
        handle = opengl.CreateProgram();
        opengl.AttachShader(handle, vertexshader);
        opengl.AttachShader(handle, fragmentshader);
        opengl.LinkProgram(handle);

        // deal with errors
        opengl.GetProgram(handle, GLEnum.LinkStatus, out int status);
        if (status == 0) throw new Exception(opengl.GetProgramInfoLog(handle));

        // delete vert and frag
        opengl.DeleteShader(vertexshader);
        opengl.DeleteShader(fragmentshader);
    }

    private uint CompileShader(GLEnum type, string source)
    {
        uint shader = opengl.CreateShader(type);
        opengl.ShaderSource(shader, source);
        opengl.CompileShader(shader);
        
        opengl.GetShader(shader, GLEnum.CompileStatus, out int status);
        if (status == 0) throw new Exception(opengl.GetShaderInfoLog(shader));

        return shader;
    }

    public void Use()
    {
        opengl.UseProgram(handle);
    }

    public void SetUniform(string name, float value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector2 value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform2(location, value.X, value.Y);
    }

    public void SetUniform(string name, Vector3 value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = opengl.GetUniformLocation(handle, name);
        opengl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    public unsafe void SetUniformMatrix(string name, Matrix4x4 matrix)
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
}