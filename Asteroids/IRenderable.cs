using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;

namespace Asteroids
{
    internal interface IRenderable
    {
        void Render(ICanvas canvas);
    }
}