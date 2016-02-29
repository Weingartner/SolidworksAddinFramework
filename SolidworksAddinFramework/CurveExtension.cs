using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class CurveExtension
    {
        /// <summary>
        /// Return the point at parameter value t on the curve.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double[] PointAt(this ICurve curve, double t)
        {
            return (double[]) curve.Evaluate2(t, 0);
        }

        /// <summary>
        /// Return the length of the curve between the start
        /// and end parameters.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static double Length(this ICurve curve)
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            return curve.GetLength3(start, end);
        }


        /// <summary>
        /// Return the domain of the curve. ie the [startParam, endParam]
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static double[] Domain(this ICurve	 curve )
        {
            bool isPeriodic;
            double end;
            bool isClosed;
            double start;
            curve.GetEndParams(out start, out end, out isClosed, out isPeriodic);
            return new[] {start, end};

        }
    }
}
