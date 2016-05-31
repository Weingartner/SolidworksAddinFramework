using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LanguageExt;
using SolidWorks.Interop.sldworks;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.Geometry;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IRenderable
    {
        void Render(DateTime time);
        void ApplyTransform(Matrix4x4 transform);
        Tuple<Vector3, float> BoundingSphere();
    }

    public static class Renderable
    {
        public static Tuple<Vector3,float> BoundingSphere(IReadOnlyList<Vector3> points)
        {
            var avg = points.Aggregate(Vector3.Zero, (a, b) => a + b)/points.Count;
            var radius = points.Max(p => (p - avg).Length());
            return Prelude.Tuple(new Vector3(avg.X,avg.Y,0), radius);
        }

        public static IEnumerable<Vector3> Points(this IEnumerable<Triangle> @this) => @this.SelectMany
            (t => new[]
            {
                t.A,
                t.B,
                t.C
            });
        public static IEnumerable<PointDirection3> Points(this IEnumerable<TriangleWithNormals> @this) => @this.SelectMany
            (t => new[]
            {
                t.A,
                t.B,
                t.C
            });
    }
}