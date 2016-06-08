using System;
using System.Collections.Generic;
using System.Numerics;
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
        public Vector3 P0 { get; }

        public Vector3 P1 { get; }

        public Vector3 Center => (P0 + P1)/2;

        public Range3Single(float x0, float y0, float z0, float x1, float y1, float z1):this
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


        public readonly float XMin;
        public readonly float XMax;
        public float XMid => (XMin + XMax)/2;

        public readonly float YMin;
        public readonly float YMax;
        public float YMid => (YMin + YMax)/2;

        public readonly float ZMin;
        public readonly float ZMax;
        public float ZMid => (ZMin + ZMax)/2; 

        public RangeSingle XRange => new RangeSingle(XMin,XMax); 
        public RangeSingle YRange => new RangeSingle(YMin, YMax);
        public RangeSingle ZRange => new RangeSingle(ZMin, ZMax);

        public float Dx => XMax - XMin;
        public float Dy => YMax - YMin;
        public float Dz => ZMax - ZMin;

        public bool Inside(Vector3 p)
        {
            return Between(p.X, XMin, XMax)&&Between(p.Y,YMin,YMax)&& Between(p.Z, ZMin, ZMax);
        }

        private bool Between(float v, float lower, float upper)
        {
            return v >= lower && v < upper;
        }


        /// <summary>
        /// return the value minimum dimension from Dx, Dy, Dz
        /// </summary>
        public float MinDim => Math.Min(Math.Min(Dx, Dy), Dz);
        public float MaxDim => Math.Max(Math.Max(Dx, Dy), Dz);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Adjust(Vector3 vertex, ref float xmin, ref float xmax, ref float ymin, ref float ymax, ref float zmin,
            ref float zmax)
        {
            xmin = Math.Min(xmin, vertex.X);
            xmax = Math.Max(xmax, vertex.X);

            ymin = Math.Min(ymin, vertex.Y);
            ymax = Math.Max(ymax, vertex.Y);

            zmin = Math.Min(zmin, vertex.Z);
            zmax = Math.Max(zmax, vertex.Z);
        }

        public static Range3Single FromVertices(IReadOnlyList<Vector3> vertices)
        {
            var xmin = Single.MaxValue;
            var ymin = Single.MaxValue;
            var zmin = Single.MaxValue;

            var xmax = Single.MinValue;
            var ymax = Single.MinValue;
            var zmax = Single.MinValue;

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
            public float Xmin = Single.MaxValue;
            public float Ymin = Single.MaxValue;
            public float Zmin = Single.MaxValue;

            public float Xmax = Single.MinValue;
            public float Ymax = Single.MinValue;
            public float Zmax = Single.MinValue;

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
            var xmin = Single.MaxValue;
            var ymin = Single.MaxValue;
            var zmin = Single.MaxValue;

            var xmax = Single.MinValue;
            var ymax = Single.MinValue;
            var zmax = Single.MinValue;

            Adjust(triangle.A, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            Adjust(triangle.B, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            Adjust(triangle.C, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);

            return new Range3Single(new Vector3(xmin,ymin,zmin), new Vector3(xmax, ymax, zmax));
            
        }
        public static Range3Single FromTriangle(IReadOnlyList<Triangle> triangles)
        {
            var xmin = Single.MaxValue;
            var ymin = Single.MaxValue;
            var zmin = Single.MaxValue;

            var xmax = Single.MinValue;
            var ymax = Single.MinValue;
            var zmax = Single.MinValue;

            for (int j = 0; j < triangles.Count; j++)
            {
                var polygon = triangles[j];
                Adjust(polygon.A, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
                Adjust(polygon.B, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
                Adjust(polygon.C, ref xmin, ref xmax, ref ymin, ref ymax, ref zmin, ref zmax);
            }

            return new Range3Single(new Vector3(xmin,ymin,zmin), new Vector3(xmax, ymax, zmax));
        }

        public Range3Single Scale(float s)
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

        public Tuple<Vector3, float> BoundingSphere()
        {
            var d = Math.Sqrt(Dx*Dx + Dy*Dy + Dz*Dz);
            return Prelude.Tuple(Center, (float) d/2);
        }
    }
}