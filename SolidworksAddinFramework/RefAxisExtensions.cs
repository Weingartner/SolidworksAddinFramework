﻿using System.DoubleNumerics;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Math;

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
