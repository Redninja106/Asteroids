using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;

namespace Asteroids;

public struct Transform
{
    public static readonly Transform Default = new Transform
    {
        Position = Vector2.Zero,
        Rotation = 0f,
        Scale = Vector2.One,
    };

    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;

    public Transform(Vector2 position, float rotation, Vector2 scale)
    {
        this.Position = position;
        this.Rotation = rotation;
        this.Scale = scale;
    }

    public Vector2 Forward 
    { 
        get => VecFromAngle(Rotation);
        set => Rotation = MathF.Atan2(value.Y, value.X);
    }

    public Vector2 Right
    {
        get => VecFromAngle(Rotation - MathF.PI / 2);
        set => Rotation = MathF.Atan2(value.Y, value.X) + MathF.PI / 2f;
    }

    public Vector2 Left
    {
        get => VecFromAngle(Rotation + MathF.PI / 2f);
        set => Rotation = MathF.Atan2(Position.Y, Position.X) - MathF.PI / 2f;
    }

    public Vector2 Backward
    {
        get => VecFromAngle(Rotation + MathF.PI);
        set => Rotation = MathF.Atan2(Position.Y, Position.X) - MathF.PI;
    }

    private Vector2 VecFromAngle(float angle)
    {
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public void Apply(ICanvas canvas)
    {
        canvas.Translate(Position);
        canvas.Rotate(Rotation);
        canvas.Scale(Scale);
    }

    public Matrix3x2 CreateMatrix()
    {
        return
            Matrix3x2.CreateScale(this.Scale) *
            Matrix3x2.CreateRotation(this.Rotation) *
            Matrix3x2.CreateTranslation(this.Position);
    }

    public Matrix3x2 CreateInvMatrix()
    {
        Matrix3x2.Invert(CreateMatrix(), out Matrix3x2 result);
        return result;
    }

    public void LookAt(Vector2 position)
    {
        this.Forward = position - this.Position;
    }

    public Vector2 WorldToLocal(Vector2 point)
    {
        return Vector2.Transform(point, CreateInvMatrix());
    }

    public Vector2 LocalToWorld(Vector2 point)
    {
        return Vector2.Transform(point, CreateMatrix());
    }

    public float DistanceFrom(Transform other)
    {
        return DistanceFrom(other.Position);
    }

    public float DistanceFrom(Vector2 point)
    {
        return (this.Position - point).Length();   
    }
}