using System;
using System.Collections.Generic;
using System.DoubleNumerics;

namespace SolidworksAddinFramework.OpenGl
{
    public abstract class RenderableBase<T> : IRenderable
    {
        protected readonly T _Data;
        protected T _TransformedData;

        private Matrix4x4 _Transform = Matrix4x4.Identity;
        private Matrix4x4 _BaseTransform = Matrix4x4.Identity;

        protected Lazy<Tuple<Vector3, double>> _BoundingSphere;

        protected RenderableBase(T data)
        {
            _Data = data;
            _TransformedData = data;
        }


        protected void UpdateBoundingSphere(IReadOnlyList<Vector3> points)
        {
            _BoundingSphere = new Lazy<Tuple<Vector3, double>>(()=> CalcBoundingSphere(points));
        }
        protected void UpdateBoundingSphere(Func<Tuple<Vector3, double>> sphere)
        {
            _BoundingSphere = new Lazy<Tuple<Vector3, double>>(sphere);
        }

        private Tuple<Vector3, double> CalcBoundingSphere(IReadOnlyList<Vector3> points)
        {
            return Renderable.BoundingSphere(points);
        }

        public void Render(DateTime time)
        {
            DoRender(_TransformedData, time);
            _BoundingSphere = new Lazy<Tuple<Vector3, double>>(()=> UpdateBoundingSphere(_TransformedData, time));
        }


        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {

            if (accumulate == false)
            {
                _Transform = transform;
            }
            else
            {
                _BaseTransform = _BaseTransform*transform;
                _Transform = Matrix4x4.Identity;
            }

            transform = this._BaseTransform*_Transform;

            _TransformedData = DoTransform(_Data, transform);

        }

        protected abstract T DoTransform(T data, Matrix4x4 transform); 
        protected abstract void DoRender(T data, DateTime time);
        protected abstract Tuple<Vector3, double> UpdateBoundingSphere(T data, DateTime time);


        public Tuple<Vector3, double> BoundingSphere => _BoundingSphere.Value;
    }
}