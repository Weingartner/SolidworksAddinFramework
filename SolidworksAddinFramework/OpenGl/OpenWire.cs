using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK.Graphics.OpenGL;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public abstract class Wire : IRenderable
    {
        private IList<Vector3> _Points;
        private readonly float _Thickness;
        private readonly PrimitiveType _Mode;

        protected Wire(IEnumerable<Vector3> points, float thickness, PrimitiveType mode)
        {
            _Points = points.ToList();
            _Thickness = thickness;
            _Mode = mode;
        }

        public void Render(DateTime time)
        {
            using (ModernOpenGl.Begin(_Mode))
            using (ModernOpenGl.SetColor(this.Color, ShadingModel.Smooth))
            using (ModernOpenGl.SetLineWidth(_Thickness))
            {
                _Points.ForEach(p=>p.GLVertex3());
            }
        }

        public Color Color { get; set; } = System.Drawing.Color.Blue;

        public void ApplyTransform(Matrix4x4 transform)
        {
            _Points = _Points
                .Select(p => Vector3.Transform(p,transform))
                .ToList();
        }
    }

    public class OpenWire : Wire
    {
        public OpenWire(IEnumerable<Vector3> points, float thickness)
            : base(points, thickness, PrimitiveType.LineStrip)
        { }
        public OpenWire(IEnumerable<double[]> points, float thickness) : this(points.Select(p=>p.ToVector3D()), thickness)
        { }
    }

    public class ClosedWire : Wire
    {
        public ClosedWire(IEnumerable<Vector3> points, float thickness)
            : base(points, thickness, PrimitiveType.LineLoop)
        { }
    }
}