using System.Drawing;
using System.Numerics;
using Lean;

public static class Game
{
    static Sprite sprite;
    static AudioClipWav music;

    static void Main()
    {
        // setup window
        Windowing.SetupWindow(800, 600, "This is a game window", OnLoad, OnUpdate, OnRender);
    }

    public static void OnLoad()
    {
        // load resources
        sprite = new Sprite("res/png/player.png");
        music = new AudioClipWav("res/wav/thegardens.wav");
        music.Start();
    }

    public static void OnUpdate(float delta)
    {
        // pause music with space
        if (Input.GetKeyDown(Key.Space)) music.PauseOrContinue();
    }

    public static void OnRender(float delta)
    {
        // draw background color
        Drawing.SetColor(Color.LightGray);
        Drawing.ClearWindow();

        // draw sprite
        Drawing.DrawSprite(sprite, Vector2.Zero, 8);

        // draw a white circle at the mouse position
        Drawing.SetColor(Color.Black);
        Drawing.DrawCircleFilled(Input.GetMousePosition(), 8);
    }
}