using System.Drawing;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Drawing;

namespace SolidworksAddinFramework
{
    public class ColorRenderable
    {
        public ColorRenderable(IRenderer renderer, Color color)
        {
            Renderer = renderer;
            Color = color;
        }

        public IRenderer Renderer { get; }
        public Color Color { get; }

    }
}