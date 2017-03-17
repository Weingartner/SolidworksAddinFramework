using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Weingartner.WeinCad.Interfaces;

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

        public void Render(DateTime time, IDrawContext drawContext, double parentOpacity = 1.0, Matrix4x4? renderTransform = null)
        {
            if (!Visibility)
                return;

            foreach (var subRenderable in _SubRenderables)
            {
                subRenderable.Render(time, drawContext, Opacity*parentOpacity, renderTransform);
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

        public double Opacity { get; set; } = 1.0;
        public bool Visibility { get; set; } = true;
    }

    public static class CompositeRendererExtensions
    {
        public static CompositeRenderer ToCompositeRenderer(this IEnumerable<IRenderer> @this )=>new CompositeRenderer(@this.ToList());
    }
}