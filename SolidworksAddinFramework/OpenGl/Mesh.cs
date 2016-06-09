using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
    public class Mesh : RenderableBase
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

        private readonly IReadOnlyList<TriangleWithNormals> _OriginalTriangleVerticies;
        private readonly IReadOnlyList<Edge3> _OriginalEdgeVertices;
        public Color Color { get; }
        private bool _IsSolid;

        private static ConditionalWeakTable<IBody2, List<TriangleWithNormals>> MeshCache = new ConditionalWeakTable<IBody2, List<TriangleWithNormals>>();

        public Mesh(Color color, bool isSolid,
            [NotNull] IEnumerable<TriangleWithNormals> tris, IReadOnlyList<Edge3> edges = null)
        {
            if (tris == null)
                throw new ArgumentNullException(nameof(tris));

            Color = color;
            _OriginalTriangleVerticies = tris.ToList();
            _OriginalEdgeVertices = edges ?? new List<Edge3>();
            _IsSolid = isSolid;

            TrianglesWithNormals = _OriginalTriangleVerticies;
            Edges = _OriginalEdgeVertices;

            UpdateBoundingSphere();
        }


        public static Mesh Combine(IEnumerable<Mesh> meshes, Color color)
        {
            var tris = meshes.SelectMany(mesh => mesh.TrianglesWithNormals);
            var edges = meshes.SelectMany(mesh => mesh.Edges).ToList();
            return new Mesh(color, false,tris, edges);
        }



        public Mesh(Color color, bool isSolid, IReadOnlyList<Triangle> enumerable, IReadOnlyList<Edge3> edges = null) 
            : this(color, isSolid, enumerable.Select(p=>(TriangleWithNormals)p), edges)
        {
        }

        private void UpdateBoundingSphere()
        {
            UpdateBoundingSphere(CreateBoundingSphere);
        }

        private Tuple<Vector3, float> CreateBoundingSphere()
        {
            var rangeBuilder = new Range3Single.Range3SingleBuilder();
            var count = TrianglesWithNormals.Count;
            for (var i = 0; i < count; i++)
            {
                var tri = TrianglesWithNormals[i];
                rangeBuilder.Update(tri.A.Point);
                rangeBuilder.Update(tri.B.Point);
                rangeBuilder.Update(tri.C.Point);
            }
            var boundingSphere = rangeBuilder.Range.BoundingSphere();
            return boundingSphere;
        }


        public IReadOnlyList<Edge3> Edges { get; private set; }

        public IReadOnlyList<TriangleWithNormals> TrianglesWithNormals { get; set; }

        public IReadOnlyList<Triangle> Triangles => 
            TrianglesWithNormals
                .Select(b => (Triangle) b )
                .ToList();


        public override void Render(DateTime time)
        {
            MeshRender.Render(this, Color, _IsSolid);
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

        /// <summary>
        /// Apply the transform to the ORIGINAL mesh that was created. Multiple
        /// calls are NOT cumulative.
        /// </summary>
        /// <param name="transform"></param>
        public override void ApplyTransform(Matrix4x4 transform)
        {
            {
                var list = new TriangleWithNormals[_OriginalTriangleVerticies.Count];

                list.Length
                    .ParallelChunked((lower, upper) =>
                    {
                        for (int i1 = lower; i1 < upper; i1++)
                        {
                            list[i1] = _OriginalTriangleVerticies[i1].ApplyTransform(transform);
                        }
                    });

                TrianglesWithNormals = list;
                
            }
            {
                var list = new Edge3[_OriginalEdgeVertices.Count]; 

                list.Length
                    .ParallelChunked((lower, upper) =>
                    {
                        for (int i = lower; i < upper; i++)
                        {
                            list[i] = _OriginalEdgeVertices[i].ApplyTransform(transform);

                        }
                    
                    });

                Edges = list;
                
            }
            UpdateBoundingSphere();

        }

    }
}