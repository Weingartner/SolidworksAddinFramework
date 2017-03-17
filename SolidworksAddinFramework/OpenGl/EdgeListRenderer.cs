using System;
using System.Collections.Generic;
using System.DoubleNumerics;
using System.Drawing;
using System.Linq;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.OpenGl
{
    public class EdgeListRenderer : RendererBase<IReadOnlyList<Edge3>>
    {
        private readonly Color _Color;

        public EdgeListRenderer(IEnumerable<Edge3> edges, Color color):base(edges.ToList())
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

        protected override void DoRender(IReadOnlyList<Edge3> data, DateTime time, double opacity, bool visbility, IDrawContext drawContext)
        {
            if (!visbility)
                return;

            drawContext.DrawEdges(data, opacity, _Color);
        }


        protected override Tuple<Vector3, double> UpdateBoundingSphere(IReadOnlyList<Edge3> data, DateTime time)
        {
            throw new NotImplementedException();
        }
    }
}