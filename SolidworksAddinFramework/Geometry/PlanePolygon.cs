using System.Collections.Generic;
using System.Linq;
using System.DoubleNumerics;

namespace SolidworksAddinFramework.Geometry
{
    public static class PlanePolygon
    {
        /// <summary>
        /// Assumes that polygon is closed, ie first and last points are the same
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static bool OrientationClosed(this IEnumerable<Vector3> polygon, Vector3 up)
        {
            var sum = Vector3.Zero;
            foreach (var b in polygon.Buffer(2, 1))
            {
                if (b.Count == 2)
                    sum = sum + Vector3.Cross(b[0], b[1])/b[0].Length()/b[1].Length();
            }

            return Vector3.Dot(up, sum) > 0;

        } 
        
    }
}