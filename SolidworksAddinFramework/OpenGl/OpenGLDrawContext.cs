using System.Collections.Generic;
using System.DoubleNumerics;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Drawing;

namespace SolidworksAddinFramework.OpenGl
{
    public class OpenGLDrawContext : IDrawContext
    {
        private Color FromArgb(double opacity, Color baseColor)
        {
            return Color.FromArgb((int)(opacity*255), baseColor);
        }

        public void DrawLine(Color fromArgb, double thickness, IReadOnlyList<Vector3> data, double opacity, bool closed)
        {
            using (ModernOpenGl.SetLineWidth(thickness))
            {
                using (ModernOpenGl.SetColor(fromArgb, ShadingModel.Smooth, solidBody: false))
                using (ModernOpenGl.Begin( closed ? PrimitiveType.LineLoop :  PrimitiveType.LineStrip))
                {
                    data.ForEach(p => p.GLVertex3());
                }
            }
        }

        public void DrawEdges(IReadOnlyList<Edge3> data, double opacity, Color color)
        {
            using (ModernOpenGl.SetColor(FromArgb(opacity, color), ShadingModel.Smooth, solidBody: false))
            using (ModernOpenGl.SetLineWidth(2.0))
            using (ModernOpenGl.Begin(PrimitiveType.Lines))
                foreach (var v in data)
                {
                    v.A.GLVertex3();
                    v.B.GLVertex3();
                }
        }
        public void DrawMesh(MeshData data, double opacity, Color colorr, bool isSolid)
        {
            var color = FromArgb(opacity, colorr);
            MeshRender.Render(data, color, isSolid);
        }

        public void DrawPoint(Color color, int size, Vector3 location)
        {
            using (ModernOpenGl.SetColor(color, ShadingModel.Flat, solidBody: false))
            {
                GL.PointSize(size);
                GL.Enable(EnableCap.AlphaTest);
                GL.AlphaFunc(AlphaFunction.Notequal, 0);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Enable(EnableCap.PointSmooth);
                GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
                GL.Begin(PrimitiveType.Points);
                location.GLVertex3();
                GL.End();

                GL.Disable(EnableCap.PointSmooth);
                GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.Zero);
                GL.Disable(EnableCap.Blend);
            }
        }
    }
}