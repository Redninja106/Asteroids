using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;

namespace Asteroids;

internal class Bullet : Entity, IRenderable, ICollidable, IDestructable
{
    Vector2 velocity;
    Ship shooter;
    private const float SIZE = 0.1f;
    private static readonly Vector2[] bulletPoly = new Vector2[] 
    { 
        (-SIZE/2f, -SIZE / 2f), 
        (SIZE / 2f, -SIZE / 2f), 
        (SIZE / 2f, SIZE / 2f), 
        (-SIZE / 2f, SIZE / 2f), 
        (-SIZE / 2f, -SIZE / 2f) 
    };

    public Bullet(Ship shooter, Vector2 position, Vector2 velocity)
    {
        this.shooter = shooter;
        this.Transform.Position = position;
        this.velocity = velocity;
    }

    public Vector2[] GetCollider()
    {
        return bulletPoly;
    }

    public void Render(ICanvas canvas)
    {
        this.Transform.Apply(canvas);
        canvas.DrawRect(0, 0, SIZE, SIZE, Color.White, Alignment.Center);
    }

    public override void Update()
    {
        if (World.IsColliding(this, out var collisions))
        {
            foreach (var other in collisions)
            {
                if (other is IDestructable destructable)
                {
                    destructable.Destroy();
                    this.Destroy();
                }
            }
        }

        if ((shooter.Transform.Position - this.Transform.Position).LengthSquared() > 100 * 100)
        {
            World.Remove(this);
        }

        this.Transform.Position += this.velocity * Time.DeltaTime;

        Renderer.Submit(this);
    }

    public void Destroy()
    {
        World.Remove(this);
    }
}
