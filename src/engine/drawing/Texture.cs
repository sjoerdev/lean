using Silk.NET.OpenGL;

namespace Lean;

public class Texture
{
    private GL opengl => Drawing.GetOpenGL();
    
    public uint handle;
    public int width;
    public int height;

    public unsafe Texture(byte[] data, int width, int height)
    {
        this.width = width;
        this.height = height;

        // generate and bind
        handle = opengl.GenTexture();
        opengl.BindTexture(TextureTarget.Texture2D, handle);

        // set pixel data
        fixed (void* dataPtr = data) opengl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, (uint)this.width, (uint)this.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dataPtr);

        // set texture parameters
        opengl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
        opengl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
        opengl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        opengl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

        // unbind
        opengl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Bind() => opengl.BindTexture(TextureTarget.Texture2D, handle);
}
