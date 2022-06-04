using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;
using SimulationFramework.IMGUI;

namespace Asteroids;

internal class World : IRenderable
{
    private readonly List<Entity> entities = new();
    private readonly Queue<Entity> newEntityQueue = new();
    private readonly Queue<Entity> oldEntityQueue = new();
    private bool updating;
    private bool renderColliders;

    public World()
    {

    }

    public void AddEntity(Entity entity)
    {
        entity.SetWorld(this);
        if (updating)
            this.newEntityQueue.Enqueue(entity);
        else
            this.entities.Add(entity);
    }

    public T FindEntity<T>() where T : Entity
    {
        return (T)entities.FirstOrDefault(e=> e.GetType() == typeof(T));
    }
    public IEnumerable<T> FindEntities<T>() where T : Entity
    {
        return entities.Where(e => e.GetType() == typeof(T)).Cast<T>();
    }

    public void Update()
    {
        updating = true;
        entities.ForEach(e => e.Update());
        updating = false;

        while (newEntityQueue.Count > 0)
            entities.Add(newEntityQueue.Dequeue());
        while (oldEntityQueue.Count > 0)
            entities.Remove(oldEntityQueue.Dequeue());

        Renderer.Submit(this);
    }

    public void Remove(Entity entity)
    {
        if (!updating)
            this.entities.Remove(entity);
        else
            oldEntityQueue.Enqueue(entity);

    }

    public bool IsColliding(Entity entity, out List<Entity> collisions)
    {
        collisions = new List<Entity>();
        
        if (entity is not ICollidable collidable) 
            return false;  

        foreach (var other in entities)
        {
            if (entity != other && other is ICollidable otherCollidable)
            {
                if (Polygon.Collide(collidable.GetCollider(), entity.Transform, otherCollidable.GetCollider(), other.Transform))
                { 
                    collisions.Add(other);
                }
            }
        }

        return collisions.Any();
    }

    public void Render(ICanvas canvas)
    {
        ImGui.CheckBox("Render Colliders", ref this.renderColliders);

        if (renderColliders)
        {
            canvas.StrokeWidth(.05f);
            
            foreach (var e in entities)
            {
                if (e is ICollidable c)
                {
                    canvas.PushState();
                    e.Transform.Apply(canvas);
                    var collider = c.GetCollider();
                    canvas.Stroke(Color.DarkRed);
                    canvas.DrawPolygon(collider);
                    var convexCollider = new List<Vector2>(collider);
                    Polygon.MakeConvex(convexCollider);
                    canvas.Stroke(Color.Red);
                    canvas.DrawPolygon(convexCollider);
                    canvas.PopState();
                }
            }
        }
    }
}
