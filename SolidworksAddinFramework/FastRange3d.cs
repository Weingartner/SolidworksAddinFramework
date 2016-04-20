using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public struct FastRange3D 
    {
        public Vector3 P0 { get; }

        public Vector3 P1 { get; }

        public Vector3 Center => (P0 + P1)/2;

        public FastRange3D(Vector3 p0, Vector3 p1)
        {
            P0 = p0;
            P1 = p1;
        }

        public IEnumerable<Vector3> Extremities
        {
            get
            {
                var @this = this;
                return from x in XRange
                    from y in @this.YRange
                    from z in @this.ZRange
                    select new Vector3(x, y, z);
            }
        }

        public float XMin => Math.Min(P0.X, P1.X);
        public float XMax => Math.Max(P0.X, P1.X);
        public float XMid => (XMin + XMax)/2; 

        public float YMin => Math.Min(P0.Y, P1.Y);
        public float YMax => Math.Max(P0.Y, P1.Y);
        public float YMid => (YMin + YMax)/2; 

        public float ZMin => Math.Min(P0.Z, P1.Z);
        public float ZMax => Math.Max(P0.Z, P1.Z);
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

        public IBody2 ToSolid()
        {
            var modeler = SwAddinBase.Active.Modeler;
            var math = SwAddinBase.Active.Math;
            var center = new double[] {XMid, YMid, ZMin};
            var box = modeler.CreateBox(center, math.ZAxisArray(), this.Dx, this.Dy, this.Dz);
            Debug.Assert(box!=null);
            return box;
        }

        public Mesh ToMesh()
        {
            return new Mesh(Triangles(),Edges());
        }


        public List<IReadOnlyList<Vector3>> Edges()
        {
            var list = new List<IReadOnlyList<Vector3>>(12);

            var _ = Vertices;

            // Bottom square
            list.Add(new [] { _[0,0,0], _[1,0,0] });
            list.Add(new [] { _[1,0,0], _[1,0,1] });
            list.Add(new [] { _[1,0,1], _[0,0,1] });
            list.Add(new [] { _[0,0,1], _[0,0,0] });

            // Top square
            list.Add(new [] { _[0,1,0], _[1,1,0] });
            list.Add(new [] { _[1,1,0], _[1,1,1] });
            list.Add(new [] { _[1,1,1], _[0,1,1] });
            list.Add(new [] { _[0,1,1], _[0,1,0] });

            // Connecting top and bottom lines
            list.Add(new [] { _[0,0,0], _[0,1,0] });
            list.Add(new [] { _[1,0,0], _[1,1,0] });
            list.Add(new [] { _[1,0,1], _[1,1,1] });
            list.Add(new [] { _[0,0,1], _[0,1,1] });

            return list;

        }

        public List<IReadOnlyList<Vector3>> Triangles()
        {

            var _ = Vertices;

            var list = new List<IReadOnlyList<Vector3>>(12)
            {
            // front
                new[] {_[0, 1, 0], _[1, 0, 0], _[0, 0, 0]},
                new[] {_[1, 0, 0], _[0, 1, 0], _[1, 1, 0]},
            // back
                new[] {_[0, 0, 1], _[1, 0, 1], _[0, 1, 1]},
                new[] {_[1, 1, 1], _[0, 1, 1], _[1, 0, 1]},
            // left
                new[] {_[0, 0, 1], _[0, 1, 0], _[0, 0, 0]},
                new[] {_[0, 1, 1], _[0, 1, 0], _[0, 0, 1]},
            // right
                new[] {_[1, 0, 0], _[1, 1, 0], _[1, 0, 1]},
                new[] {_[1, 0, 1], _[1, 1, 0], _[1, 1, 1]},
            // top
                new[] {_[0, 1, 0], _[1, 1, 1], _[1, 1, 0]},
                new[] {_[0, 1, 0], _[0, 1, 1], _[1, 1, 1]},
            // bottom
                new[] {_[1, 0, 0], _[1, 0, 1], _[0, 0, 0]},
                new[] {_[1, 0, 1], _[0, 0, 1], _[0, 0, 0]}
            };

            return list;

        }

        private Vector3[,,] Vertices
        {
            get
            {
                var _ = new Vector3[2, 2, 2];
                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        for (var k = 0; k < 2; k++)
                        {
                            _[i, j, k] = new Vector3
                            (
                                i == 0 ? XMin : XMax,
                                j == 0 ? YMin : YMax,
                                k == 0 ? ZMin : ZMax
                            );
                        }
                    }
                }
                return _;
            }
        }

        public static FastRange3D FromVertices(IReadOnlyList<Vector3>  vertices)
        {
            var xmin = float.MaxValue;
            var ymin = float.MaxValue;
            var zmin = float.MaxValue;

            var xmax = float.MinValue;
            var ymax = float.MinValue;
            var zmax = float.MinValue;

            // performance optimization. Avoid LINQ
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < vertices.Count; i++)
            {

                var vertex = vertices[i];
                var v0 = vertex.X;
                var v1 = vertex.Y;
                var v2 = vertex.Z;

                xmin = Math.Min(xmin, v0);
                xmax = Math.Max(xmax, v0);

                ymin = Math.Min(ymin, v1);
                ymax = Math.Max(ymax, v1);

                zmin = Math.Min(zmin, v2);
                zmax = Math.Max(zmax, v2);
            }

            return new FastRange3D(new Vector3(xmin,ymin,zmin), new Vector3(xmax, ymax, zmax));

        }

        public FastRange3D Scale(float s)
        {
            s = (s - 1)/2;
            var sx = Dx*s;
            var sy = Dy*s;
            var sz = Dz*s;
            return new FastRange3D(new Vector3(XMin-sx,YMin-sy,ZMin-sz), new Vector3(XMax+sx, YMax+sy, ZMax+sz));
        }

        public bool Intersects(FastRange3D other)
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

        public FastRange3D Intersect(FastRange3D other)
        {
            var xrange = XRange.Intersect(other.XRange);
            var yrange = YRange.Intersect(other.YRange);
            var zrange = ZRange.Intersect(other.ZRange);
            return new FastRange3D
                (new Vector3(xrange.Min, yrange.Min, zrange.Min),
                 new Vector3(xrange.Max, yrange.Max, zrange.Max));

        }

    }
}