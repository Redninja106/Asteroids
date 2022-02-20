using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;

namespace Asteroids;

internal interface ICollidable
{
    Vector2[] GetCollider();
}