using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;

namespace Asteroids;

internal class Camera : Entity
{
    public float Width;
    public float AspectRatio => canvas.Width / (float)canvas.Height;
    public float Height => Width / AspectRatio;

    public ICanvas canvas;


    public Camera(float width)
    {
        this.Width = width;
    }

    public void Apply()
    {
        this.canvas.SetTransform(CreateScreenToWorld());
    }

    public Matrix3x2 CreateScreenToWorld()
    {
        return 
            this.Transform.CreateInvMatrix() *
            Matrix3x2.CreateScale(1f, -1f) *
            Matrix3x2.CreateScale(canvas.Width / this.Width) *
            Matrix3x2.CreateTranslation(canvas.Width / 2f, canvas.Height / 2f);
    }

    public Matrix3x2 CreateWorldToScreen()
    {
        Matrix3x2.Invert(CreateScreenToWorld(), out Matrix3x2 result);
        return result;
    }
}
