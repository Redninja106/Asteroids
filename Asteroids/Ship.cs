using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;

namespace Asteroids;

internal class Ship : Entity, IRenderable, IDestructable, ICollidable
{
    private static readonly Vector2[] shipPoly = new Vector2[] { (1, 0), (-1, .5f), (-.8f, 0), (-1, -.5f), (1, 0), };
    private static readonly Vector2[] thrustPoly = new Vector2[] { (-.1f, .25f),  (-.7f, 0f), (-.1f, -.25f), (0, 0) };

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

        using (canvas.Push())
        {
            canvas.SetDrawMode(DrawMode.Fill);
            canvas.Translate(-.8f, 0);
            canvas.Scale(thrustAnimationState * thrustAnimationState);
            canvas.DrawPolygon(thrustPoly, Color.White);
        }

        canvas.SetDrawMode(DrawMode.Fill);
        canvas.DrawPolygon(shipPoly, Color.Black);

        canvas.SetDrawMode(DrawMode.Border);
        canvas.SetStrokeWidth(.05f);
        canvas.DrawPolygon(shipPoly, Color.White);
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
            velocity = velocity.Normalized() * maxSpeed;
        }

        if (Mouse.IsButtonPressed(MouseButton.Left))
        {
            World.AddEntity(new Bullet(this, this.Transform.Position + this.Transform.Forward * 1.5f, this.velocity + this.Transform.Forward * 20));
        }

        Renderer.Submit(this);
    }
}