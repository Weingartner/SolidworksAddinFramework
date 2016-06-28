using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using SolidworksAddinFramework.OpenGl;

namespace SolidworksAddinFramework.Geometry
{
    public struct PointDirection3
    {
        public readonly Vector3 Point;
        public readonly Vector3 Direction;

        /// <summary>
        /// Apply a tranformation to the poin and a rotation to the Direction. The
        /// rotation should be consistent with the matrix. 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointDirection3 ApplyTransform(Matrix4x4 matrix)
        {
            return new PointDirection3
                ( Vector3.Transform(Point, matrix)
                    , Vector3.TransformNormal(Direction, matrix));
        }

        public PointDirection3(Vector3 point, Vector3 direction)
        {
            Point = point;
            Direction = direction;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator PointDirection3(Vector3 point)
        {
            return new PointDirection3(point, default(Vector3));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector3(PointDirection3 point)
        {
            return point.Point;
        }
        public void GLVertex3AndNormal3()
        {
            if(!Direction.Equals(default(Vector3)))
                Direction.GLNormal3();
            Point.GLVertex3();
            
        }

        /// <summary>
        /// Project a point onto this plane
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Vector3 ProjectOnPlane(Vector3 q)
        {
            var p = this.Point;
            return q - (q - p).Dot(Direction)*Direction;
        }

        public Vector3 ProjectOnEdge(Vector3 point)
        {
            var d = Direction;
            var t = Vector3.Dot(point - Point, d)/d.LengthSquared();
            return d*t + Point;
        }

        public static PointDirection3 UnitZ = new PointDirection3(Vector3.Zero, Vector3.UnitZ);
        public static PointDirection3 UnitY = new PointDirection3(Vector3.Zero, Vector3.UnitY);
        public static PointDirection3 UnitX = new PointDirection3(Vector3.Zero, Vector3.UnitX);


    }

    public static class PointDirection3Extensions
    {
        public static PointDirection3 ToPointDirection3(this IEnumerable<Vector3> source)
        {
            var s = source.ToList();
            if(s.Count!=2)
                throw new ArgumentException("should be of length 2", nameof(source));
            return new PointDirection3(s[0],s[1]);
        }
        
    }
}