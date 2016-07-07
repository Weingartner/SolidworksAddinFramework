using System;
using System.Linq;
using System.Numerics;
using System.Windows.Markup;
using JetBrains.Annotations;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public class BSplineSurface : IEquatable<BSplineSurface>
    {
        public Vector4[,] ControlPointList { get; }

        public int SwOrderU { get; }

        public int SwOrderV { get; }

        public double[] KnotVectorU { get; }

        public double[] KnotVectorV { get; }
        public BSplineSurface([NotNull] Vector4[,] controlPointList, int swOrderU, int swOrderV,
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

        public BSplineSurface WithCtrlPts(Func<Vector4[,], Vector4[,]> converter)
        {
            var mod = converter(ControlPointList);
            return new BSplineSurface(mod, SwOrderU,SwOrderV,KnotVectorU,KnotVectorV);
        }

        #region equality
        public bool Equals(BSplineSurface other)
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
            return Equals((BSplineSurface) obj);
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

        public static bool operator ==(BSplineSurface left, BSplineSurface right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BSplineSurface left, BSplineSurface right)
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
                .SelectMany(v => new double [] {v.X/v.W, v.Y/v.W, v.Z/v.W, v.W})
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

    public static class BSplineSurfaceExt
    {
        public static BSplineSurface GetBSplineSurfaceParams(this ISurface swSurf, double tol)
        {

            var swSurfParameterisation = swSurf.Parameterization2();

            bool sense;
            var surfParams = swSurf.GetBSurfParams3(false, false, swSurfParameterisation, tol, out sense);

            var uKnotVector = surfParams.UKnots.CastArray<double>();
            var vKnotVector = surfParams.VKnots.CastArray<double>();

            // Yeah it is flipped. I know. Don't switch it back. BPH
            var controlPointArray = new Vector4[surfParams.ControlPointColumnCount, surfParams.ControlPointRowCount];
            Enumerable.Range(0, surfParams.ControlPointRowCount)
                .ForEach(u =>
                {
                    Enumerable.Range(0, surfParams.ControlPointColumnCount)
                        .ForEach(v =>
                        {

                            var array = surfParams.GetControlPoints(u+1, v+1).CastArray<double>();

                            var x = array[0];
                            var y = array[1];
                            var z = array[2];
                            var w = 1.0;

                            if (surfParams.ControlPointDimension == 4)
                                w = array[3];

                            var ctrlPoint = new Vector4((float) (x*w),(float) (y*w),(float) (z*w),(float) w);


                            controlPointArray[v, u] = ctrlPoint;
                        });
                });


            return new BSplineSurface
                ( controlPointList: controlPointArray
                    ,swOrderU: surfParams.UOrder
                    ,swOrderV: surfParams.VOrder
                    ,knotVectorU: uKnotVector
                    ,knotVectorV: vKnotVector);


        }
    }
}