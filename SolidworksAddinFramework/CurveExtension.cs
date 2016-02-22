using System;
using System.Collections.Generic;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class CurveExtension
    {
        public static double[] PointAt(this ICurve curve, double t)
        {
            return (double[]) curve.Evaluate2(t, 0);
        }

        public static double[] Domain(this ICurve curve )
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
