using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Reactive.Subjects;
using LanguageExt;

namespace SolidworksAddinFramework.OpenGl
{
    public class Transformable
    {
        private Matrix4x4 _AdditionalTransform = Matrix4x4.Identity;
        private Matrix4x4 _BaseTransform = Matrix4x4.Identity;

        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {

            if (accumulate == false)
            {
                _AdditionalTransform = transform;
            }
            else
            {
                _BaseTransform = _BaseTransform*transform;
                _AdditionalTransform = Matrix4x4.Identity;
            }
        }

        public Matrix4x4 Transform => this._BaseTransform*_AdditionalTransform;
    }

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

        public void Render(DateTime time, Matrix4x4? renderTransform = null)
        {
            var transform = _Transform.Transform*(renderTransform ?? Matrix4x4.Identity);
            _TransformedData = DoTransform(_Data, transform);
            DoRender(_TransformedData, time);
            _BoundingSphere = new Lazy<Tuple<Vector3, double>>(()=> UpdateBoundingSphere(_TransformedData, time));
        }

        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {
            _Transform.ApplyTransform(transform, accumulate);
        }

        protected abstract T DoTransform(T data, Matrix4x4 transform); 
        protected abstract void DoRender(T data, DateTime time);
        protected abstract Tuple<Vector3, double> UpdateBoundingSphere(T data, DateTime time);


        public Tuple<Vector3, double> BoundingSphere => _BoundingSphere.Value;
    }
}