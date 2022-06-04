using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;

namespace Asteroids;

internal class Ship : Entity, IRenderable, IDestructable, ICollidable
{
    private static readonly Vector2[] shipPoly = new Vector2[] { new(1, 0), new(-1, .5f), new(-.8f, 0), new(-1, -.5f), new(1, 0), };
    private static readonly Vector2[] thrustPoly = new Vector2[] { new(-.1f, .25f),  new(-.7f, 0f), new(-.1f, -.25f), new(0, 0), new(-.1f, .25f) };

    private Vector2 velocity;
    private float acceleration = 25;
    private float drag = 0.1f;
    private float maxSpeed = 25f;
    private float thrustAnimationState;

    public void Destroy()
    {
        Console.WriteLine("Ship Destroyed!");
        World.Remove(this);
    }

    public Vector2[] GetCollider()
    {
        return shipPoly;
    }

    public void Render(ICanvas canvas)
    {
        this.Transform.Apply(canvas);
        
        if (Keyboard.IsKeyDown(Key.Space))
        {
            thrustAnimationState += Time.DeltaTime * 10;
            thrustAnimationState = MathF.Min(thrustAnimationState, 1);
        }
        else
        {
            thrustAnimationState -= Time.DeltaTime * 10;
            thrustAnimationState = MathF.Max(thrustAnimationState, 0);
        }

        canvas.PushState();
        canvas.Translate(-.8f, 0);
        canvas.Scale(thrustAnimationState * thrustAnimationState);
        canvas.StrokeWidth(0.05f);
        canvas.Stroke(Color.White);
        canvas.DrawPolygon(thrustPoly);
        canvas.PopState();
        
        canvas.Fill(Color.Black);
        canvas.DrawPolygon(shipPoly);

        canvas.Stroke(Color.White);
        canvas.StrokeWidth(.05f);
        canvas.DrawPolygon(shipPoly);
    }

    public override void Update()
    {
        var mousePos = Vector2.Transform(Mouse.Position, AsteroidsGame.camera.CreateWorldToScreen());

        this.Transform.LookAt(mousePos);
        AsteroidsGame.camera.Transform.Position = this.Transform.Position;

        this.Transform.Position += velocity * Time.DeltaTime;

        if (Keyboard.IsKeyDown(Key.Space))
        {
            velocity += this.Transform.Forward * acceleration * Time.DeltaTime;
        }

        velocity *= MathF.Pow(drag, Time.DeltaTime);

        if (velocity.LengthSquared() > maxSpeed * maxSpeed)
        {
            velocity = Vector2.Normalize(velocity) * maxSpeed;
        }

        if (Mouse.IsButtonPressed(MouseButton.Left))
        {
            World.AddEntity(new Bullet(this, this.Transform.Position + this.Transform.Forward * 1.5f, this.velocity + this.Transform.Forward * 20));
        }

        Renderer.Submit(this);
    }
}