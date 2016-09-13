using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Runtime.CompilerServices;
using LanguageExt;

namespace SolidworksAddinFramework.Geometry
{
    /// <summary>
    /// A fast 3D range object. Models an axis aligned box between
    /// two points. Because it uses single precision in can using
    /// System.Numerics.Vectors Vector3 class which is very fast.
    /// </summary>
    public struct Range3Single 
    {
        public static bool operator ==(Range3Single left, Range3Single right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Range3Single left, Range3Single right)
        {
            return !left.Equals(right);
        }

        public Vector3 P0 { get; }

        public Vector3 P1 { get; }

        public Vector3 Center => (P0 + P1)/2;

        public Range3Single(double x0, double y0, double z0, double x1, double y1, double z1):this
            (new Vector3(x0,y0,z0), new Vector3(x1,y1,z1) )
        {
        }

        public override string ToString() => $"{XMin}:{XMax}, {YMin}:{YMax}, {ZMin}:{ZMax}";


        public Range3Single(Vector3 p0, Vector3 p1)
        {
            P0 = p0;
            P1 = p1;

            XMin = Math.Min(p0.X, p1.X);
            YMin = Math.Min(p0.Y, p1.Y);
            ZMin = Math.Min(p0.Z, p1.Z);

            XMax = Math.Max(p0.X, p1.X);
            YMax = Math.Max(p0.Y, p1.Y);
            ZMax = Math.Max(p0.Z, p1.Z);

            P0 = new Vector3(XMin,YMin,ZMin);
            P1 = new Vector3(XMax,YMax,ZMax);

        }



        /// <summary>
        /// Passes every vertex to the action. This method avoids
        /// creating an array of vertices on the heap and incurring
        /// the garbage collection and iteration costs.
        /// </summary>
        /// <param name="action"></param>
        public void ProcessVertices(Action<Vector3> action)
        {
            var v = new Vector3(XMin, YMin, ZMin);
            action(v);
            v = new Vector3(XMax, YMin, ZMin);
            action(v);
            v = new Vector3(XMin, YMax, ZMin);
            action(v);
            v = new Vector3(XMax, YMax, ZMin);
            action(v);
            v = new Vector3(XMin, YMin, ZMax);
            action(v);
            v = new Vector3(XMax, YMin, ZMax);
            action(v);
            v = new Vector3(XMin, YMax, ZMax);
            action(v);
            v = new Vector3(XMax, YMax, ZMax);
            action(v);
        }

