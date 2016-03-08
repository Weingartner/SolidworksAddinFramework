using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class ModellerExtensions
    {
        /// <summary>
        /// Create a line from p0 to p1. 
        /// </summary>
        /// <param name="modeler"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static ICurve CreateTrimmedLine(this IModeler modeler, double[] p0, double[] p1)
        {
            Debug.Assert(p0.Length==p1.Length);
            var p0v = new DenseVector(p0);
            var p1v = new DenseVector(p1);
            var dir = (p1v - p0v).Normalize(2);
            var line = (ICurve)modeler.CreateLine(p0, new[] {dir[0], dir[1], dir[2] });
            Debug.Assert(line != null, "line != null");
            if(p1.Length==3)
            {
                line = line.CreateTrimmedCurve2(p0[0], p0[1], p0[2], p1[0], p1[1], p1[2]);
            }else
            if (p1.Length==2)
            {
                line = line.CreateTrimmedCurve2(p0[0], p0[1], 0, p1[0], p1[1], 0);
            }else
                throw new Exception("End points must have 2 or 3 elements");

            Debug.Assert(line != null, "line != null");
            return line;
        }

        public static ICurve CreateTrimmedLine(this IModeler modeler, MathPoint p0, MathPoint p1)
        {
            return CreateTrimmedLine(modeler, (double[])p0.ArrayData, (double[])p1.ArrayData);
        }


        #region CreateSheetFromSurface
        public static IBody2 CreateSheetFromSurface(this IModeler modeler, ISurface surf , SurfaceExtensions.PointUv uvLow, SurfaceExtensions.PointUv uvHigh)
        {
            var uRange = new double[] {uvLow.U, uvHigh.U}.OrderBy(v=>v).ToArray();
            var vRange = new double[] {uvLow.V, uvHigh.V}.OrderBy(v=>v).ToArray();
            return CreateSheetFromSurface(modeler, surf, uRange, vRange);
        }

        private static IBody2 CreateSheetFromSurface(IModeler modeler, ISurface surf, double[] uRange, double[] vRange)
        {
            var uvRange = uRange.Concat(vRange).ToArray();
            var sheet = (IBody2) modeler.CreateSheetFromSurface(surf, uvRange);
            return sheet;
        }

        #endregion

        /// <summary>
        /// Create a bounded sheet.
        /// </summary>
        /// <param name="modeler"></param>
        /// <param name="center">Center of the surface</param>
        /// <param name="vNormal">Normal of the surface</param>
        /// <param name="p0">Point to project onto surface to find UV bounds</param>
        /// <param name="p1">Point to project onto surface to find UV bounds</param>
        /// <returns></returns>
        public static IBody2 CreateSheet(this IModeler modeler, double[] center, double[] vNormal, double[] p0, double[] p1)
        {
            var surf = (Surface) modeler.CreatePlanarSurface(center, vNormal);
            return CreateSheetFromSurface(modeler, surf, p0, p1);
        }

        public static IBody2 CreateSheetFromSurface(this IModeler modeler, ISurface surf, IMathPoint p0, IMathPoint p1)
        {
            var uvLow = surf.GetClosestPointOnTs((double[])p0.ArrayData);
            var uvHigh = surf.GetClosestPointOnTs((double[])p1.ArrayData);
            return modeler.CreateSheetFromSurface(surf, uvLow, uvHigh);
        }
    }
}
