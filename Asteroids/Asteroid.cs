using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;

namespace Asteroids;

internal class Asteroid : Entity, IRenderable, ICollidable, IDestructable
{
    private Vector2[] asteroidPoly;
     
    public Asteroid(float scale, Vector2 position, Vector2 velocity, float angularVelocity)
    {
        this.Transform.Position = position;
        this.Transform.Scale = new Vector2(scale);
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;

        const int detail = 16;
        const float jaggedness = .1f;
        const float variability = 1f;
        const float shapeModifierStrength = 1f;

        asteroidPoly = new Vector2[detail + 1];

        float shapeModifier = Random.Shared.NextSingle() * shapeModifierStrength;
        for (int i = 0; i < asteroidPoly.Length - 1; i++)
        {
            var angle = MathF.Tau * (i / ((float)asteroidPoly.Length - 1));

            float length = 1f;
            length += MathF.Sin(shapeModifier * angle + Random.Shared.NextSingle()) * variability + Random.Shared.NextSingle() * jaggedness;

            asteroidPoly[i] = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * length;
        }

        asteroidPoly[detail] = asteroidPoly[0];
    }

    private Vector2 velocity;
    private float angularVelocity;

    public void Render(ICanvas canvas)
    {
        Transform.Apply(canvas);
        canvas.Stroke(Color.White);
        canvas.StrokeWidth(.05f);
        canvas.DrawPolygon(asteroidPoly);

    }

    public override void Update()
    {
        this.Transform.Position += velocity * Time.DeltaTime;
        this.Transform.Rotation += angularVelocity * Time.DeltaTime;
        this.velocity *= MathF.Pow(.99f, Time.DeltaTime);

        if (World.IsColliding(this, out var collisions))
        {
            foreach (var other in collisions)
            {
                if (World.FindEntities<Ship>().Contains(other))
                {
                    (other as Ship).Destroy();
                }
                if (other is Asteroid asteroid)
                {
                    asteroid.Destroy();
                }
            }
        }
        var ship = World.FindEntity<Ship>();
        if (ship != null)
        {
            if (this.Transform.DistanceFrom(ship.Transform) > World.FindEntity<Camera>().Width)
            {
                this.Destroy();
            }
        }

        Renderer.Submit(this, 0);
    }

    public Vector2[] GetCollider()
    {
        return asteroidPoly;
    }

    public void Destroy()
    {
        World.Remove(this);
        var scale = this.Transform.Scale.X - 1 / 3f;

        if (scale <= 0.01f)
            return;

        World.AddEntity(CreateAsteroid(0 * MathF.Tau / 3, scale));
        World.AddEntity(CreateAsteroid(1 * MathF.Tau / 3, scale));
        World.AddEntity(CreateAsteroid(2 * MathF.Tau / 3, scale));
    }

    private Asteroid CreateAsteroid(float angle, float scale)
    {
        Vector2 dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 2.5f * scale;
        return new Asteroid(scale, this.Transform.Position + dir, (dir + dir * Random.Shared.NextSingle()), Random.Shared.NextSingle());
    }
}
