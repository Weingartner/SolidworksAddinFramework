using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DoubleNumerics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LanguageExt;
using SolidworksAddinFramework.Geometry;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Drawing;
using Curve = SolidWorks.Interop.sldworks.Curve;
using ICurve = SolidWorks.Interop.sldworks.ICurve;
using LogViewer = Weingartner.WeinCad.Interfaces.LogViewer;
using Mesh = Weingartner.WeinCad.Interfaces.Drawing.Mesh;

namespace SolidworksAddinFramework
{
    public static class BodyExtensions
    {
        private static ConditionalWeakTable<IBody2, List<TriangleWithNormals>> MeshCache = new ConditionalWeakTable<IBody2, List<TriangleWithNormals>>();
        private static string ExportGeometryFileExtension = ".igs";

        public static Mesh CreateMesh(this IBody2 body, Color color, bool isSolid)
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
                    var vertexIds = Enumerable.ToList<int>(Enumerable.SelectMany<int, int>(finIds, finId => tess.GetFinVertices(finId).CastArray<int>())
                                               .DistinctUntilChanged()
                                               .SkipLast(1));

                    var vertexs = vertexIds
                        .Select<int, double[]>(vId => tess.GetVertexPoint(vId).CastArray<double>())
                        .ToList();

                    var normals = vertexIds
                        .Select<int, double[]>(vId => tess.GetVertexNormal(vId).CastArray<double>())
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


        public static IBody2 Add(this IEnumerable<IBody2> bodies)
        {
            return bodies.Aggregate((a, acc) => a.Add(acc).Bodies[0]);
        }

        /// <summary>
        /// Result for OperationsTs call.
        /// </summary>
        public class OperationsResult
        {
            /// <summary>
            /// If 0 then there is no error
            /// </summary>
            public int Error { get; }

            /// <summary>
            /// If error == 0 then this contains the resulting bodies from the operations
            /// </summary>
            public IBody2[] Bodies { get; }

            public OperationsResult(int error, IBody2[] bodies)
            {
                Error = error;
                Bodies = bodies;
            }
        }

        #region boolean operations
        /// <summary>
        /// Perform Add, Cut, Intersect operations on solid bodies. 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="type"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static OperationsResult OperationsTs(this IBody2 body, swBodyOperationType_e type, IBody2 tool)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            tool = tool.CopyTs();
            body = body.CopyTs();

            int error;
            var objects = (object[]) body.Operations2((int) type, tool, out error);
            if(objects==null)
                return new OperationsResult(error, new IBody2[] {});

            var bodies = error == 0 ?  objects.Cast<IBody2>().ToArray() : new IBody2[] {};
            return new OperationsResult(error, bodies);
        }

        public static OperationsResult Cut(this IBody2 body , IBody2 tool)
        {
            return body.OperationsTs(swBodyOperationType_e.SWBODYCUT, tool);
        } 
        public static OperationsResult Add(this IBody2 body , IBody2 tool)
        {
            return body.OperationsTs(swBodyOperationType_e.SWBODYADD, tool);
        } 
        public static OperationsResult Intersect(this IBody2 body , IBody2 tool)
        {
            return body.OperationsTs(swBodyOperationType_e.SWBODYINTERSECT, tool);
        }
        #endregion

        public static void HideAll(this IEnumerable<IBody2> bodies, IModelDoc2 doc)
        {
            foreach (var body in bodies)
            {
                body.Hide(doc);
            }
        }

