using System;
using System.Collections.Generic;
using System.DoubleNumerics;

namespace SolidworksAddinFramework.OpenGl
{
    public abstract class RenderableBase : IRenderable
    {
        protected Lazy<Tuple<Vector3, double>> _BoundingSphere;

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

        public abstract void Render(DateTime time);
        public abstract void ApplyTransform(Matrix4x4 transform);

        public virtual Tuple<Vector3, double> BoundingSphere => _BoundingSphere.Value;
    }
}