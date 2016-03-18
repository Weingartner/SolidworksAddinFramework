using System.Drawing;
using SolidworksAddinFramework.OpenGl;

namespace SolidworksAddinFramework
{
    public class ColorRenderable
    {
        public ColorRenderable(IRenderable renderable, Color color)
        {
            Renderable = renderable;
            Color = color;
        }

        public IRenderable Renderable { get; }
        public Color Color { get; }

        public void Render()
        {
            Renderable.Render(Color);
        }
    }
}