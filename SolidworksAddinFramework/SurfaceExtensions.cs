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

        public static SwBSplineSurfaceParams GetBSplineSurfaceParams(this ISurface swSurf, double tol)
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
                            var ctrlPoint = surfParams.ControlPointDimension == 3 ? 
                                new Vector4((float) array[0], (float) array[1], (float) array[2],1) :
                                new Vector4((float) array[0], (float) array[1], (float) array[2], (float) array[3]);
                            controlPointArray[v, u] = ctrlPoint;
                        });
                });


            return new SwBSplineSurfaceParams
                ( controlPointList: controlPointArray
                ,swOrderU: surfParams.UOrder
                ,swOrderV: surfParams.VOrder
                ,knotVectorU: uKnotVector
                ,knotVectorV: vKnotVector);


        }
    }
}
