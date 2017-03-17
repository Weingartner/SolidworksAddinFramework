using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Math;

namespace SolidworksAddinFramework.OpenGl
{
    public static class MeshRender
    {
        public static void Render(MeshData mesh, Color color, bool isSolid)
        {
            using (ModernOpenGl.SetColor(color, ShadingModel.Smooth, solidBody:isSolid))
            using (ModernOpenGl.SetLineWidth(2.0))
            using (ModernOpenGl.Begin(PrimitiveType.Triangles))
            {
                var tris = mesh.Triangles;
                foreach (var tri in tris)
                {
                    tri.GLVertexAndNormal();
                }
            }
            if (mesh.Edges == null) return;
            using (ModernOpenGl.SetColor(Color.Blue, ShadingModel.Smooth, solidBody:isSolid))
            using (ModernOpenGl.SetLineWidth(2.0))
            using (ModernOpenGl.Begin(PrimitiveType.Lines))
                foreach (var v in mesh.Edges)
                {
                    v.A.GLVertex3();
                    v.B.GLVertex3();
                }
        }
    }
}
