using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Security;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework.OpenGl
{
    public static class Wgl
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("OPENGL32.DLL", EntryPoint = "wglGetProcAddress", SetLastError = true)]
        internal static extern IntPtr GetProcAddress(string lpszProc);
    }

    public static class MeshRender
    {
        public static float[] GetMaterial(MaterialFace materialFace, MaterialParameter materialParameter)
        {
            var @params = new float[4];
            GL.GetMaterial(materialFace, materialParameter, @params);
            return @params;
        }

        public static IDisposable SetColor(Color value)
        {
            // store current settings
            var materialFace = MaterialFace.Front;
            var currentAmbient = GetMaterial(materialFace, MaterialParameter.Ambient);
            var currentDiffuse = GetMaterial(materialFace, MaterialParameter.Diffuse);
            var currentSpecular = GetMaterial(materialFace, MaterialParameter.Specular);
            var currentEmmission = GetMaterial(materialFace, MaterialParameter.Emission);
            var currentShininess = GetMaterial(materialFace, MaterialParameter.Shininess);

            // set default values (see https://www.opengl.org/sdk/docs/man2/xhtml/glMaterial.xml)
            GL.Material(materialFace, MaterialParameter.Ambient, new[] { 0.2f, 0.2f, 0.2f, 1.0f });
            GL.Material(materialFace, MaterialParameter.Diffuse, new[] { 0.8f, 0.8f, 0.8f, 1.0f });
            GL.Material(materialFace, MaterialParameter.Specular, new[] { 0.0f, 0.0f, 0.0f, 1.0f });
            GL.Material(materialFace, MaterialParameter.Emission, new[] { 0.0f, 0.0f, 0.0f, 1.0f });
            GL.Material(materialFace, MaterialParameter.Shininess, new[] { 0.0f });

            var color = new[] { value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f };
            // SW only draws front faces but we draw both
            GL.Material(materialFace, MaterialParameter.AmbientAndDiffuse, color);
            GL.Material(materialFace, MaterialParameter.Shininess, new [] { 100.0f });
            GL.Material(materialFace, MaterialParameter.Specular, new[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(materialFace, MaterialParameter.Diffuse, new[] { 1f, 0f, 0, 1f });

            return Disposable.Create(() =>
            {
                GL.Material(materialFace, MaterialParameter.Ambient, currentAmbient);
                GL.Material(materialFace, MaterialParameter.Diffuse, currentDiffuse);
                GL.Material(materialFace, MaterialParameter.Specular, currentSpecular);
                GL.Material(materialFace, MaterialParameter.Emission, currentEmmission);
                GL.Material(materialFace, MaterialParameter.Shininess, currentShininess);
            });
        }

        public static IDisposable SetLineWidth(float value)
        {
            var currentLineWidth = GL.GetFloat(GetPName.LineWidth);
            GL.LineWidth(value);
            return Disposable.Create(() => GL.LineWidth(currentLineWidth));
        }

        public static IDisposable Begin(PrimitiveType mode)
        {
            GL.Begin(mode);
            return Disposable.Create(GL.End);
        }

        private static bool _Setup;

        public static ITessellation GetTess(IBody2 body, IFace2[] faceList)
        {
            var tess = (ITessellation)body.GetTessellation(faceList);
            tess.NeedFaceFacetMap = true;
            tess.NeedVertexParams = true;
            tess.NeedVertexNormal = true;
            tess.ImprovedQuality = true;
            tess.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
            tess.Tessellate();
            return tess;

        }

        /// <summary>
        /// This is slow and sux due tothe overhead of making many many COM calls. Do
        /// not use it but it is a good reference.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="app"></param>
        public static void Render(IBody2 body, ISldWorks app)
        {
            DoSetup(app);

            var faceList = body.GetFaces().CastArray<IFace2>();
            var tess = GetTess(body,faceList);

            // Do it

            GL.ShadeModel(ShadingModel.Flat);
            using (SetColor(Color.Blue))
            using (SetLineWidth(2.0f))
            using (Begin(PrimitiveType.Triangles))
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
                                GL.Vertex3(vertexs[i]);
                                GL.Normal3(normals[i]);
                                
                            }

                        }


                    }

                }
            }
        }

        public static void Render(IFace2[] faces, ISldWorks app, Color color, float lineWidth)
        {
            if (faces.Length == 0) return;

            DoSetup(app);

            //GL.ShadeModel(ShadingModel.Flat);
            using (SetColor(color))
            using (SetLineWidth(lineWidth))
            {
                faces
                    .ForEach(face =>
                    {
                        var strips = FaceTriStrips.Unpack(face.GetTessTriStrips(true).CastArray<float>());
                        var norms = FaceTriStrips.Unpack(face.GetTessTriStripNorms().CastArray<float>());
                        if (strips == null || norms == null)
                            return;
                        Debug.Assert(norms.Length == strips.Length);
                        Debug.Assert(norms.Zip(strips, (a, b) => a.Length == b.Length).All(x => x));
                        norms.Zip(strips, (normStrip, pointStrip) => normStrip.Zip(pointStrip, (norm, point) => new { norm, point }))
                        .ForEach(strip =>
                        {
                            using (Begin(PrimitiveType.TriangleStrip))
                            {
                                foreach (var vertex in strip)
                                {
                                    Debug.Assert(vertex.point.Length == 3);
                                    GL.Vertex3(vertex.point);
                                    Debug.Assert(vertex.norm.Length == 3);
                                    GL.Normal3(vertex.norm);
                                }
                            }
                        });
                    });

            }
        }

        private static void DoSetup(ISldWorks app)
        {
            if (!_Setup)
            {
                _Setup = true;
                var modelDoc = (IModelDoc2) app.ActiveDoc;
                ////modelDoc.ViewOglShading();
                var view = (IModelView) modelDoc.ActiveView;
                ////view.InitializeShading();
                //var windowHandle = (IntPtr) view.GetViewHWndx64();
                view.UpdateAllGraphicsLayers = true;
                view.InitializeShading();
                //Toolkit.Init();
                //var windowInfo = Utilities.CreateWindowsWindowInfo(windowHandle);
                ////var context = new GraphicsContext(GraphicsMode.Default, windowInfo);
                //var contextHandle = new ContextHandle(windowHandle);
                //var context = new GraphicsContext(contextHandle, Wgl.GetProcAddress, () => contextHandle);
                //context.MakeCurrent(windowInfo);
                //context.LoadAll();
                new GLControl().CreateGraphics();
                //Toolkit.Init();
                //IGraphicsContext context = new GraphicsContext(
                //    new ContextHandle(windowHandle),null );
                //context.LoadAll();
            }
        }
    }

    public class FaceTriStrips
    {
        public float[][][] Data { get; }
        public static float[][][] Unpack(float[] packedData)
        {
            if (packedData == null || packedData.Length == 0)
                return null;
            var numStrips = (int)ToUnit32(packedData, 0);
            var vertexPerStrip = Enumerable.Range(1, numStrips)
                .Select(i => ToUnit32(packedData, i))
                .ToArray();
            var bufferIdex = vertexPerStrip
                .Scan(
                    Tuple.Create(0U, 0U),
                    (a, next) => Tuple.Create(a.Item1 + a.Item2, next) )
                    .ToList();

           var r = 
                bufferIdex
                    .Select(idx =>
                        Enumerable.Range((int) idx.Item1, (int) idx.Item2)
                            .Select(i => 1 + vertexPerStrip.Length + i*3)
                            .Select(i => new[] { packedData[i], packedData[i + 1], packedData[i + 2]}).ToArray()).ToArray();

            Debug.Assert(r.Length==numStrips);

            return r;



        }

        private static uint ToUnit32(float[] data, int i)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(data[i]), 0);
        }
    }

    public static class FaceExtensions
    {
        /// <summary>
        /// Returns a list of triangles. A triangle is a list of points
        /// </summary>
        /// <param name="face"></param>
        /// <param name="noConversion"></param>
        /// <returns></returns>
        private static List<List<IList<float>>> GetTessTrianglesTs(this IFace2 face, bool noConversion)
        {
            var data = (float[])face.GetTessTriangles(noConversion);
            return data
                .Buffer(9,9)
                .Select(v=>v.Buffer(3,3).ToList())
                .ToList();
        }

        public static List<List<IList<float>>> GetTessTrianglesNoConversion(this IFace2 face) => face.GetTessTrianglesTs(true);
        public static List<List<IList<float>>> GetTessTrianglesAllowConversion(this IFace2 face) => face.GetTessTrianglesTs(false);
    }
}
