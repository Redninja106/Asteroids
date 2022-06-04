using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework;
using SimulationFramework.Drawing.Canvas;
using SimulationFramework.IMGUI;

namespace Asteroids;

internal static class Polygon
{
    // i dont want to deal with complex algorithms for this. just drop inflexive points in order to approximate the shape
    public static void MakeConvex(List<Vector2> polygon)
    {
        var inflexives = InflexivePoints(polygon);

        if (inflexives.Count == 0)
        {
            if (polygon[0] != polygon[^1])
                polygon.Add(polygon[0]);
            return;
        }

        for (int i = 0; i < inflexives.Count; i++)
        {
            polygon.RemoveAt(inflexives[i]);

            for (int j = i; j < inflexives.Count; j++)
            {
                inflexives[j]--;
            }
        }

        MakeConvex(polygon);
    }

    public static bool Collide(Vector2[] polyA, Transform transformA, Vector2[] polyB, Transform transformB)
    {
        if (polyA.Length == 0 || polyB.Length == 0)
            return false;

        List<Vector2> convexPolyA = new(polyA);
        MakeConvex(convexPolyA);
        List<Vector2> convexPolyB = new(polyB);
        MakeConvex(convexPolyB);

        for (int shape = 0; shape < 2; shape++)
        {
            if (shape == 1)
            {
                (polyA, polyB) = (polyB, polyA);
            }
            for (int i = 0; i < polyA.Length - 1; i++)
            {
                var diff = polyA[i] - polyA[i + 1];
                Vector2 axis = new(-diff.Y, diff.X);
                var pa = Project(axis, polyA, transformA);
                var pb = Project(axis, polyB, transformB);

                if (!Overlap(pa, pb))
                {
                    return false;
                }
            }
        }

        return true;
    }

    static (float min, float max) Project(Vector2 axis, Span<Vector2> points, Transform transform)
    {
        float min = Vector2.Dot(axis, transform.LocalToWorld(points[0]));
        float max = min;

        for (int i = 1; i < points.Length; i++)
        {
            float dot = Vector2.Dot(axis, transform.LocalToWorld(points[i]));
            if (dot < min) 
                min = dot;
            else if (dot > max) 
                max = dot;
        }

        return (min, max);
    }

    static bool Overlap((float min, float max) a, (float min, float max) b)
    {
        return a.max > b.min &&
        a.min <  b.max;
    }

    public static List<int> InflexivePoints(List<Vector2> poly, ICanvas canvas = null)
    {
        bool gui = canvas != null;
        List<int> result = new();

        if (poly.Count < 4)
            return result;

        Vector2 a, b, c;

        a = poly[0];
        b = poly[1];

        for (int i = 2; i < poly.Count; i++)
        {
            c = poly[i];

            var ab = a - b;
            var abp = new Vector2(ab.Y, -ab.X);
            var bc = b - c;

            if (Vector2.Dot(abp, bc) > 0)
                result.Add(i - 1);

            a = b;
            b = c;
        }

        return result;
    }

    private static List<Vector2> a = new();
    public static void DrawTest(ICanvas canvas)
    {
        //PolyEdit(a, "a");

        //using (canvas.Push()) 
        //{
        //    canvas.SetStrokeWidth(0.05f);
        //    canvas.SetDrawMode(DrawMode.Border);
        //    if (a.Count > 3)
        //    canvas.DrawPolygon(a, Color.White);

        //    var inflexives = InflexivePoints(CollectionsMarshal.AsSpan(a), canvas);
        //    foreach (var inflexive in inflexives)
        //    {
        //        canvas.DrawCircle(a[inflexive], .1f, Color.Red);
        //    }
        //    if (convexPolyA.Length > 3)
        //        canvas.DrawPolygon(convexPolyA, Color.Gray);
        //}
    }

    private static void PolyEdit(List<Vector2> poly, string name)
    {
        if (ImGui.TreeNode(name))
        {
            if (ImGui.Button("+"))
                poly.Add(Vector2.Zero);
            ImGui.SameLine();
            if (ImGui.Button("-"))
                poly.RemoveAt(poly.Count - 1);

            var s = CollectionsMarshal.AsSpan(poly);

            if (ImGui.BeginListBox(name, default))
            {
                for (int i = 0; i < poly.Count; i++)
                {
                    ImGui.DragFloat(new string(' ', i), ref s[i]);
                }
                ImGui.EndListBox();
            }
            ImGui.TreePop();
        }
    }
}
