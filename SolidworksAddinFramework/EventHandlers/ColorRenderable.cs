using System.Drawing;
using SolidworksAddinFramework.OpenGl;

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