using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.DoubleNumerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AForge;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework.OpenGl
{
    public class MeshData
    {
        public MeshData(IReadOnlyList<TriangleWithNormals> triangles, IReadOnlyList<Edge3> edges)
        {
            Triangles = triangles;
            Edges = edges;
        }

        public MeshData()
        {
        }

        public IReadOnlyList<TriangleWithNormals> Triangles { get; }

        public IReadOnlyList<Edge3> Edges { get; }
    }

    public class Mesh : RendererBase<MeshData>
    {
        public static Mesh CreateMesh(IBody2 body, Color color, bool isSolid)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));


            var tris = MeshCache.GetValue(body, bdy =>
            {
                var faceList = bdy.GetFaces().CastArray<IFace2>();
                var tess = GetTess(bdy, faceList);
                return Tesselate(faceList, tess)
                .Buffer(3,3)
                .Select(b=>new TriangleWithNormals(b[0],b[1],b[2])).ToList();
                
            });

            var edges = new List<Edge3>();

            return new Mesh(color, isSolid, tris, edges);
        }

        public Color Color { get; set; }
        private bool _IsSolid;

        private static ConditionalWeakTable<IBody2, List<TriangleWithNormals>> MeshCache = new ConditionalWeakTable<IBody2, List<TriangleWithNormals>>();

        public static Mesh Empty = new Mesh(Color.Black, false, Enumerable.Empty<TriangleWithNormals>());

        public Mesh(Color color, bool isSolid,
            [NotNull] IEnumerable<TriangleWithNormals> tris, IReadOnlyList<Edge3> edges = null):base(new MeshData(tris.ToList(), edges ?? new List<Edge3>()))
        {
            if (tris == null)
                throw new ArgumentNullException(nameof(tris));
            Color = color;
            _IsSolid = isSolid;
        }


        public Mesh(Color color, bool isSolid, IReadOnlyList<Triangle> enumerable, IReadOnlyList<Edge3> edges = null) 
            : this(color, isSolid, enumerable.Select(p=>(TriangleWithNormals)p), edges)
        {
        }


        public IReadOnlyList<Edge3> Edges => _TransformedData.Edges;

        public IReadOnlyList<TriangleWithNormals> TrianglesWithNormals => _TransformedData.Triangles
            ;

        public IReadOnlyList<Triangle> Triangles => 
            TrianglesWithNormals
                .Select(b => (Triangle) b )
                .ToList();


        protected override MeshData DoTransform(MeshData data, Matrix4x4 transform) => new MeshData(TransformTriangles(data,transform), TransformEdges(data, transform));

        protected override void DoRender(MeshData data, DateTime time, double opacity, bool visibility, IDrawContext drawContext)
        {
            if (!visibility)
                return;
            drawContext.DrawMesh(data, opacity, Color, _IsSolid);
        }


        protected override Tuple<Vector3, double> UpdateBoundingSphere(MeshData data, DateTime time)
        {
            var rangeBuilder = new Range3Single.Range3SingleBuilder();
            var count = data.Triangles.Count;
            for (var i = 0; i < count; i++)
            {
                var tri = data.Triangles[i];
                rangeBuilder.Update(tri.A.Point);
                rangeBuilder.Update(tri.B.Point);
                rangeBuilder.Update(tri.C.Point);
            }
            var boundingSphere = rangeBuilder.Range.BoundingSphere();
            return boundingSphere;
        }

        public static List<PointDirection3> Tesselate(IFace2[] faceList, ITessellation tess)
        {
            var r = new List<PointDirection3>();
            // performance improvement
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int index = 0; index < faceList.Length; index++)
            {
                var face = faceList[index];
                foreach (var facet in tess.GetFaceFacets(face).CastArray<int>())
                {
                    var finIds = tess.GetFacetFins(facet).CastArray<int>();
                    var vertexIds = finIds
                        .SelectMany(finId => tess.GetFinVertices(finId).CastArray<int>())
                        .DistinctUntilChanged()
                        .SkipLast(1)
                        .ToList();

                    var vertexs = vertexIds
                        .Select(vId => tess.GetVertexPoint(vId).CastArray<double>())
                        .ToList();

                    var normals = vertexIds
                        .Select(vId => tess.GetVertexNormal(vId).CastArray<double>())
                        .ToList();

                    // Unroll loop for performance
                    r.Add(new PointDirection3(vertexs[0].ToVector3D(), normals[0].ToVector3D()));
                    r.Add(new PointDirection3(vertexs[1].ToVector3D(), normals[1].ToVector3D()));
                    r.Add(new PointDirection3(vertexs[2].ToVector3D(), normals[2].ToVector3D()));
                }
            }

            return r;
        }

        public static ITessellation GetTess(IBody2 body, IFace2[] faceList)
        {
            var tess = (ITessellation)body.GetTessellation(faceList);
            tess.NeedFaceFacetMap = true;
            tess.NeedVertexParams = true;
            tess.NeedVertexNormal = true;
            tess.ImprovedQuality = true;
            tess.NeedEdgeFinMap = true;
            tess.CurveChordTolerance = 0.001 / 10;
            tess.SurfacePlaneTolerance = 0.001 / 10;
            tess.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
            tess.Tessellate();
            return tess;

        }


        public Mesh CreateTransformed(Matrix4x4 transform)
        {
            return new Mesh(Color, _IsSolid, TransformTriangles(_Data, transform), TransformEdges(_Data, transform));
        }

        private static Edge3[] TransformEdges(MeshData data, Matrix4x4 transform)
        {
            var list = new Edge3[data.Edges.Count];

            list.Length
                .ParallelChunked((lower, upper) =>
                {
                    for (int i = lower; i < upper; i++)
                    {
                        list[i] = data.Edges[i].ApplyTransform(transform);
                    }
                });
            return list;
        }

        private static TriangleWithNormals[] TransformTriangles(MeshData data, Matrix4x4 transform)
        {
            var list = new TriangleWithNormals[data.Triangles.Count];

            list.Length
                .ParallelChunked((lower, upper) =>
                {
                    for (int i1 = lower; i1 < upper; i1++)
                    {
                        list[i1] = data.Triangles[i1].ApplyTransform(transform);
                    }
                });
            return list;
        }
    }
}