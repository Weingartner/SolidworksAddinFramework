using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Geometry
{
    public class BSpline3D : BSpline<Vector4>
    {
        public BSpline3D([NotNull] Vector4[] controlPoints, [NotNull] double[] knotVectorU, int order, bool isPeriodic, bool isRational) : base(controlPoints, knotVectorU, order, isPeriodic, isRational)
        {
            if(!IsRational)
            {
                Debug.Assert(controlPoints.All(c => Math.Abs(c.W - 1.0) < 1e-9));
            }
        }
        public ICurve ToCurve()
        {
            var propsDouble = PropsDouble;
            var knots = KnotVectorU;
            var ctrlPtCoords = ControlPoints.SelectMany(p => Vector3Extensions.ToDoubles((Vector4) p)).ToArray();
            return (ICurve) SwAddinBase.Active.Modeler.CreateBsplineCurve( propsDouble, knots, ctrlPtCoords);
        }

        public double[] ToDoubles(Vector4 t)
        {
            return t.ToDoubles();
        }

        public override int Dimension => IsRational ? 4 : 3;
    }
}