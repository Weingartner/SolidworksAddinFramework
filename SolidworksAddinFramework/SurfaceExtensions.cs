using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class SurfaceExtensions
    {
        public static IFace2 ToFace(this ISurface surface)
            => (IFace2) surface.ToSheetBody().GetFirstFace();

        public static IBody2 ToSheetBody(this ISurface surface, bool transferOwnership = false)
            => SwAddinBase.Active.Modeler.CreateSurfaceBody(transferOwnership ? surface : (ISurface) surface.Copy());

        /// <summary>
        /// Represents a point on a surface. 
        /// (X,Y,Z) and (U,V)
        /// </summary>
        public class PointUv
        {
            public double X { get; }

            public double Y { get; }

            public double Z { get; }

            public double U { get; }

            public double V { get; }

            public double[] Point => new[] {X, Y, Z};
            public double[] UV => new[] {U, V};

            public PointUv(double x, double y, double z, double u, double v)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.U = u;
                this.V = v;
            }
        }

        /// <summary>
        /// Gets the closest point on a surface to the input point
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static PointUv GetClosestPointOnTs(this ISurface surface, Vector3 p)
        {
            return ClosestPointOnTs(surface, p.X, p.Y, p.Z);
        }
        public static PointUv GetClosestPointOnTs(this ISurface surface, double [] p)
        {
            return ClosestPointOnTs(surface, p[0], p[1], p[2]);
        }

        /// <summary>
        /// Gets the closest point on a surface to the input point
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private static PointUv ClosestPointOnTs(this ISurface surface, double x, double y, double z)
        {
            var r = (double[]) surface.GetClosestPointOn(x, y, z);
            return new PointUv(r[0], r[1], r[2], r[3], r[4]);
        }

        public static Vector3 PointAt(this ISurface s, double u, double v) => ((double[]) s.Evaluate( u,v,0,0 ))
            .Take(3)
            .ToArray()
            .ToVector3();

        public static Vector3 GetClosestPointOnTs(IFace2 f, Vector3 curvePoint)
        {
            var pt = f.GetClosestPointOn(curvePoint.X, curvePoint.Y, curvePoint.Z)
                .CastArray<double>()
                .ToVector3();
            return pt;
        }

        /// <summary>
        /// Create a surface
        /// </summary>
        /// <param name="controlPointList">Indexed [u,v] </param>
        /// <param name="swOrderU"></param>
        /// <param name="swOrderV"></param>
        /// <param name="knotVectorU"></param>
        /// <param name="knotVectorV"></param>
        /// <returns></returns>
        public static ISurface CreateBSplineSurface
            (Vector4[,] controlPointList, int swOrderU, int swOrderV, double[] knotVectorU, double[] knotVectorV)
        {
            var vOrder = BitConverter.GetBytes(swOrderV);
            var uOrder = BitConverter.GetBytes(swOrderU);

            var swControlPointList = controlPointList
                .EnumerateColumnWise()
                .SelectMany(v => new double [] {v.X, v.Y, v.Z, v.W})
                .ToArray();

            var uLength = controlPointList.GetLength(0);
            var vLength = controlPointList.GetLength(1);

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
                BitConverter.ToDouble(numUCtrPts.Concat(numVCtrPts).ToArray(), 0),
                BitConverter.ToDouble(uPeriodicity.Concat(vPeriodicity).ToArray(), 0),
                BitConverter.ToDouble(dimControlPoints.Concat(unusedParameter).ToArray(), 0)
            };


            return (Surface) SwAddinBase.Active.Modeler
                .CreateBsplineSurface
                (props
                    ,
                    knotVectorV
                    ,
                    knotVectorU
                    ,
                    swControlPointList);
        }
    }

}
