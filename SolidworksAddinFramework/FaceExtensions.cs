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

        private static float[][] GetTessTrianglesTs(this IFace2 face, bool noConversion)
        {
            var data = (float[])face.GetTessTriangles(noConversion);
            return data.Select((value, index) => new { value, index })
                .GroupBy(x => x.index / 3, x => x.value) // a group is a point of the triangle
                .Select(x => x.ToArray())
                .ToArray();
        }

        public static float[][] GetTessTrianglesNoConversion(this IFace2 face) => face.GetTessTrianglesTs(true);
        public static float[][] GetTessTrianglesAllowConversion(this IFace2 face) => face.GetTessTrianglesTs(false);
    }
}
