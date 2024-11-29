using System.Drawing;
using System.Numerics;

using Engine;

public static class Entry
{
    static void Main()
    {
        Game game = new Game();
        Windowing.CreateWindow(800, 600, "Game", game.OnLoad, game.OnUpdate, game.OnRender);
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
        if (Input.GetKeyDown(Key.Space)) Audio.PlayAudioClipWav(testaudio);
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