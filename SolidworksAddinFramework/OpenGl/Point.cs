using System;
using System.Drawing;
using System.DoubleNumerics;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.OpenGl
{
    public class Point : RendererBase<Vector3>
    {
        private readonly Color _Color;
        private readonly int _Size;

        public Point(Vector3 location, Color color, int size, bool inFront=false):base(location)
        {
            _Color = color;
            _Size = size;
        }


        protected override Vector3 DoTransform(Vector3 data, Matrix4x4 transform)
        {
            return Vector3.Transform(data,transform);
        }

        protected override void DoRender(Vector3 data, DateTime time, double opacity, bool visibile, IDrawContext drawContext)
        {
            if (!visibile)
                return;
            drawContext.DrawPoint(FromArgb(opacity,_Color), _Size - 3, data);
            drawContext.DrawPoint(Color.Black, _Size, data);
        }

        protected override Tuple<Vector3, double> UpdateBoundingSphere(Vector3 data, DateTime time)
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