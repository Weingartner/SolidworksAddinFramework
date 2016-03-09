using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class FaceExtensions
    {
        public static TwoPointRange GetBoxTs(this IFace2 face)
        {
            var box = (double[]) face.GetBox();
            return new TwoPointRange(
                new[] {box[0], box[1], box[2]}, 
                new[] {box[3], box[4], box[5]});
        }

        public static bool GetDistance(this IFace2 entity0, IFace2 entity1, out double[] posacast, out double[] posbcast)
        {
            var bounds = entity1.GetUVBounds().CastArray<double>();

            var param = new[] {bounds[0], bounds[2], bounds[1], bounds[3]};

            object posa, posb;
            double distance;
            var result = ((IEntity) entity0).GetDistance(entity1, true, param, out posa, out posb, out distance);
            posacast = posa.CastArray<double>();
            posbcast = posb.CastArray<double>();
            return result == 0;
        }
    }
}
