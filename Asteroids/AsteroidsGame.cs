using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.IMGUI;

namespace Asteroids;

internal sealed class AsteroidsGame : Simulation
{
    private List<Entity> entities = new();
    public static Camera camera;

    private StarBackground background;
    private World world;

    public override void OnInitialize(AppConfig config)
    {
        world = new();

        var ship = new Ship();
        camera = new Camera(50);

        world.AddEntity(ship);
        world.AddEntity(camera);
        world.AddEntity(new AsteroidSpawner(camera, ship));
    }


    public override void OnRender(ICanvas canvas)
    {
        camera.canvas = canvas;
        Renderer.Submit(background ??= new StarBackground(camera));

        world.Update();

        canvas.Clear(Color.Black);
        camera.Apply();
        Polygon.DrawTest(canvas);

        Renderer.RenderAll(canvas);
    }
}