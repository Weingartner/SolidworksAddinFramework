using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class FeatureExtensions
    {
        public static double[] GetBoxTs(this IFeature feat)
        {
            object boxObject = null;
            var box = feat.GetBox(ref boxObject);

            if (box)
            {
                return (double[])boxObject;
            }
            throw new Exception("Can't eveluate box!");
        }
    }
}
