using System.Diagnostics;
using System.Linq;
using System.DoubleNumerics;

using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using LogViewer = Weingartner.WeinCad.Interfaces.LogViewer;

namespace SolidworksAddinFramework.Geometry
{
    public static class SwBSplineParamsExtensions
    {
        public static BSpline3D ToBSpline3D(this ICurve swCurve, bool isClosed)
        {
            return swCurve
                .GetBCurveParams5
                    ( WantCubicIn: false
                    , WantNRational: false
                    , ForceNonPeriodic: false
                    , IsClosed: isClosed
                    )
                .ToBSpline3D(isClosed);
        }

        public static BSpline3D ToBSpline3D(this SplineParamData swSurfParameterisation, bool isClosed)
        {
            object ctrlPts;
            var canGetCtrlPts = swSurfParameterisation.GetControlPoints(out ctrlPts);
            Debug.Assert(canGetCtrlPts);
            var ctrlPtArray = ctrlPts.CastArray<double>();

            object knots;
            var canGetKnots = swSurfParameterisation.GetKnotPoints(out knots);
            Debug.Assert(canGetKnots);
            var knotArray = knots.CastArray<double>();

            var dimension = swSurfParameterisation.Dimension;
            var order = swSurfParameterisation.Order;
            var degree = order - 1;

            var isPeriodic = swSurfParameterisation.Periodic == 1;

            var controlPoints4D = ctrlPtArray
                .Buffer(dimension, dimension)
                .Where(p => p.Count == dimension)
                //http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.isplineparamdata~igetcontrolpoints.html
                .Select
                (p =>
                {
                    var x = 0.0;
                    var y = 0.0;
                    var z = 0.0;
                    var w = 1.0;
                    if (dimension >= 2)
                    {
                        x = p[0];
                        y = p[1];
                    }
                    if (dimension >= 3)
                    {
                        z = p[2];
                    }
                    if (dimension == 4)
                    {
                        w = p[3];
                    }

                    return new Vector4(x*w, y*w, z*w, w);
                })
                .ToArray();

            if(controlPoints4D.Any(c=>c.W!=1.0))
                LogViewer.Log($"Got a rational curve, periodic = {isPeriodic}, isClosed = {isClosed}");

            if (isPeriodic)
                ConvertToNonPeriodic(ref controlPoints4D, ref knotArray, degree);

            return new BSpline3D(controlPoints4D, knotArray, order, isClosed:isClosed, isRational: dimension==4);
        }


        /// <summary>
        /// We want to convert the periodic formulation into a non periodic formulation. To do
        /// this we follow the instructions from Guilia at DevDept.
        ///
        /// the things that need to be done to get the correct curve in Eyeshot are:
        /// - add one last control point equal to the first to close the curve
        /// - multiply the(x, y, z) coordinates of each control point by its w coordinate
        /// - the degree p of your curve is given by the multiplicity of the last knot in the periodic knot vector, therefore you need to increase by 1 the multiplicity of the last knot, and by p the multiplicity of the first knot.
        /// </summary>
        /// <param name="controlPoints4D"></param>
        /// <param name="knotArray"></param>
        /// <param name="degree"></param>
        private static void ConvertToNonPeriodic(ref Vector4[] controlPoints4D, ref double[] knotArray, int degree)
        {

            controlPoints4D = controlPoints4D.EndWith(controlPoints4D.First()).ToArray();

            var last = knotArray.Last();
            var first = knotArray.First();
            knotArray = Enumerable.Repeat(first, degree).Concat(knotArray).EndWith(last).ToArray();

            // We need to do some special handling to make it non periodic
        }
    }
}