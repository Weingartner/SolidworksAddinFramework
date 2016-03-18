using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public class Mesh
    {
        private IList<Tuple<double[], double[]>> _OriginalTriangleVerticies;

        public Mesh(IList<Tuple<double[], double[]>> triangleVertices)
        {
            TriangleVertices = triangleVertices;
        }

        public Mesh(IBody2 body)
        {
            var faceList = body.GetFaces().CastArray<IFace2>();
            var tess = GetTess(body,faceList);
            var tris = Tesselate(faceList, tess);
            TriangleVertices = tris.ToList();
            _OriginalTriangleVerticies = TriangleVertices;
        }

        public IList<Tuple<double[], double[]>> TriangleVertices { get; set; }

        public void RenderOpenGL(ISldWorks iSwApp)
        {
            MeshRender.Render(this, iSwApp );
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

                    {
                        for (int i = 0; i < 3; i++)
                        {
                            yield return Tuple.Create(vertexs[i], normals[i]);
                        }
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
            tess.CurveChordTolerance = 0.001/10;
            tess.SurfacePlaneTolerance = 0.001/1;
            tess.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
            tess.Tessellate();
            return tess;

        }

        public void ApplyTransform(MathTransform transform)
        {
            var transformArray = transform.ArrayData.CastArray<double>();
            var rotation = new DenseMatrix(3,3,transformArray.Take(9).ToArray());
            var translation = new DenseVector(transformArray.Skip(9).Take(3).ToArray());

            TriangleVertices = _OriginalTriangleVerticies.Select(pn =>
            {
                var p = new DenseVector(pn.Item1);
                var n = new DenseVector(pn.Item2);
                p = rotation*p + translation;
                n = rotation*n;
                return Tuple.Create(p.Values, n.Values);
            }).ToList();

        }
    }
}