using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Drawing;
using System.Linq;
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
        Tuple<Vector3, double> BoundingSphere { get; }
    }

    public class CompositeRenderable : IRenderable
    {
        private readonly IReadOnlyList<IRenderable> _SubRenderables;

        public CompositeRenderable(IReadOnlyList<IRenderable> subRenderables)
        {
            _SubRenderables = subRenderables;
        }

        public CompositeRenderable(params IRenderable[] subRenderables) : this(subRenderables.ToList())
        {
        }

        public void Render(DateTime time)
        {
            foreach (var subRenderable in _SubRenderables)
            {
                subRenderable.Render(time);
            }
        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            foreach (var subRenderable in _SubRenderables)
            {
                   subRenderable.ApplyTransform(transform);
            }
        }

        public Tuple<Vector3, double> BoundingSphere
        {
            get
            {
              throw  new NotSupportedException("");
            }
        } 
    }

    public class EdgeList : IRenderable
    {
        private readonly Color _Color;
        private readonly IReadOnlyList<Edge3> _Edges;

        public EdgeList(IEnumerable<Edge3> edges, Color color)
        {
            _Color = color;
            _Edges = edges.ToList();
        }

        public void Render(DateTime time)
        {
            using (ModernOpenGl.SetColor(_Color, ShadingModel.Smooth, solidBody: false))
            using (ModernOpenGl.SetLineWidth(2.0))
            using (ModernOpenGl.Begin(PrimitiveType.Lines))
                foreach (var v in _Edges)
                {
                    v.A.GLVertex3();
                    v.B.GLVertex3();
                }
        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            throw new NotImplementedException();
        }

        public Tuple<Vector3, double> BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class Renderable
    {
        public static Tuple<Vector3,double> BoundingSphere(IReadOnlyList<Vector3> points)
        {
            var range = Range3Single.FromVertices(points);
            return range.BoundingSphere();
        }
    }
}