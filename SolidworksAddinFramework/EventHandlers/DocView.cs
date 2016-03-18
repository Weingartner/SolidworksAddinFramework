using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using static OpenGl;


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

    public class DocView
    {
        private ISldWorks _ISwApp;
        private SwAddinBase _UserAddin;
        private ModelView _MView;
        private DocumentEventHandler _Parent;

        public DocView(SwAddinBase addin, IModelView mv, DocumentEventHandler doc)
        {
            _UserAddin = addin;
            _MView = (ModelView)mv;
            _ISwApp = (ISldWorks)_UserAddin.SwApp;
            _Parent = doc;
        }

        public bool AttachEventHandlers()
        {
            _MView.DestroyNotify2 += OnDestroy;
            _MView.RepaintNotify += OnRepaint;
            _MView.BufferSwapNotify += OnBufferSwapNotify;
            return true;
        }

        public static ConcurrentDictionary<IBody2, TessellatableBody> BodiesToRender = new ConcurrentDictionary<IBody2, TessellatableBody>();

        public static IDisposable DisplayUndoable(TessellatableBody body, Color? color, IModelDoc2 doc)
        {
            BodiesToRender[body.Body]=body;
            ((IModelView)doc.ActiveView).GraphicsRedraw(null);
            return Disposable.Create(() =>
            {
                TessellatableBody dummy;
                BodiesToRender.TryRemove(body.Body, out dummy);
            });
        }
        public static IDisposable DisplayUndoable(IBody2 body, Color? color, IModelDoc2 doc, IMathUtility math)
        {
            BodiesToRender[body]=new TessellatableBody(math,body);
            return Disposable.Create(() =>
            {
                TessellatableBody dummy;
                BodiesToRender.TryRemove(body, out dummy);
            });
        }

        private int OnBufferSwapNotify()
        {

            foreach (var o in BodiesToRender.Values)
            {
                o.Tesselation.RenderOpenGL(_ISwApp);
            }

            //foreach (var body in ModelDoc.GetBodiesTs())
            //{
            //    MeshRender.Render(body, _ISwApp);
            //}

            //const int GL_LINES = 1;

            //glLineWidth(3);

            //glBegin(GL_LINES);
            //glVertex3f(0.0F, 0.0F, 0.0F);
            //glVertex3f(0.5F, 0.5F, 0.5F);
            //glEnd();


            return 0;
        }

        private IModelDoc2 ModelDoc => ((IModelDoc2)_MView.GetModelDoc());

        public bool DetachEventHandlers()
        {
            _MView.DestroyNotify2 -= OnDestroy;
            _MView.RepaintNotify -= OnRepaint;
            _Parent.DetachModelViewEventHandler(_MView);
            return true;
        }

        //EventHandlers
        public int OnDestroy(int destroyType)
        {
            return 0;
        }

        public int OnRepaint(int repaintType)
        {
            return 0;
        }
    }

}
