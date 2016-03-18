using System.Drawing;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IRenderable
    {
        void Render(Color color);
        void ApplyTransform(IMathTransform transform);
    }
}