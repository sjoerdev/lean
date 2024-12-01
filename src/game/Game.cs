using System.Drawing;
using System.Numerics;

using Lean;

namespace Game;

public static class Game
{
    static AudioClipWav music;
    static AudioClipWav effect;

    static void Main()
    {
        Windowing.SetupWindow(800, 600, "This is a game window", OnLoad, OnUpdate, OnRender);
    }

    public static void OnLoad()
    {
        effect = new AudioClipWav("res/wav/powerup.wav");
        music = new AudioClipWav("res/wav/thegardens.wav");
        music.Start();
    }

    public static void OnUpdate(float delta)
    {
        // audio test
        if (Input.GetKeyDown(Key.I)) effect.Start();
        if (Input.GetKeyDown(Key.O)) effect.PauseOrContinue();
        if (Input.GetKeyDown(Key.P)) effect.Stop();
    }

    public static void OnRender(float delta)
    {
        // draw background color
        Drawing.SetColor(Color.CornflowerBlue);
        Drawing.ClearWindow();

        // draw red circle in the centre
        Drawing.SetColor(Color.Red);
        Drawing.DrawCircle(Vector2.Zero, 64);

        // draw a white circle at the mouse position
        Drawing.SetColor(Color.White);
        Drawing.DrawCircleFilled(Input.GetMousePosition(), 16);
    }
}