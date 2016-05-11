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
        private readonly int _Size;
        private bool _InFront;

        public Point(Vector3 location, Color color, int size, bool inFront=false)
        {
            Location = location;
            _Color = color;
            _Size = size;
            _InFront = inFront;
        }

        private Vector3 Location { get; }

        public void Render(DateTime time)
        {
            //using(ModernOpenGl.SetColor(_Color, ShadingModel.Smooth))
            //using (ModernOpenGl.Begin(PrimitiveType.Points))
            //{
            //    Location.GLVertex3();
            //}

            //GL.Clear(ClearBufferMask.ColorBufferBit);
            using (ModernOpenGl.SetColor(_Color, ShadingModel.Flat))
            {
                GL.PointSize(_Size);
                GL.Enable(EnableCap.AlphaTest);
                GL.AlphaFunc(AlphaFunction.Notequal, 0);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Enable(EnableCap.PointSmooth);
                GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
                GL.Begin(PrimitiveType.Points);
                Location.GLVertex3();
                GL.End();

                GL.Disable(EnableCap.PointSmooth);
                GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.Zero);
                GL.Disable(EnableCap.Blend);
            }
        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            throw new NotImplementedException();
        }
    }

    public static class PointExtensions
    {
        public static IDisposable DisplayUndoable(this Vector3 p, IModelDoc2 doc, Color color, int size, int layer = 0)
        {
            return new Point(p, color, size).DisplayUndoable(doc, layer);
        }

    }
}