        public static void DisplayTs([NotNull] this IBody2 body ,IModelDoc2 doc = null, Color? c = null, swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));
            doc = doc ?? (IModelDoc2)SwAddinBase.Active.SwApp.ActiveDoc;
            c = c ?? Color.Yellow;
            var colorref = ColorTranslator.ToWin32(c.Value);
            body.Display3(doc, colorref, (int) opt);
        }

        public static void DisplayAll(this IEnumerable<IBody2> bodies, IModelDoc2 doc, Color c, swTempBodySelectOptions_e opt)
        {
            foreach (var body in bodies)
            {
                body.DisplayTs(doc, c, opt);
            }
        }

        public static bool ApplyTransform(this IBody2 body, Matrix4x4 t)
        {
            var transform = SwAddinBase.Active.Math.ToSwMatrix(t);
            return body.ApplyTransform(transform);
        }

        /// <summary>
        /// Return the bounding box of the solid renderable.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Range3Double GetBodyBoxTs(this IBody2 body)
        {
            var box = (double[]) body.GetBodyBox();
            return new Range3Double(
                new Vector3(box[0], box[1], box[2]), 
                new Vector3(box[3], box[4], box[5]));
        }

        public static bool GetDistance(this IBody2 body0, IBody2 body1, out double[] p0, out double []p1)
        {
            var faces0 = body0.GetFaces().CastArray<IFace2>();
            var faces1 = body1.GetFaces().CastArray<IFace2>();
            var distances = GetDistances(faces0, faces1);

            p0 = null;
            p1 = null;


            var shortest = distances.MinBy(t => MathPointExtensions.Distance(t.Item1, t.Item2));
            if (shortest.Count == 0)
                return false;

            p0 = shortest[0].Item1;
            p1 = shortest[0].Item2;

            return true;
        }


        public static IEnumerable<Tuple<double[], double[]>> GetDistances(IEnumerable<IFace2> entities0, IEnumerable<IFace2> entities1)
        {
            var entities1List = entities1 as IList<IFace2> ?? entities1.ToList();

            foreach (var entity0 in entities0)
            {
                foreach (var entity1 in entities1List)
                {
                    double[] posacast;
                    double[] posbcast;
                    if(entity0.GetDistance(entity1, out posacast, out posbcast))
                        yield return Tuple.Create( posacast, posbcast);
                    else
                    {
                        throw new Exception("Unable to get distance between faces");
                    }
                }
            }
        }


        /// <summary>
        /// Get the edges formed by intersection both bodies together. The bodies are first copied 
        /// before intersecting so that the input bodies are not modified.
        /// </summary>
        /// <param name="toolBody"></param>
        /// <param name="cuttingPlane"></param>
        /// <returns></returns>
        public static List<IEdge> GetIntersectionEdgesNonDestructive(this IBody2 toolBody, IBody2 cuttingPlane)
        {
            toolBody = (IBody2) toolBody.Copy();
            cuttingPlane = (IBody2) cuttingPlane.Copy();
            var enumerable = ((object[]) toolBody.GetIntersectionEdges(cuttingPlane)).Cast<IEdge>().ToList();
            return enumerable;
        }

        public static IBody2 CopyTs(this IBody2 tool)
        {
            return (IBody2) tool.Copy();
        }

        public static List<ICurve> GetXzCrossSectionCurves(this IBody2 body)
        {
            var modeler = SwAddinBase.Active.Modeler;
            var yZPlane = ((ISurface)modeler.CreatePlanarSurface2(new[] { 0, 0, 0.0 }, new[] { 1.0, 0, 0 }, new[] { 0, 0, 1.0 })).ToSheetBody();

            var cutBody = body
                    .CutBySheets(new[] {yZPlane})
                    .First(b => b.GetBodyBoxTs().XMax > 1e-5);

            var zXPlane = ((ISurface)modeler.CreatePlanarSurface2(new[] { 0, 0, 0.0 }, new[] { 0, 1.0, 0 }, new[] { 0, 0, 1.0 })).ToSheetBody();
            var edges = cutBody.GetIntersectionEdgesNonDestructive(zXPlane)
                //GetIntersectionEdges always returns the edges on the target and on the tool!
                .Where((p,i)=>i%2==0) 
                .ToList();

            return edges
                .Select(e => e.GetCurveTs())
                .Where(c => !(Math.Abs(c.StartPoint().X) < 1e-6 && Math.Abs(c.EndPoint().X) < 1e-6))
                .Distinct()
                .ToList();
        }


        /// <summary>
        /// Assumes that the tools are non overlapping sheets and that the sheets fully
        /// overlap the target. The target should be cut into N+1 parts of there are N
        /// tools. They 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tools"></param>
        /// <returns></returns>
        public static IEnumerable<IBody2> CutBySheets(this IBody2 target, IEnumerable<IBody2> tools)
            {
                var targets = new List<IBody2> { target.CopyTs() };
                foreach (var tool in tools)
                {
                    targets = targets.SelectMany
                        (tgt =>
                        {
                            var result = tgt.Cut(tool);
                            if (result.Error != 0)
                                return new[] { tgt };
                             //throw new Exception("Tool was unable to cut");
                             return result.Bodies;
                        })
                        .ToList();
                }
                return targets;
            }


        public static Option<Curve> GetIntersectionCurve(this IBody2 toolBody, IBody2 cuttingPlane, IModeler modeler)
        {
            var innerCurves =
                toolBody.GetIntersectionEdgesNonDestructive(cuttingPlane)
                    .Buffer(2, 2)
                    .Select(b => (ICurve) b[0].GetCurve())
                    .Select(c=>(ICurve)c.Copy())
                    .ToArray();

            if (innerCurves.Length <= 0)
                return Prelude.None;

            var curve = modeler.MergeCurves(innerCurves);
            return Prelude.Optional(curve);
        }
        private static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid() + extension;
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Save the iges format of the body into the stream
        /// </summary>
        /// <param name="body"></param>
        /// <param name="handler"></param>
        /// <param name="hidden"></param>
        public static void SaveAsIges(this IBody2 body, Action<Stream> handler, bool hidden)
        {
            var igesFile = GetTempFilePathWithExtension(ExportGeometryFileExtension);
            try
            {
                SaveAsIges(body, igesFile, hidden);
                using (var stream = File.OpenRead(igesFile))
                {
                    handler(stream);
                };
            }
            finally
            {
                File.Delete(igesFile);
            }
            
        }

        public static T SaveAsIges<T>(this IBody2 body, bool hidden, Func<Stream, T> selector)
        {
            T r = default(T);
            SaveAsIges
                (body,
                    stream =>
                    {
                        r = selector(stream);
                    },hidden);
            return r;
        }

        /// <summary>
        /// Load the iges format of the body from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static List<IBody2> LoadBodiesAsIges(Stream stream)
        {
            var igesFile = GetTempFilePathWithExtension(ExportGeometryFileExtension);
            LogViewer.Log($"Loading iges file {igesFile} into Solidworks");
            using (var ostream = File.OpenWrite(igesFile))
            {
                stream.CopyTo(ostream);
            }
            var doc = SwAddinBase.Active.SwApp.LoadIgesInvisible(igesFile);
            try
            {
                var newPartDoc = (PartDoc) doc;
                Debug.Assert(newPartDoc!=null);

                var loadedBody =
                    newPartDoc.GetBodies2((int) swBodyType_e.swAllBodies, false).CastArray<IBody2>().ToList();
                Debug.Assert(loadedBody.Count > 0);

                return loadedBody.Select(p => p.CopyTs()).ToList();
            }
            finally
            {
                SwAddinBase.Active.SwApp.QuitDoc(doc.GetTitle());
                
            }
        }

        public static void SaveAsIges(this IBody2 body, string igesFile, bool hidden = false)
        {

            SldWorks app = SwAddinBase.Active.SwApp;

            var doc = app.CreateHiddenDocument(hidden: hidden);

            doc.Extension.SetUserPreferenceInteger
                ((int)(swUserPreferenceIntegerValue_e.swUnitSystem)
                , 0
                , ((int)(swUnitSystem_e.swUnitSystem_MKS)));

            try
            {
                var partDoc = (PartDoc)doc;

                partDoc.CreateFeatureFromBody3(body, false, 0);

                int errorsi = 0;
                int warningsi = 0;

                app.ActivateDoc3
                    (doc.GetTitle(), false, (int)swRebuildOnActivation_e.swRebuildActiveDoc, ref errorsi);


                // http://help.solidworks.com/2013/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IModelDocExtension~SaveAs.html
                // Note the doc says:
                // To save as an IGES, STL, or STEP file, the document to convert must be the active document. Before calling this method:
                // Call ISldWorks::ActivateDoc3 to make the document to convert the active document.
                // Call ISldWorks::ActiveDoc to get the active document.

                var status = doc // Note that this does not return the correct doc if the doc is hidden
                    .Extension
                    .SaveAs
                        ( igesFile
                        , (int)swSaveAsVersion_e.swSaveAsCurrentVersion
                        , (int)swSaveAsOptions_e.swSaveAsOptions_Silent
                        , null
                        , ref errorsi
                        , ref warningsi
                        );

                if(!status)
                    throw new Exception($"Failed to save {igesFile}. Got error bitmask {errorsi}");

            }
            finally
            {
                app.QuitDoc(doc.GetTitle());
                
            }


        }
    }
    

    public static class DisplayTransaction
    {
        public static IDisposable DisplayUndoable(this IEnumerable<IBody2> bodies, IModelDoc2 doc,
            Color? c = null,
            swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
                var view = (IModelView)doc.ActiveView;
                using (view.DisableGraphicsUpdate())
                {
                    var d = bodies.Select(toolBody => toolBody.DisplayUndoable(doc, c)).ToCompositeDisposable();
                    return Disposable.Create(() =>
                    {
                        using (view.DisableGraphicsUpdate())
                        {
                            d.Dispose();
                        }
                    });
                }

        }

        public static BodyMaterials GetMaterial(this IBody2 body) => new BodyMaterials(body);
        public static Color GetColor(this IBody2 body) => body.GetMaterial().Color;
        public static void SetColor(this IBody2 body, Color color)
        {
            body.GetMaterial().Color = color;

        }

        public static IDisposable DisplayUndoable
            (this IRenderer renderer, IModelDoc2 doc, int layer = 0)
        {
            return OpenGlRenderer.DisplayUndoable(renderer, doc, layer);
        }
        public static IDisposable DisplayUndoable
            (this IEnumerable<IRenderer> renderable, IModelDoc2 doc, int layer = 0)
        {
            return OpenGlRenderer.DeferRedraw
                ( doc
                , ()=>renderable.Select(r=>r.DisplayUndoable(doc, layer)).ToCompositeDisposable()
                );
        }

        /// <summary>
        /// Uses Display3 to render the object. This is slow for animation. Better to create a Mesh and then render it. This
        /// will use OpenGL directly.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="doc"></param>
        /// <param name="c"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static IDisposable DisplayUndoable(this IBody2 body, IModelDoc2 doc, Color? c = null, swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            body.DisplayTs(doc, c, opt);
            return Disposable.Create(() => body.Hide(doc));
        }

        public static IDisposable DisplayBodiesUndoable(this IEnumerable<IBody2> bodies, IModelDoc2 doc, Color? c = null,
            swTempBodySelectOptions_e opt = swTempBodySelectOptions_e.swTempBodySelectOptionNone)
        {
            return new CompositeDisposable(bodies.Select(b=>b.DisplayUndoable(doc,c,opt)));
        }

        public static IDisposable HideBodyUndoable(this IBody2 body)
        {
            body.HideBody(true);
            return Disposable.Create(() =>
            {
                body.HideBody(false);
            });
        }
        public static IDisposable HideBodiesUndoable(this IEnumerable<IBody2> bodies)
        {
            return new CompositeDisposable(bodies.Select(b=>b.HideBodyUndoable()));
        }

    }
}