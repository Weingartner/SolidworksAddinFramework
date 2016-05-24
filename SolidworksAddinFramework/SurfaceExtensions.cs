using System;
using System.Collections.Generic;
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

        public static IBody2 ToSheetBody(this ISurface surface) => SwAddinBase.Active.Modeler.CreateSurfaceBody(surface);

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

        public static Vector3 GetClosestPointOnTs(IFace2 f, Vector3 curvePoint)
        {
            var pt = f.GetClosestPointOn(curvePoint.X, curvePoint.Y, curvePoint.Z)
                .CastArray<double>()
                .ToVector3();
            return pt;
        }
    }

}
