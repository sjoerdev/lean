using System;
using System.Drawing;
using System.Numerics;
using Engine;

class Game
{
    static void Main()
    {
        Window.Create(800, 600, "Game");
        Window.SetupCallbacks(OnLoad, OnUpdate, OnRender);
        Window.Run();
    }

    static void OnLoad()
    {
        // load resources here
    }

    static void OnUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(Silk.NET.Input.Key.Space)) Console.WriteLine("Space");
    }

    static void OnRender(float deltaTime)
    {
        Drawing.SetColor(Color.CornflowerBlue);
        Drawing.ClearWindow();

        Drawing.SetColor(Color.Red);
        Drawing.DrawCircle(Vector2.Zero, 64, 32);

        Window.SwapBuffers();
    }
}