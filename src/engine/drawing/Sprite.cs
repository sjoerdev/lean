using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Lean;

public class Sprite
{
    public Texture texture;
    public Shader shader;

    public Sprite(string path)
    {
        // load image data using imagesharp
        using var image = Image.Load<Rgba32>(path);
        image.Mutate(x => x.Flip(FlipMode.Vertical));
        var data = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(data);

        // generate opengl texture from image data
        texture = new Texture(data, image.Width, image.Height);

        // generate shader
        shader = new Shader("res/glsl/sprite-vert.glsl", "res/glsl/sprite-frag.glsl");
    }
}