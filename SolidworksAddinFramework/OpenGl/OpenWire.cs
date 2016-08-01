using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public abstract class Wire : RenderableBase
    {
        private IReadOnlyList<Vector3> _Points;
        private readonly PrimitiveType _Mode;
        public float Thickness { get; set; }

        public Color Color { get; set; }

        protected Wire(IEnumerable<Vector3> points, float thickness, PrimitiveType mode, Color color)
        {
            _Points = points.ToList();
            Thickness = thickness;
            _Mode = mode;
            Color = color;
            UpdateBoundingSphere(_Points);
        }

        public override void Render(DateTime time)
        {
            using (ModernOpenGl.SetLineWidth(Thickness))
            using (ModernOpenGl.SetColor(Color, ShadingModel.Smooth, solidBody:false))
            using (ModernOpenGl.Begin(_Mode))
            {
                _Points.ForEach(p=>p.GLVertex3());
            }
        }

        public override void ApplyTransform(Matrix4x4 transform)
        {
            _Points = _Points
                .Select(p => Vector3.Transform(p,transform))
                .ToList();
            UpdateBoundingSphere(_Points);
        }
    }

    public class OpenWire : Wire
    {
        public OpenWire(IEnumerable<Vector3> points, float thickness, Color color)
            : base(points, thickness, PrimitiveType.LineStrip, color)
        { }

        public OpenWire(ICurve curve, double thickness, Color color, double chordTol=1e-6, double lengthTol = 0) : this(curve.GetTessPoints(chordTol, lengthTol), (float)thickness, color)
        {
            
        }

        public OpenWire(IEnumerable<double[]> points, float thickness, Color color) : this(points.Select(p=>p.ToVector3D()), thickness, color)
        { }
    }

    public class ClosedWire : Wire
    {
        public ClosedWire(IEnumerable<Vector3> points, float thickness, Color color)
            : base(points, thickness, PrimitiveType.LineLoop, color)
        { }
    }
}