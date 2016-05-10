using System;
using System.Drawing;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public class Point : IRenderable
    {
        private readonly Color _Color;

        public Point(Vector3 location, Color? color)
        {
            _Color = color ?? Color.Blue;
            Location = location;
        }

        private Vector3 Location { get; }

        public void Render(DateTime time)
        {
            GL.PointSize(5);
            using (ModernOpenGl.Begin(PrimitiveType.Points))
            using(ModernOpenGl.SetColor(_Color, ShadingModel.Flat))
            {
                Location.GLVertex3();
            }

        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            throw new NotImplementedException();
        }
    }

    public static class PointExtensions
    {
        public static IDisposable DisplayUndoable(this Vector3 p, IModelDoc2 doc, Color? color=null)
        {
            return new Point(p, color).DisplayUndoable(doc);
        }
    }
}