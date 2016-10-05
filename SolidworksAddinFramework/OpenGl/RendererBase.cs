using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Drawing;
using System.Reactive.Subjects;
using LanguageExt;

namespace SolidworksAddinFramework.OpenGl
{
    public abstract class RendererBase<T> :  IRenderer
    {
        protected readonly T _Data;
        protected T _TransformedData;

        private Transformable _Transform = new Transformable();

        private Lazy<Tuple<Vector3, double>> _BoundingSphere;

        protected RendererBase(T data)
        {
            _Data = data;
            _TransformedData = data;
            _BoundingSphere = new Lazy<Tuple<Vector3, double>>(()=> UpdateBoundingSphere(_TransformedData, DateTime.Now));
        }

        private Tuple<Vector3, double> CalcBoundingSphere(IReadOnlyList<Vector3> points)
        {
            return RendererExtensions.BoundingSphere(points);
        }

        private readonly Subject<Unit> _NeedsRedraw = new Subject<Unit>();
        public IObservable<Unit> NeedsRedraw => _NeedsRedraw;

        public void FireRedraw() => _NeedsRedraw.OnNext(Unit.Default);

        public void Render(DateTime time, double parentOpacity = 1.0, Matrix4x4? renderTransform = null)
        {
            var transform = _Transform.Transform*(renderTransform ?? Matrix4x4.Identity);
            _TransformedData = DoTransform(_Data, transform);
            DoRender(_TransformedData, time, Opacity, Visibility );
            _BoundingSphere = new Lazy<Tuple<Vector3, double>>(()=> UpdateBoundingSphere(_TransformedData, time));
        }

        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {
            _Transform.ApplyTransform(transform, accumulate);
        }

        protected abstract T DoTransform(T data, Matrix4x4 transform); 
        protected abstract void DoRender(T data, DateTime time, double opacity, bool visibile);
        protected abstract Tuple<Vector3, double> UpdateBoundingSphere(T data, DateTime time);


        public Tuple<Vector3, double> BoundingSphere => _BoundingSphere.Value;

        public double Opacity { get; set; } = 1.0;
        public bool Visibility { get; set; } = true;

        protected Color FromArgb(double opacity, Color baseColor)
        {
            return Color.FromArgb((int)(opacity*255), baseColor);
        }
    }
}