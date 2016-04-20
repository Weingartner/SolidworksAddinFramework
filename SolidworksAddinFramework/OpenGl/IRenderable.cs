using System;
using System.Numerics;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IRenderable
    {
        void Render(DateTime time);
        void ApplyTransform(Matrix4x4 transform);
    }
}