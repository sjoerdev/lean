using System;
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
        // draw stuff here
    }
}