using System.Drawing;
using System.Numerics;

using Engine;

public static class Entry
{
    static void Main()
    {
        var game = new Game();
        Windowing.CreateWindow(800, 600, "This is a game window", game.OnLoad, game.OnUpdate, game.OnRender);
    }
}

public class Game
{
    AudioClipWav testaudio;

    public void OnLoad()
    {
        testaudio = new AudioClipWav("src/game/sound/powerup.wav");
    }

    public void OnUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(Key.Space)) testaudio.Play();
    }

    public void OnRender(float deltaTime)
    {
        Drawing.SetColor(Color.CornflowerBlue);
        Drawing.ClearWindow();

        Drawing.SetColor(Color.Red);
        Drawing.DrawCircle(Vector2.Zero, 64);

        Windowing.SwapBuffers();
    }
}