using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            return new Edge3(new Vector3((float) axisParams[0], (float) axisParams[1], (float) axisParams[2])
                ,new Vector3((float) axisParams[3], (float) axisParams[4], (float) axisParams[5]));
        }
    }
}
