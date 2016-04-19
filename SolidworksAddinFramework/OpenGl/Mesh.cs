using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework.OpenGl
{

    public class Mesh : IRenderable
    {
        private readonly IReadOnlyList<Tuple<double[], double[]>> _OriginalTriangleVerticies;
        private List<IReadOnlyList<double[]>> _OriginalEdgeVertices;

        public Mesh(IReadOnlyList<Tuple<double[], double[]>> triangleVertices)
        {
            TriangleVertices = triangleVertices;
        }
        public Mesh(IReadOnlyList<IReadOnlyList<Tuple<double[], double[]>>> triangleVertices)
        {
            TriangleVertices = triangleVertices.SelectMany(p=>p).ToList();
        }
        public Mesh(IReadOnlyList<IReadOnlyList<double[]>> triangleVertices)
        {
            TriangleVertices = triangleVertices.SelectMany(ps=>ps.Select(p=>Tuple.Create(p,(double[])null))).ToList();
        }

        public Mesh(IBody2 body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            var faceList = body.GetFaces().CastArray<IFace2>();
            var tess = GetTess(body, faceList);
            var tris = Tesselate(faceList, tess);
            var edges = Edges(faceList, tess);
            FaceEdges = edges.ToList();
            TriangleVertices = tris.ToList();
            _OriginalTriangleVerticies = TriangleVertices;
            _OriginalEdgeVertices = FaceEdges;
        }

        public List<IReadOnlyList<double[]>> FaceEdges { get; private set; }

        public IReadOnlyList<Tuple<double[], double[]>> TriangleVertices { get; set; }

        public IReadOnlyList<IReadOnlyList<double[]>> Triangles =>
            TriangleVertices
                .Buffer(3, 3)
                .Select(b => b.Select(i => i.Item1).ToList())
                .ToList();

        public IReadOnlyList<IReadOnlyList<DenseVector>> DenseTriangles => 
            TriangleVertices
                .Buffer(3, 3)
                .Select(b => b.Select(i => (DenseVector) i.Item1).ToList())
                .ToList();


        public void Render(DateTime time)
        {
            MeshRender.Render(this, Color);
        }

        public Color Color { get; set; } = Color.Red;

        public static IEnumerable<IReadOnlyList<double[]>> Edges(IFace2[] faceList, ITessellation tess)
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
                        .Select(vId => tess.GetVertexPoint(vId).CastArray<double>())
                        .ToList()
                    ).ToList());
        }

        public static IEnumerable<Tuple<double[], double[]>> Tesselate(IFace2[] faceList, ITessellation tess)
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
                        yield return Tuple.Create(vertexs[i], normals[i]);
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
        public void ApplyTransform(IMathTransform transform)
        {
            DenseMatrix rotation;
            DenseVector translation;
            transform.ExtractTransform(out rotation, out translation);

            TriangleVertices = _OriginalTriangleVerticies.Select(pn =>
            {
                var p = rotation * pn.Item1 + translation;
                var n = rotation * pn.Item2;
                return Tuple.Create(p.Values, n.Values);
            }).ToList();

            FaceEdges = _OriginalEdgeVertices.Select(pn =>
            {
                return pn
                .Select(p => (rotation*p + translation).Values)
                .ToList() as IReadOnlyList<double[]>;
            }).ToList();

        }
    }
}