using System;
using System.Collections.Generic;
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
        Tuple<Vector3, float> BoundingSphere { get; }
    }

    public static class Renderable
    {
        public static Tuple<Vector3,float> BoundingSphere(IReadOnlyList<Vector3> points)
        {
            var range = Range3Single.FromVertices(points);
            return range.BoundingSphere();
        }
    }
}