using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Timers;
using SimulationFramework;
using SimulationFramework.IMGUI;

namespace Asteroids;

internal class AsteroidSpawner : Entity
{
    Timer timer;
    Camera camera;
    Ship target;
    float difficulty = 0;

    public AsteroidSpawner(Camera camera, Ship target)
    {
        timer = new Timer();
        this.camera = camera;
        this.target = target;
        timer.Interval = GetInterval();
        timer.Elapsed += Timer_Elapsed;
        timer.Start();

    }

    public void UpdateDifficulty(float difficulty)
    {
        this.difficulty = difficulty;
        timer.Interval = GetInterval();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        var a = Random.Shared.NextSingle() * MathF.Tau;
        Vector2 asteroidPosition = target.Transform.Position + camera.Width / 2f * new Vector2(MathF.Cos(a), MathF.Sin(a));

        World.AddEntity(new Asteroid(1f, asteroidPosition, Vector2.Normalize(target.Transform.Position - asteroidPosition) * (1 + Random.Shared.NextSingle() * 2), Random.Shared.NextSingle() * 2 - 1));

        Console.WriteLine("Spawned Asteroid");
    }

    public override void Update()
    {
        if (Keyboard.IsKeyPressed(Key.Plus))
        {
            UpdateDifficulty(difficulty + 1);
        }
        if (Keyboard.IsKeyPressed(Key.Minus))
        {
            UpdateDifficulty(difficulty - 1);
        }
        ImGui.Text("Difficulty: " + difficulty);
    }

    public float GetInterval()
    {
        return 1000 * MathF.Pow(1.1f, -difficulty);
    }
}