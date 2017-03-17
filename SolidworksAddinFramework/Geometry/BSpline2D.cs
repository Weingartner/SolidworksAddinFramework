using System;
using System.Diagnostics;
using System.Linq;
using System.DoubleNumerics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.Geometry
{
    /// <summary>
    /// Control points are (X,Y,W) the W param is mapped to the Z parameter of the Vector3
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BSpline2D : BSpline<Vector3>
    {
        public BSpline2D([NotNull] Vector3[] controlPoints, [NotNull] double[] knotVectorU, int order, bool isClosed, bool isRational) : base(controlPoints, knotVectorU, order, isClosed, isRational)
        {
            if(!isRational) //  Non rational should have all W ( Z ) be 1.0
                Debug.Assert(controlPoints.All(c=>Math.Abs(c.Z - 1) < 1e-9));
        }

        /// <summary>
        /// Creates a PCurve, a 2D curve parameterized in UV over
        /// the surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public ICurve ToPCurve(ISurface surface)
        {
            var propsDouble = PropsDouble;
            var knots = KnotVectorU;
            var ctrlPtCoords = ControlPoints.SelectMany(p => p.ToDoubles().Take(Dimension)).ToArray();

            #region debug
            var param = surface.Parameterization2();
            Debug.Assert(ControlPoints.All(c=>
                c.X <= param.UMax && c.X >= param.UMin 
                && c.Y <= param.VMax && c.Y >= param.VMin
                ));
            #endregion

            Debug.Assert(knots.Length == ControlPoints.Length + Order);

            var pCurve = (ICurve) SwAddinBase.Active.Modeler.CreatePCurve(surface, propsDouble, knots, ctrlPtCoords);
            Debug.Assert(pCurve!=null);
            return pCurve;
        }


        public override int Dimension => IsRational ? 3 : 2;
    }
}