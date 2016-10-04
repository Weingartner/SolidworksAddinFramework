using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Reactive.Linq;
using LanguageExt;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SolidWorks.Interop.sldworks;
using SolidworksAddinFramework.Geometry;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IRenderer
    {
        IObservable<Unit> NeedsRedraw { get; }
        void Render(DateTime time);

        /// <summary>
        /// Temporarily transforms the object. Subsequent calls to this method are not cumulative.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="accumulate"></param>
        void ApplyTransform(Matrix4x4 transform, bool accumulate=false);
        Tuple<Vector3, double> BoundingSphere { get; }
    }

    public interface IRenderable
    {
        IRenderer GetRenderer();
    }

    /// <summary>
    /// A Renderer that can switch between two sub objects
    /// </summary>
    public class EitherRenderable : ReactiveObject, IRenderable
    {
        private IRenderer _A;
        private IRenderer _B;

        private SerialRenderer _Target = new SerialRenderer();

        [Reactive]
        public bool Switch { get; set; } = true;

        public EitherRenderable(IRenderer a, IRenderer b)
        {
            _A = a;
            _B = b;
            this.WhenAnyValue(p => p.Switch)
                .Subscribe
                (v => _Target.Renderer = v ? _A : _B);
        }

        public IRenderer GetRenderer()
        {
            return _Target;
        }
    }

    public static class RendererExtensions
    {
        public static EitherRenderable Hideable(this IRenderer @this)
        {
            return new EitherRenderable(@this, new EmptyRenderer());
        }
        public static Tuple<Vector3,double> BoundingSphere(IReadOnlyList<Vector3> points)
        {
            var range = Range3Single.FromVertices(points);
            return range.BoundingSphere();
        }
    }
}