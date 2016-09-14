using System;
using System.Linq;
using System.DoubleNumerics;
using JetBrains.Annotations;

namespace SolidworksAddinFramework.Geometry
{
    public abstract class BSpline<T> : IEquatable<BSpline<T>>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Control points are stored as (X,Y,Z) being the
        /// true location of the control point and (W) being
        /// the weight. Some frameworks store (X*W,Y*W,Z*W).
        /// This is not done here. We use the unscaled location
        /// variables. You may have to scale up and down when
        /// coverting to other systems such as eyeshot and 
        /// solidworks.
        /// 
        /// Eyeshot always stores as (X*W,Y*W,Z*W,W) in
        /// it's Point4D class. Solidworks varies depending
        /// on if the spline is periodic, closed and if 
        /// it's a tuesday or not. Grrrr. So be careful.
        /// </summary>
        public T[] ControlPoints { get; }

        public bool IsClosed { get; }

        /// <summary>
        /// The dimension of the control point
        /// </summary>
        public abstract int Dimension { get; }

        public bool IsRational { get; }

        public int Order { get; }

        public double[] KnotVectorU { get; }

        public BSpline([NotNull] T[] controlPoints, [NotNull] double[] knotVectorU, int order, bool isClosed, bool isRational)
        {

            if (controlPoints == null) throw new ArgumentNullException(nameof(controlPoints));
            if (knotVectorU == null) throw new ArgumentNullException(nameof(knotVectorU));

            ControlPoints = controlPoints;
            Order = order;
            IsClosed = isClosed;
            IsRational = isRational;
            KnotVectorU = knotVectorU;
        }


        #region equality
        public bool Equals(BSpline<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ControlPoints.SequenceEqual(other.ControlPoints)
                   && KnotVectorU.SequenceEqual(other.KnotVectorU)

                   && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BSpline<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlPoints.Cast<Vector3>().GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotVectorU.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ Order;
                return hashCode;
            }
        }

        public static bool operator ==(BSpline<T> left, BSpline<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BSpline<T> left, BSpline<T> right)
        {
            return !Equals(left, right);
        }
        #endregion
        protected double[] PropsDouble
        {
            get
            {
                int order = (short) Order;
                int numCtrlPoints = (short) ControlPoints.Length;

                // Here periodicity means "Is the curve solidworks periodic". Our curves are
                // never solidworks periodic though they may be closed. Sometimes in solidworks doc you
                // find the work periodic but it means closed. Better differentation is made when they
                // say "clamped periodic" vs "solidworks periodic"

                int periodicity = 0;
                var propsDouble = new[] {Dimension, order, numCtrlPoints, periodicity}.ToDouble();
                return propsDouble;
            }
        }

    }
}