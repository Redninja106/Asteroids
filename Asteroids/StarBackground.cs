using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;

namespace Asteroids;

internal class StarBackground : IRenderable
{
    private const int STAR_COUNT = 250;

    private Camera camera;
    private Star[] stars;

    public StarBackground(Camera camera)
    {
        this.camera = camera;
        stars = new Star[STAR_COUNT];

        var rng = new Random(0);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = new Star(camera, rng);
        }
    }

    public void Render(ICanvas canvas)
    {
        canvas.SetDrawMode(DrawMode.Fill);
        
        Vector2 p = (
                camera.Transform.Position.X,
                 camera.Transform.Position.Y
                );
        p += (
            MathF.Sign(p.X) * camera.Width / 2f,
            MathF.Sign(p.Y) * camera.Height / 2f
            );
        p -= (p.X % camera.Width, p.Y % camera.Height);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].Render(p, (camera.Width, camera.Height), canvas);
        }
    }

    struct Star
    {
        public Vector2 position;
        public float brightness;

        public Star(Camera cam, Random rng)
        {
            position = (rng.NextSingle(), rng.NextSingle());
            brightness = rng.NextSingle();
        }

        public void Render(Vector2 basePosition, Vector2 viewSize, ICanvas canvas)
        {
            canvas.DrawCircle(basePosition + (this.position.X * viewSize.X,              this.position.Y * viewSize.Y             ), brightness / viewSize.X, Color.White);
            canvas.DrawCircle(basePosition + (this.position.X * viewSize.X,              this.position.Y * viewSize.Y - viewSize.Y), brightness / viewSize.X, Color.White);
            canvas.DrawCircle(basePosition + (this.position.X * viewSize.X - viewSize.X, this.position.Y * viewSize.Y             ), brightness / viewSize.X, Color.White);
            canvas.DrawCircle(basePosition + (this.position.X * viewSize.X - viewSize.X, this.position.Y * viewSize.Y - viewSize.Y), brightness / viewSize.X, Color.White);
        }
    }
}