        public bool Equals(Range3Single other)
        {
            return P0.Equals(other.P0) && P1.Equals(other.P1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Range3Single && Equals((Range3Single) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (P0.GetHashCode()*397) ^ P1.GetHashCode();
            }
        }


        public readonly double XMin;
        public readonly double XMax;
        public double XMid => (XMin + XMax)/2;

        public readonly double YMin;
        public readonly double YMax;
        public double YMid => (YMin + YMax)/2;

        public readonly double ZMin;
        public readonly double ZMax;
        public double ZMid => (ZMin + ZMax)/2; 

        public RangeSingle XRange => new RangeSingle(XMin,XMax); 
        public RangeSingle YRange => new RangeSingle(YMin, YMax);
        public RangeSingle ZRange => new RangeSingle(ZMin, ZMax);

        public double Dx => XMax - XMin;
        public double Dy => YMax - YMin;
        public double Dz => ZMax - ZMin;

        public bool Inside(Vector3 p)
        {
            return Between(p.X, XMin, XMax)&&Between(p.Y,YMin,YMax)&& Between(p.Z, ZMin, ZMax);
        }

        private bool Between(double v, double lower, double upper)
        {
            return v >= lower && v < upper;
        }


        /// <summary>
        /// return the value minimum dimension from Dx, Dy, Dz
        /// </summary>
        public double MinDim => Math.Min(Math.Min(Dx, Dy), Dz);
        public double MaxDim => Math.Max(Math.Max(Dx, Dy), Dz);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Adjust(Vector3 vertex, ref double xmin, ref double xmax, ref double ymin, ref double ymax, ref double zmin,
            ref double zmax)
        {
            xmin = Math.Min(xmin, vertex.X);
            xmax = Math.Max(xmax, vertex.X);

            ymin = Math.Min(ymin, vertex.Y);
            ymax = Math.Max(ymax, vertex.Y);

            zmin = Math.Min(zmin, vertex.Z);
            zmax = Math.Max(zmax, vertex.Z);
        }

        public IEnumerable<Vector3> Verticies
        {
            get
            {
                yield return new Vector3(XMin, YMin, ZMin);
                yield return new Vector3(XMin, YMin, ZMax);
                yield return new Vector3(XMin, YMax, ZMin);
                yield return new Vector3(XMin, YMax, ZMax);
                yield return new Vector3(XMax, YMin, ZMin);
                yield return new Vector3(XMax, YMin, ZMax);
                yield return new Vector3(XMax, YMax, ZMin);
                yield return new Vector3(XMax, YMax, ZMax);
            }
        }

        public Vector3 Size
        {
            get
            {
                var result = P1 - P0;
                return new Vector3(Math.Abs(result.X), Math.Abs(result.Y), Math.Abs(result.Z));
            }
        }

        public static Range3Single FromVertices(IReadOnlyList<Vector3> vertices)
        {
            var xmin = double.MaxValue;
            var ymin = double.MaxValue;
            var zmin = double.MaxValue;

            var xmax = double.MinValue;
            var ymax = double.MinValue;
            var zmax = double.MinValue;

            for (var i = 0; i < vertices.Count; i++)
            {
                // performance optimization. Avoid LINQ
                // ReSharper disable once ForCanBeConvertedToForeach
                Adjust(vertices[i], ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            }

            return new Range3Single(new Vector3(xmin,ymin,zmin), new Vector3(xmax, ymax, zmax));
        }

        public class Range3SingleBuilder
        {
            public double Xmin = double.MaxValue;
            public double Ymin = double.MaxValue;
            public double Zmin = double.MaxValue;

            public double Xmax = double.MinValue;
            public double Ymax = double.MinValue;
            public double Zmax = double.MinValue;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(Vector3 vertex)
            {
                unchecked
                {
                    var x = vertex.X;
                    var y = vertex.Y;
                    var z = vertex.Z;
                    Xmin = Math.Min(Xmin, x);
                    Xmax = Math.Max(Xmax, x);

                    Ymin = Math.Min(Ymin, y);
                    Ymax = Math.Max(Ymax, y);

                    Zmin = Math.Min(Zmin, z);
                    Zmax = Math.Max(Zmax, z);
                }
            }

            public Range3Single Range => new Range3Single(new Vector3(Xmin,Ymin,Zmin), new Vector3(Xmax, Ymax, Zmax));
        }

        public static Range3Single FromTriangle(Triangle triangle)
        {
            var xmin = double.MaxValue;
            var ymin = double.MaxValue;
            var zmin = double.MaxValue;

            var xmax = double.MinValue;
            var ymax = double.MinValue;
            var zmax = double.MinValue;

            Adjust(triangle.A, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            Adjust(triangle.B, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            Adjust(triangle.C, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);

            return new Range3Single(new Vector3(xmin,ymin,zmin), new Vector3(xmax, ymax, zmax));
            
        }
        public static Range3Single FromTriangle(IReadOnlyList<Triangle> triangles)
        {
            var xmin = double.MaxValue;
            var ymin = double.MaxValue;
            var zmin = double.MaxValue;

            var xmax = double.MinValue;
            var ymax = double.MinValue;
            var zmax = double.MinValue;

            for (int j = 0; j < triangles.Count; j++)
            {
                var polygon = triangles[j];
                Adjust(polygon.A, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
                Adjust(polygon.B, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
                Adjust(polygon.C, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            }

            return new Range3Single(new Vector3(xmin,ymin,zmin), new Vector3(xmax, ymax, zmax));
        }

        public Range3Single Scale(double s)
        {
            s = (s - 1)/2;
            var sx = Dx*s;
            var sy = Dy*s;
            var sz = Dz*s;
            return new Range3Single(new Vector3(XMin-sx,YMin-sy,ZMin-sz), new Vector3(XMax+sx, YMax+sy, ZMax+sz));
        }

        public bool Intersects(Range3Single other)
        {
            if (XMax < other.XMin)
                return false;
            if (YMax < other.YMin)
                return false;
            if (ZMax < other.ZMin)
                return false;

            if (XMin > other.XMax)
                return false;
            if (YMin > other.YMax)
                return false;
            if (ZMin > other.ZMax)
                return false;

            return true;
        }
        public Range3Single Intersect(Range3Single other)
        {
            var xrange = XRange.Intersect(other.XRange);
            var yrange = YRange.Intersect(other.YRange);
            var zrange = ZRange.Intersect(other.ZRange);
            return new Range3Single
                (new Vector3(xrange.Min, yrange.Min, zrange.Min),
                 new Vector3(xrange.Max, yrange.Max, zrange.Max));

        }

        public Tuple<Vector3, double> BoundingSphere()
        {
            var d = Math.Sqrt(Dx*Dx + Dy*Dy + Dz*Dz);
            return Prelude.Tuple(Center, (double) d/2);
        }
    }
}