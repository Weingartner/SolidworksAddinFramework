using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class RefPlaneExtensions
    {
        public static double[] Normal(this IRefPlane plane, IMathUtility math)
        {
            var transform = plane.Transform;
            var normalWorld = math.Vector(new[] {0.0, 0, 1});
            return normalWorld.MultiplyTransformTs(transform).ArrayData.CastArray<double>();
        }
        public static double[] Origin(this IRefPlane plane, IMathUtility math)
        {
            var transform = plane.Transform;
            var originWorld = math.Point(new[] {0.0, 0, 0});
            return originWorld.MultiplyTransformTs(transform).ArrayData.CastArray<double>();
        }

        public static bool PlaneOriginEquals(this IRefPlane p, IMathUtility mathUtility, double[] origin, double tol)
        {
            return MathPointExtensions.Equals(mathUtility.Point(p.Origin(mathUtility)), mathUtility.Point(origin), tol);
        }

        public static bool PlaneNormalEquals(this IRefPlane p, IMathUtility mathUtility, double[] normal, double tol)
        {
            return MathPointExtensions.Equals(mathUtility.Vector(p.Normal(mathUtility)), mathUtility.Vector(normal), tol);
        }

        public static bool PlaneOriginAndNormalEquals(this IRefPlane p, 
            IMathUtility math ,
            double[] origin,
            double[] normal,
            double tol)
        {
            return PlaneOriginEquals(p, math, origin, tol) &&
                PlaneNormalEquals(p, math, normal, tol);
            
        }
    }
}
