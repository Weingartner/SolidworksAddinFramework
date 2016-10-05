using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Linq;
using System.Reactive.Linq;
using LanguageExt;

namespace SolidworksAddinFramework.OpenGl
{
    public class CompositeRenderer : IRenderer
    {
        private readonly IReadOnlyList<IRenderer> _SubRenderables;

        public CompositeRenderer(IReadOnlyList<IRenderer> subRenderables)
        {
            _SubRenderables = subRenderables;
        }

        public CompositeRenderer(params IRenderer[] subRenderers) : this(subRenderers.ToList())
        {
        }

        public IObservable<Unit> NeedsRedraw => _SubRenderables.Select(v => v.NeedsRedraw).Merge();

        public void Render(DateTime time, Matrix4x4? renderTransform)
        {
            foreach (var subRenderable in _SubRenderables)
            {
                subRenderable.Render(time, renderTransform);
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

    public static class CompositeRendererExtensions
    {
        public static CompositeRenderer ToCompositeRenderer(this IEnumerable<IRenderer> @this )=>new CompositeRenderer(@this.ToList());
    }
}