using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Linq;
using LanguageExt;
using SolidWorks.Interop.sldworks;
using SolidworksAddinFramework.Geometry;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IRenderable
    {
        void Render(DateTime time);

        /// <summary>
        /// Temporarily transforms the object. Subsequent calls to this method are not cumulative.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="accumulate"></param>
        void ApplyTransform(Matrix4x4 transform, bool accumulate=false);
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

        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {
            foreach (var subRenderable in _SubRenderables)
            {
                   subRenderable.ApplyTransform(transform, accumulate);
            }
        }


        public Tuple<Vector3, double> BoundingSphere
        {
            get
            {
              throw new NotSupportedException("");
            }
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