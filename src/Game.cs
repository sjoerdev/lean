using System;
using System.Numerics;

class Game
{
    static void Main()
    {
        Engine.SetupWindow(new Vector2(1280, 720), "Spank Engine Game");
        Engine.SetupCallbacks(OnLoad, OnUpdate, OnRender);
        Engine.RunWindow();
    }

    static void OnLoad()
    {
        // load resources here
    }

    static void OnUpdate(float deltaTime)
    {
        if (Engine.GetKeyDown(Key.Space)) Console.WriteLine("Space");
    }

    static void OnRender(float deltaTime)
    {
        // draw stuff here
    }
}