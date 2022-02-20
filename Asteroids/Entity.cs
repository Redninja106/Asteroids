using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids;

internal class Entity
{
    public World World { get; private set; }

    public Transform Transform = Transform.Default;

    public virtual void Update()
    {
    }

    internal void SetWorld(World world)
    {
        this.World = world; 
    }
}