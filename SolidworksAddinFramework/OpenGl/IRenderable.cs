using System;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;
using MGL = SolidworksAddinFramework.OpenGl.ModernOpenGl;
using OpenTK.Graphics.OpenGL;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IRenderable
    {
        void Render(DateTime time);
        void ApplyTransform(Matrix4x4 transform);
    }

    public class Point : IRenderable
    {
        public Point(Vector3 location)
        {
            Location = location;
        }

        private Vector3 Location { get; }

        public void Render(DateTime time)
        {
            using (MGL.Begin(PrimitiveType.Points))
            {
                Location.GLVertex3();
            }

        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            throw new NotImplementedException();
        }
    }
}