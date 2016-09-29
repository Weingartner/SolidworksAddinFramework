using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.Geometry;

namespace SolidworksAddinFramework.OpenGl
{
    public class EdgeListRenderable : RenderableBase<IReadOnlyList<Edge3>>
    {
        private readonly Color _Color;

        public EdgeListRenderable(IEnumerable<Edge3> edges, Color color):base(edges.ToList())
        {
            _Color = color;
        }

        protected override IReadOnlyList<Edge3> DoTransform(IReadOnlyList<Edge3> data, Matrix4x4 transform)
        {
            var list = new Edge3[data.Count];

            list.Length
                .ParallelChunked((lower, upper) =>
                {
                    for (int i = lower; i < upper; i++)
                    {
                        list[i] = data[i].ApplyTransform(transform);
                    }
                });
            return list;
        }

        protected override void DoRender(IReadOnlyList<Edge3> data, DateTime time)
        {
            using (ModernOpenGl.SetColor(_Color, ShadingModel.Smooth, solidBody: false))
            using (ModernOpenGl.SetLineWidth(2.0))
            using (ModernOpenGl.Begin(PrimitiveType.Lines))
                foreach (var v in data)
                {
                    v.A.GLVertex3();
                    v.B.GLVertex3();
                }
        }

        protected override Tuple<Vector3, double> UpdateBoundingSphere(IReadOnlyList<Edge3> data, DateTime time)
        {
            throw new NotImplementedException();
        }
    }
}