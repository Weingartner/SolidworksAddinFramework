using System;
using System.Drawing;
using Reactive.Bindings;
using SolidworksAddinFramework.ReactiveProperty;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    /// <summary>
    /// This is a wrapper for IBody2 that can transform
    /// the tesselation to improve animation performance.
    /// </summary>
    public class TessellatableBody : IRenderable 
    {
        public IRenderable Tesselation { get; private set; }


        public TessellatableBody(IMathUtility math, IBody2 body)
        {
            Tesselation = new Mesh(body);
        }

        private void UpdateTransform(IMathTransform transform)
        {
            Tesselation.ApplyTransform((MathTransform)transform);
        }

        public void Render(Color color)
        {
            Tesselation.Render(color);
        }

        public void ApplyTransform(IMathTransform transform)
        {
            UpdateTransform(transform);
        }
    }
}
