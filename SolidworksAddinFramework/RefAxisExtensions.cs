using System;
using System.Collections.Generic;
using System.Linq;
using System.DoubleNumerics;
using System.Text;
using System.Threading.Tasks;
using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class RefAxisExtensions
    {
        public static Edge3 Edge(this IRefAxis axis)
        {
            var axisParams = axis.GetRefAxisParams().DirectCast<double[]>();
            return new Edge3(new Vector3((double) axisParams[0], (double) axisParams[1], (double) axisParams[2])
                ,new Vector3((double) axisParams[3], (double) axisParams[4], (double) axisParams[5]));
        }
    }
}
