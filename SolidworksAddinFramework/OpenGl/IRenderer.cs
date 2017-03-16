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

        /// <summary>
        /// Draw the object with an additional transform. This additional
        /// transform could be a reference frame transform
        /// </summary>
        /// <param name="time"></param>
        /// <param name="drawContext"></param>
        /// <param name="parentOpacity"></param>
        /// <param name="renderTransform"></param>
        void Render(DateTime time, IDrawContext drawContext, double parentOpacity = 1.0, Matrix4x4? renderTransform = null);

        /// <summary>
        /// Temporarily transforms the object. Subsequent calls to this method are not cumulative unless
        /// the 'accumulate' parameter is set to true.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="accumulate"></param>
        void ApplyTransform(Matrix4x4 transform, bool accumulate=false);
        Tuple<Vector3, double> BoundingSphere { get; }

        double Opacity { get; set; }

        bool Visibility { get; set; }
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

        public enum Choose
        {
            A,
            B
        };

        private SerialRenderer _Target = new SerialRenderer();

        [Reactive]
        public Choose Switch { get; set; } = Choose.A;

        public EitherRenderable(IRenderer a, IRenderer b)
        {
            _A = a;
            _B = b;
            this.WhenAnyValue(p => p.Switch)
                .Subscribe
                (v => _Target.Renderer = v == Choose.A ? _A : _B);
        }

        public IRenderer GetRenderer()
        {
            return _Target;
        }
    }

    public class HideableRenderable :ReactiveObject, IRenderable
    {
        private EitherRenderable _Inner;

        public HideableRenderable(IRenderer inner)
        {
            _Inner = new EitherRenderable(a: new EmptyRenderer(), b: inner);
            this.WhenAnyValue(p => p.Visibile)
                .Subscribe
                (visible => _Inner.Switch = ToAorB(visible));
        }

        private static EitherRenderable.Choose ToAorB(bool visible) => visible ? EitherRenderable.Choose.B : EitherRenderable.Choose.A;

        [Reactive] public bool Visibile {get; set;}

        public IRenderer GetRenderer()
        {
            return _Inner.GetRenderer();
        }
    }

    public static class RendererExtensions
    {
        public static HideableRenderable Hideable(this IRenderer @this)
        {
            return new HideableRenderable(@this);
        }

        public static Tuple<Vector3,double> BoundingSphere(IReadOnlyList<Vector3> points)
        {
            var range = Range3Single.FromVertices(points);
            return range.BoundingSphere();
        }
    }
}