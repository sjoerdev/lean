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

        int segments = 8;

        Drawing.DrawCircle(new Vector2(-250, 0), 50, segments);
        Drawing.DrawCircleFilled(new Vector2(-100, 0), 50, segments);

        Drawing.DrawEllipse(new Vector2(100, 0), new Vector2(40, 50), segments);
        Drawing.DrawEllipseFilled(new Vector2(250, 0), new Vector2(40, 50), segments);

        Windowing.SwapBuffers();
    }
}