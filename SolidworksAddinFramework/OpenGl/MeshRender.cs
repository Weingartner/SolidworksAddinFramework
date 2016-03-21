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
    public static class MeshRender
    {
        public static void Render(Mesh mesh, Color color)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            using (ModernOpenGl.SetColor(color))
            using (ModernOpenGl.SetLineWidth(2.0f))
            using (ModernOpenGl.Begin(PrimitiveType.Triangles))
            {
                var tris = mesh.TriangleVertices;
                foreach (var tri in tris)
                {
                    GL.Normal3(tri.Item2);
                    GL.Vertex3(tri.Item1);
                }
            }
        }

        public static void Render(IFace2[] faces, Color color, float lineWidth)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            using (ModernOpenGl.SetColor(color))
            using (ModernOpenGl.SetLineWidth(lineWidth))
            {
                faces
                    .ForEach(face =>
                    {
                        var strips = FaceTriStrips.Unpack(face.GetTessTriStrips(true).CastArray<float>());
                        var norms = FaceTriStrips.Unpack(face.GetTessTriStripNorms().CastArray<float>());
                        Debug.Assert(norms.Length == strips.Length);
                        Debug.Assert(norms.Zip(strips, (a, b) => a.Length == b.Length).All(x => x));
                        norms.Zip(strips, (normStrip, pointStrip) => normStrip.Zip(pointStrip, (norm, point) => new { norm, point }))
                        .ForEach(strip =>
                        {
                            using (ModernOpenGl.Begin(PrimitiveType.TriangleStrip))
                            {
                                foreach (var vertex in strip)
                                {
                                    GL.Normal3(vertex.norm);
                                    GL.Vertex3(vertex.point);
                                }
                            }
                        });
                    });

            }
        }
    }
}
