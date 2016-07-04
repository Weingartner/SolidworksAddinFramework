using System;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public class SwBSplineSurfaceParams : IEquatable<SwBSplineSurfaceParams>
    {
        public Vector4[,] ControlPointList { get; }

        public int SwOrderU { get; }

        public int SwOrderV { get; }

        public double[] KnotVectorU { get; }

        public double[] KnotVectorV { get; }
        public SwBSplineSurfaceParams([NotNull] Vector4[,] controlPointList, int swOrderU, int swOrderV,
            [NotNull] double[] knotVectorU, [NotNull] double[] knotVectorV)
        {

            if (controlPointList == null) throw new ArgumentNullException(nameof(controlPointList));
            if (knotVectorU == null) throw new ArgumentNullException(nameof(knotVectorU));
            if (knotVectorV == null) throw new ArgumentNullException(nameof(knotVectorV));

            ControlPointList = controlPointList;
            SwOrderU = swOrderU;
            SwOrderV = swOrderV;
            KnotVectorU = knotVectorU;
            KnotVectorV = knotVectorV;
        }

        public SwBSplineSurfaceParams WithCtrlPts(Func<Vector4[,], Vector4[,]> converter)
        {
            var mod = converter(ControlPointList);
            return new SwBSplineSurfaceParams(mod, SwOrderU,SwOrderV,KnotVectorU,KnotVectorV);
        }

        #region equality
        public bool Equals(SwBSplineSurfaceParams other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ControlPointList.Cast<Vector4>().SequenceEqual(other.ControlPointList.Cast<Vector4>())
                && KnotVectorU.SequenceEqual(other.KnotVectorU) 
                && KnotVectorV.SequenceEqual(other.KnotVectorV) 

                && SwOrderU == other.SwOrderU 
                && SwOrderV == other.SwOrderV;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SwBSplineSurfaceParams) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlPointList.Cast<Vector4>().GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotVectorU.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotVectorV.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ SwOrderU;
                hashCode = (hashCode*397) ^ SwOrderV;
                return hashCode;
            }
        }

        public static bool operator ==(SwBSplineSurfaceParams left, SwBSplineSurfaceParams right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SwBSplineSurfaceParams left, SwBSplineSurfaceParams right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// Create a surface
        /// </summary>
        /// <param name="swBSplineSurfaceParams"></param>
        /// <returns></returns>
        public ISurface ToSurface ()
        {
            var vOrder = BitConverter.GetBytes(SwOrderV);
            var uOrder = BitConverter.GetBytes(SwOrderU);

            var swControlPointList = ControlPointList
                .EnumerateColumnWise()
                .SelectMany(v => new double [] {v.X, v.Y, v.Z, v.W})
                .ToArray();

            var uLength = ControlPointList.GetLength(0);
            var vLength = ControlPointList.GetLength(1);

            var numUCtrPts = BitConverter.GetBytes(uLength);
            var numVCtrPts = BitConverter.GetBytes(vLength);
            //TODO: find out what periodicity means in this context 
            var uPeriodicity = BitConverter.GetBytes(0);
            var vPeriodicity = BitConverter.GetBytes(0);
            var dimControlPoints = BitConverter.GetBytes(4);
            var unusedParameter = BitConverter.GetBytes(0);

            var props = new[]
            {
                BitConverter.ToDouble(uOrder.Concat(vOrder).ToArray(), 0),
                BitConverter.ToDouble(numVCtrPts.Concat(numUCtrPts).ToArray(), 0),
                BitConverter.ToDouble(uPeriodicity.Concat(vPeriodicity).ToArray(), 0),
                BitConverter.ToDouble(dimControlPoints.Concat(unusedParameter).ToArray(), 0)
            };


            return (Surface) SwAddinBase.Active.Modeler
                .CreateBsplineSurface
                ( props
                    , KnotVectorU
                    , KnotVectorV
                    , swControlPointList);
        }
    }
}