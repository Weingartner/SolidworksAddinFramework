using System.Collections.Generic;
using System.Linq;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Geometry
{
    public static class FaceExtensions
    {
        public static Range3Single GetBoxTs(this IFace2 face)
        {
            var box = (double[]) face.GetBox();
            return new Range3Single(
                (double) box[0], (double) box[1], (double) box[2], 
                (double) box[3], (double) box[4], (double) box[5]);
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

        private static double[][] GetTessTrianglesTs(this IFace2 face, bool noConversion)
        {
            var data = (double[])face.GetTessTriangles(noConversion);
            return data.Select((value, index) => new { value, index })
                .GroupBy(x => x.index / 3, x => x.value) // a group is a point of the triangle
                .Select(x => x.ToArray())
                .ToArray();
        }

        /// <summary>
        /// Returns all the trim loops of the face as list of a list of curves
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static List<List<ICurve>> GetTrimLoops(this IFace2 face)
        {
            return face
                .GetLoops()
                .CastArray<ILoop2>()
                .OrderBy(l=>l.IsOuter() ? 0 : 1)
                .Select(l=>l.GetEdges().CastArray<IEdge>().Select(e=>(ICurve)e.GetCurve()))
                .Select(curves=>curves.Select(c=>(ICurve)c.Copy()).ToList())
                .ToList();
        }

        public static double[][] GetTessTrianglesNoConversion(this IFace2 face) => face.GetTessTrianglesTs(true);
        public static double[][] GetTessTrianglesAllowConversion(this IFace2 face) => face.GetTessTrianglesTs(false);

        /// <summary>
        /// Packs trimming loops into an array suitable for usage by ISurface::CreateTrimmedSheet4
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static ICurve[] PackForTrimming(this IEnumerable<IEnumerable<ICurve>> enumerable)
        {
            return enumerable
                .SelectMany( l => l .Concat(new ICurve[] {null}))
                .SkipLast(1)
                .ToArray();
        }
    }
}
