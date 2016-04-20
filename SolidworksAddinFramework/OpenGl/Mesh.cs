using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework.OpenGl
{

    public class Mesh : IRenderable
    {
        private readonly IReadOnlyList<Tuple<Vector3, Vector3>> _OriginalTriangleVerticies;
        private IReadOnlyList<IReadOnlyList<Vector3>> _OriginalEdgeVertices;

        public Mesh(IReadOnlyList<Tuple<Vector3, Vector3>> triangleVertices)
        {
            TriangleVertices = triangleVertices;
        }
        public Mesh(IReadOnlyList<IReadOnlyList<Tuple<Vector3, Vector3>>> triangleVertices)
        {
            TriangleVertices = triangleVertices.SelectMany(p=>p).ToList();
        }
        public Mesh(IReadOnlyList<IReadOnlyList<Vector3>> triangleVertices, IReadOnlyList<IReadOnlyList<Vector3>> edges )
        {
            Edges = edges;
            TriangleVertices = triangleVertices.SelectMany(ps=>ps.Select(p=>Tuple.Create(p,default(Vector3)))).ToList();
        }

        public Mesh(IBody2 body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            var faceList = body.GetFaces().CastArray<IFace2>();
            var tess = GetTess(body, faceList);
            var tris = Tesselate(faceList, tess);
            var edges = EdgesFromTesselation(faceList, tess);
            Edges = edges.ToList();
            TriangleVertices = tris.ToList();
            _OriginalTriangleVerticies = TriangleVertices;
            _OriginalEdgeVertices = Edges;
        }

        public IReadOnlyList<IReadOnlyList<Vector3>> Edges { get; private set; }

        public IReadOnlyList<Tuple<Vector3, Vector3>> TriangleVertices { get; set; }

        public IReadOnlyList<IReadOnlyList<Vector3>> Triangles =>
            TriangleVertices
                .Buffer(3, 3)
                .Select(b => b.Select(i => i.Item1).ToList())
                .ToList();

        public IReadOnlyList<IReadOnlyList<Vector3>> DenseTriangles => 
            TriangleVertices
                .Buffer(3, 3)
                .Select(b => b.Select(i =>  i.Item1).ToList())
                .ToList();


        public void Render(DateTime time)
        {
            MeshRender.Render(this, Color);
        }

        public Color Color { get; set; } = Color.Red;

        public static IEnumerable<IReadOnlyList<Vector3>> EdgesFromTesselation(IFace2[] faceList, ITessellation tess)
        {
            return faceList
                .Select(face => face
                    .GetEdges()
                    .CastArray<IEdge>()
                    .Select(edge => tess.GetEdgeFins(edge).CastArray<int>())
                    .Distinct()
                    .ToList()
                    .SelectMany(f=>f
                        .SelectMany(finId => tess.GetFinVertices(finId).CastArray<int>())
                        .DistinctUntilChanged()
                        .Select(vId => tess.GetVertexPoint(vId).CastArray<double>().ToVector3D())
                        .ToList()
                    ).ToList());
        }

        public static IEnumerable<Tuple<Vector3, Vector3>> Tesselate(IFace2[] faceList, ITessellation tess)
        {
            foreach (var face in faceList)
            {

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

                    foreach (var i in Enumerable.Range(0, 3))
                    {
                        yield return Tuple.Create(Vector3Extensions.ToVector3D(vertexs[i]), Vector3Extensions.ToVector3D(normals[i]));
                    }
                }
            }
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

        /// <summary>
        /// Apply the transform to the ORIGINAL mesh that was created. Multiple
        /// calls are NOT cumulative.
        /// </summary>
        /// <param name="transform"></param>
        public void ApplyTransform(Matrix4x4 transform)
        {
            Vector3 scale;
            Quaternion rotation;
            Vector3 translation;
            Matrix4x4.Decompose(transform, out scale, out rotation, out translation);

            TriangleVertices = _OriginalTriangleVerticies.Select(pn =>
            {
                var p = Vector3.Transform(pn.Item1,transform);
                var n = Vector3.Transform(pn.Item2, rotation);
                return Tuple.Create(p, n);
            }).ToList();

            Edges = _OriginalEdgeVertices.Select(pn =>
            {
                return pn
                .Select(p => Vector3.Transform(p, transform))
                .ToList();
            }).ToList();

        }
    }
}