using System;
using System.Collections.Generic;
using System.Numerics;

namespace SolidworksAddinFramework.OpenGl
{
    public abstract class RenderableBase : IRenderable
    {
        protected Tuple<Vector3, float> _BoundingSphere;

        protected Tuple<Vector3, float> UpdateBoundingSphere(IReadOnlyList<Vector3> points)
        {
            return _BoundingSphere = CalcBoundingSphere(points);
        }

        private Tuple<Vector3, float> CalcBoundingSphere(IReadOnlyList<Vector3> points)
        {
            return Renderable.BoundingSphere(points);
        }

        public abstract void Render(DateTime time);
        public abstract void ApplyTransform(Matrix4x4 transform);

        public virtual Tuple<Vector3, float> BoundingSphere()
        {
            return _BoundingSphere;
        }
    }
}