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
            var range = Range3Single.FromVertices(points);
            var d = Math.Sqrt(range.Dx*range.Dx + range.Dy*range.Dy + range.Dz*range.Dz);
            return Prelude.Tuple(range.Center, (float)d/2);
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