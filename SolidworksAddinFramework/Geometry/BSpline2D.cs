using System.Diagnostics;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Geometry
{
    /// <summary>
    /// Control points are (X,Y,W) the W param is mapped to the Z parameter of the Vector3
    /// </summary>
    public class BSpline2D : BSpline<Vector3>
    {
        public BSpline2D([NotNull] Vector3[] controlPoints, [NotNull] double[] knotVectorU, int order, bool isPeriodic) : base(controlPoints, knotVectorU, order, isPeriodic)
        {
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
            var ctrlPtCoords = ControlPoints.SelectMany(p => Vector3Extensions.ToDoubles((Vector3) p)).ToArray();
            var param = surface.Parameterization2();
            Debug.Assert(ControlPoints.All(c=>
                c.X < param.UMax && c.X > param.UMin 
                && c.Y < param.VMax && c.Y > param.VMin
                ));
            var pCurve = (ICurve) SwAddinBase.Active.Modeler.CreatePCurve(surface, propsDouble, knots, ctrlPtCoords);
            Debug.Assert(pCurve!=null);
            return pCurve;
        }


        public double[] ToDoubles(Vector3 t)
        {
            return t.ToDoubles();
        }
    }
}