using System.Drawing;
using System.Numerics;

using Lean;

namespace Game;

public static class Game
{
    static AudioClipWav testaudio;

    static void Main()
    {
        Windowing.SetupWindow(800, 600, "This is a game window", OnLoad, OnUpdate, OnRender);
    }

    public static void OnLoad()
    {
        testaudio = new AudioClipWav("res/wav/powerup.wav");
    }

    public static void OnUpdate(float delta)
    {
        if (Input.GetKeyDown(Key.I)) testaudio.Start();
        if (Input.GetKeyDown(Key.O)) testaudio.PauseOrContinue();
        if (Input.GetKeyDown(Key.P)) testaudio.Stop();
    }

    public static void OnRender(float delta)
    {
        Drawing.SetColor(Color.CornflowerBlue);
        Drawing.ClearWindow();
        Drawing.SetColor(Color.Red);
        Drawing.DrawCircle(Vector2.Zero, 64);
    }
}