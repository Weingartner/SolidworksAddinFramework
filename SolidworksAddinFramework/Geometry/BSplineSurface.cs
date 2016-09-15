using System;
using System.Diagnostics;
using System.Linq;
using System.DoubleNumerics;
using JetBrains.Annotations;
using SolidworksAddinFramework.Wpf;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Geometry
{
    public class BSplineSurface : IEquatable<BSplineSurface>
    {
        public Vector4[,] ControlPointList { get; }

        public int OrderU { get; }

        public int OrderV { get; }

        public double[] KnotsU { get; }

        public double[] KnotsV { get; }
        public int SurfaceDimension { get; set; }
        public bool UIsPeriodic { get; set; }
        public bool VIsPeriodic { get; set; }

        public BSplineSurface
            ( [NotNull] Vector4[,] controlPointList
            , int orderU
            , int orderV
            , [NotNull] double[] knotsU
            , [NotNull] double[] knotsV
            , int surfaceDimension
            , bool uIsPeriodic
            , bool vIsPeriodic
            )
        {

            if (controlPointList == null) throw new ArgumentNullException(nameof(controlPointList));
            if (knotsU == null) throw new ArgumentNullException(nameof(knotsU));
            if (knotsV == null) throw new ArgumentNullException(nameof(knotsV));

            ControlPointList = controlPointList;
            OrderU = orderU;
            OrderV = orderV;
            KnotsU = knotsU;
            KnotsV = knotsV;
            SurfaceDimension = surfaceDimension;
            UIsPeriodic = uIsPeriodic;
            VIsPeriodic = vIsPeriodic;
        }

        public BSplineSurface WithCtrlPts(Func<Vector4[,], Vector4[,]> converter)
        {
                
            var mod = converter(ControlPointList);
            if(SurfaceDimension==3 && mod.EnumerateColumnWise().Any(v=>v.Z!=1.0) )
                throw new ArgumentException("This should be a non rational surface");

            return new BSplineSurface(mod, OrderU,OrderV,KnotsU,KnotsV, SurfaceDimension, UIsPeriodic, VIsPeriodic);
        }

        #region equality
        public bool Equals(BSplineSurface other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ControlPointList.Cast<Vector4>().SequenceEqual(other.ControlPointList.Cast<Vector4>())
                && KnotsU.SequenceEqual(other.KnotsU) 
                && KnotsV.SequenceEqual(other.KnotsV) 

                && OrderU == other.OrderU 
                && OrderV == other.OrderV;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BSplineSurface) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlPointList.Cast<Vector4>().GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotsU.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotsV.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ OrderU;
                hashCode = (hashCode*397) ^ OrderV;
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
            var vOrder = BitConverter.GetBytes(OrderV);
            var uOrder = BitConverter.GetBytes(OrderU);

            var swControlPointList = ControlPointList
                .EnumerateColumnWise()
                .SelectMany(v => new[] {v.X/v.W, v.Y/v.W, v.Z/v.W, v.W}.Take(SurfaceDimension))
                .ToArray();

            var uLength = ControlPointList.GetLength(0);
            var vLength = ControlPointList.GetLength(1);

            var numUCtrPts = BitConverter.GetBytes(uLength);
            var numVCtrPts = BitConverter.GetBytes(vLength);

            var uPeriodicity = BitConverter.GetBytes(UIsPeriodic ? 1 : 0);
            var vPeriodicity = BitConverter.GetBytes(VIsPeriodic ? 1 : 0);

            var dimControlPoints = BitConverter.GetBytes(SurfaceDimension);
            var unusedParameter = BitConverter.GetBytes(0);

            var props = new[]
            {
                BitConverter.ToDouble(uOrder.Concat(vOrder).ToArray(), 0),
                BitConverter.ToDouble(numVCtrPts.Concat(numUCtrPts).ToArray(), 0),
                BitConverter.ToDouble(uPeriodicity.Concat(vPeriodicity).ToArray(), 0),
                BitConverter.ToDouble(dimControlPoints.Concat(unusedParameter).ToArray(), 0)
            };

            var bsplineSurface = (Surface) SwAddinBase.Active.Modeler
                .CreateBsplineSurface
                    ( props
                    , KnotsU
                    , KnotsV
                    , swControlPointList
                    );

            Debug.Assert(bsplineSurface != null);
            var p = bsplineSurface.Parameterization2();
            Debug.Assert(Math.Abs(p.UMax - KnotsU.Last()) < 1e-9);
            Debug.Assert(Math.Abs(p.UMin - KnotsU.First()) < 1e-9);
            Debug.Assert(Math.Abs(p.VMax - KnotsV.Last()) < 1e-9);
            Debug.Assert(Math.Abs(p.VMin - KnotsV.First()) < 1e-9);

            return bsplineSurface;
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

                            var ctrlPoint = new Vector4(x*w,y*w,z*w,w);


                            controlPointArray[v, u] = ctrlPoint;
                        });
                });


            return new BSplineSurface
                ( controlPointList: controlPointArray
                    ,orderU: surfParams.UOrder
                    ,orderV: surfParams.VOrder
                    ,knotsU: uKnotVector
                    ,knotsV: vKnotVector
                    ,surfaceDimension: surfParams.ControlPointDimension
                    ,uIsPeriodic:surfParams.UPeriodicity
                    ,vIsPeriodic:surfParams.VPeriodicity
                    );
        }
    }
}