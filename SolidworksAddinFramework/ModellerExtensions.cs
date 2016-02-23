using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class ModellerExtensions
    {
        public static ICurve CreateTrimmedLine(this IModeler modeler, double[] p0, double[] p1)
        {
            Debug.Assert(p0.Length==p1.Length);
            var p0v = new DenseVector(p0);
            var p1v = new DenseVector(p1);
            var dir = (p1v - p0v).Normalize(2);
            var line = (ICurve)modeler.CreateLine(p0, new[] {dir[0], dir[1] });
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
    }
}
