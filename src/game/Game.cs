using System;
using System.Drawing;
using System.Numerics;

using Engine;

class Game
{
    static void Main()
    {
        Windowing.CreateWindow(800, 600, "Game", OnLoad, OnUpdate, OnRender);
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

        //Drawing.DrawCircle(Vector2.Zero, 64, 50);
        Drawing.DrawCircleFilled(Vector2.Zero, 64, 50);

        //Drawing.DrawEllipse(Vector2.Zero, new Vector2(64, 32), 50);
        //Drawing.DrawEllipseFilled(Vector2.Zero, new Vector2(64, 32), 50);

        //Drawing.DrawRectangle(Vector2.Zero, new Vector2(32, 32));

        Windowing.SwapBuffers();
    }
}