using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;

namespace Asteroids;

internal static class Renderer
{
    private static readonly List<(float, IRenderable)> renderingObjects = new();

    public static void Submit(IRenderable renderable, float depth = 0)
    {
        renderingObjects.Add((depth, renderable));
    }

    public static void RenderAll(ICanvas canvas)
    {
        for (int i = 0; i < renderingObjects.Count; i++)
        {
            canvas.PushState();
            renderingObjects[i].Item2.Render(canvas);
            canvas.PopState();
        }

        renderingObjects.Clear();
    }
}
