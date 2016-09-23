using System;
using System.Linq;
using FsCheck;
using SolidworksAddinFramework.Geometry;
using WeinCadSW.Spec.FsCheck;
using static LanguageExt.Prelude;

namespace SolidworksAddinFramework.Spec.Geometry
{
    public class TriangeIntersectPlaneSpec
    {
        public static Gen<Triangle> Triangle()
        {
            return GenFloat
                .Normal
                .Where(v => v > 0.1 && v < 10)
                .Vector3()
                .Triangle()
                .Where
                (v =>
                {
                    var e0 = new Edge3(v.A, v.B);
                    var e1 = new Edge3(v.A, v.C);
                    var e2 = new Edge3(v.B, v.C);

                    var minDist = 1e-2;
                    var maxDist = 1;

                    if (List(e0, e1, e2).Any(e => e.Length < minDist))
                        return false;

                    if (List(e0, e1, e2).XPaired().Any(t => t.Map((a, b) => a.AngleBetween(b) < 5d / 180 * Math.PI)))
                        return false;

                    return true;
                });
        }


    }
}