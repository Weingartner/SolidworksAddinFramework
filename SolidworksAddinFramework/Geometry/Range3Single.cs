using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using LanguageExt;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

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

        [Pure]
        public IBody2 ToSolid()
        {
            var modeler = SwAddinBase.Active.Modeler;
            var center = new Vector3(XMid, YMid, ZMin);
            var box = modeler.CreateBox(center, Vector3.UnitZ, Dx, Dy, Dz);
            Debug.Assert(box!=null);
            return box;
        }

        [Pure]
        public Mesh ToMesh(Color color)
        {
            return new Mesh(color, false, Triangles(), Edges());
        }

        public static Mesh ToMesh(IEnumerable<Range3Single> voxels, Color color)
        {
            return new Mesh(color, false, voxels.SelectMany(p=>p.TrianglesWithNormals()));
        }


        public List<Edge3> Edges()
        {
            var list = new List<Edge3>(12);

            Func<int, int, int, Vector3> _ = GetAt;

            // Bottom square
            list.Add(new Edge3( _(0,0,0), _(1,0,0) ));
            list.Add(new Edge3( _(1,0,0), _(1,0,1) ));
            list.Add(new Edge3(_(1,0,1), _(0,0,1) ));
            list.Add(new Edge3(_(0,0,1), _(0,0,0) ));

            // Top square
            list.Add(new Edge3(_(0,1,0), _(1,1,0) ));
            list.Add(new Edge3(_(1,1,0), _(1,1,1) ));
            list.Add(new Edge3(_(1,1,1), _(0,1,1) ));
            list.Add(new Edge3(_(0,1,1), _(0,1,0) ));

            // Connecting top and bottom lines
            list.Add(new Edge3(_(0,0,0), _(0,1,0) ));
            list.Add(new Edge3(_(1,0,0), _(1,1,0) ));
            list.Add(new Edge3(_(1,0,1), _(1,1,1) ));
            list.Add(new Edge3(_(0,0,1), _(0,1,1) ));

            return list;

        }

        [Pure]
        public Triangle[] Triangles()
        {
            var v000 = GetAt(0, 0, 1);
            var v001 = GetAt(0, 0, 0);
            var v010 = GetAt(0, 1, 1);
            var v011 = GetAt(0, 1, 0);
            var v100 = GetAt(1, 0, 1);
            var v101 = GetAt(1, 0, 0);
            var v110 = GetAt(1, 1, 1);
            var v111 = GetAt(1, 1, 0);

            var list = new[]
            {
            // front
                new Triangle(v010, v100, v000),
                new Triangle(v100, v010, v110),
            // back
                new Triangle(v001, v101, v011),
                new Triangle(v111, v011, v101),
            // left
                new Triangle(v001, v010, v000),
                new Triangle(v011, v010, v001),
            // right
                new Triangle(v100, v110, v101),
                new Triangle(v101, v110, v111),
            // top
                new Triangle(v010, v111, v110),
                new Triangle(v010, v011, v111),
            // bottom
                new Triangle(v100, v101, v000),
                new Triangle(v101, v001, v000)
            };

            return list;

        }

        [Pure]
        public void TrianglesWithNormals(List<TriangleWithNormals> list )
        {
            var v000 = GetAt(0, 0, 1);
            var v001 = GetAt(0, 0, 0);
            var v010 = GetAt(0, 1, 1);
            var v011 = GetAt(0, 1, 0);
            var v100 = GetAt(1, 0, 1);
            var v101 = GetAt(1, 0, 0);
            var v110 = GetAt(1, 1, 1);
            var v111 = GetAt(1, 1, 0);

            // front
            list.Add(new TriangleWithNormals(v010, v100, v000));
            list.Add(new TriangleWithNormals(v100, v010, v110));
            // back
            list.Add(new TriangleWithNormals(v001, v101, v011));
            list.Add(new TriangleWithNormals(v111, v011, v101));
            // left
            list.Add(new TriangleWithNormals(v001, v010, v000));
            list.Add(new TriangleWithNormals(v011, v010, v001));
            // right
            list.Add(new TriangleWithNormals(v100, v110, v101));
            list.Add(new TriangleWithNormals(v101, v110, v111));
            // top
            list.Add(new TriangleWithNormals(v010, v111, v110));
            list.Add(new TriangleWithNormals(v010, v011, v111));
            // bottom
            list.Add(new TriangleWithNormals(v100, v101, v000));
            list.Add(new TriangleWithNormals(v101, v001, v000));
        }

        [Pure]
        public TriangleWithNormals[] TrianglesWithNormals()
        {
            var v000 = GetAt(0, 0, 1);
            var v001 = GetAt(0, 0, 0);
            var v010 = GetAt(0, 1, 1);
            var v011 = GetAt(0, 1, 0);
            var v100 = GetAt(1, 0, 1);
            var v101 = GetAt(1, 0, 0);
            var v110 = GetAt(1, 1, 1);
            var v111 = GetAt(1, 1, 0);

            var list = new []
            {
            // front
                new TriangleWithNormals(v010, v100, v000),
                new TriangleWithNormals(v100, v010, v110),
            // back
                new TriangleWithNormals(v001, v101, v011),
                new TriangleWithNormals(v111, v011, v101),
            // left
                new TriangleWithNormals(v001, v010, v000),
                new TriangleWithNormals(v011, v010, v001),
            // right
                new TriangleWithNormals(v100, v110, v101),
                new TriangleWithNormals(v101, v110, v111),
            // top
                new TriangleWithNormals(v010, v111, v110),
                new TriangleWithNormals(v010, v011, v111),
            // bottom
                new TriangleWithNormals(v100, v101, v000),
                new TriangleWithNormals(v101, v001, v000)
            };

            return list;

        }

        private Vector3[,,] Vertices
        {
            get
            {
                var _ = new Vector3[2, 2, 2];
                _[0, 0, 0] = GetAt(0, 0, 0);
                _[0, 0, 1] = GetAt(0, 0, 1);
                _[0, 1, 0] = GetAt(0, 1, 0);
                _[0, 1, 1] = GetAt(0, 1, 1);
                _[1, 0, 0] = GetAt(1, 0, 0);
                _[1, 0, 1] = GetAt(1, 0, 1);
                _[1, 1, 0] = GetAt(1, 1, 0);
                _[1, 1, 1] = GetAt(1, 1, 1);
                return _;
            }
        }

        /// <summary>
        /// return the value minimum dimension from Dx, Dy, Dz
        /// </summary>
        public float MinDim => Math.Min(Math.Min(Dx, Dy), Dz);
        public float MaxDim => Math.Max(Math.Max(Dx, Dy), Dz);

        public Vector3 GetAt(int i, int j, int k)
        {
            return new Vector3
                (
                    i == 0 ? XMin : XMax,
                    j == 0 ? YMin : YMax,
                    k == 0 ? ZMin : ZMax
                );
        }

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