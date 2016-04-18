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
        private IList<Tuple<double[], double[]>> _OriginalTriangleVerticies;

        public Mesh(IList<Tuple<double[], double[]>> triangleVertices)
        {
            TriangleVertices = triangleVertices;
        }

        public Mesh(IBody2 body)
        {
            var faceList = body.GetFaces().CastArray<IFace2>();
            var tess = GetTess(body, faceList);
            var tris = Tesselate(faceList, tess);
            TriangleVertices = tris.ToList();
            _OriginalTriangleVerticies = TriangleVertices;
        }

        public IList<Tuple<double[], double[]>> TriangleVertices { get; set; }

        public void Render(DateTime time)
        {
            MeshRender.Render(this, Color);
        }

        public Color Color { get; set; } = Color.Red;

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
            tess.CurveChordTolerance = 0.001 / 10;
            tess.SurfacePlaneTolerance = 0.001 / 10;
            tess.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
            tess.Tessellate();
            return tess;

        }

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
        }
    }
}