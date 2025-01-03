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

        // draw test rectangle
        Drawing.SetColor(Color.Orange);
        Drawing.DrawRectangleFilled(new(100, 100), new(100, 100));

        // draw test thick line
        Drawing.SetColor(Color.Brown);
        Drawing.DrawLine(new(100, 100), Input.GetMousePosition());
    }
}