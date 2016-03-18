using System;
using Reactive.Bindings;
using SolidworksAddinFramework.ReactiveProperty;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// This is a wrapper for IBody2 that can transform
    /// the tesselation to improve animation performance.
    /// </summary>
    public class TessellatableBody
    {
        public double Radius { get; }
        public double Length { get; }

        public IBody2 Body { get; private set; }

        public IBody2 _OriginalBody{ get; }

        public Mesh Tesselation { get; private set; }

        public ReactiveProperty<IMathTransform> Transform { get; }

        public TessellatableBody(IMathUtility math, IBody2 body)
        {
            Body = body;
            _OriginalBody = body;
            Transform = new ReactiveProperty<IMathTransform>(math.IdentityTransform());

            Tesselation = new Mesh(body);

            Transform
                .WhenAnyValue()
                .Subscribe(UpdateTransform);
        }

        private void UpdateTransform(IMathTransform transform)
        {
            Body = (IBody2)_OriginalBody.Copy();
            Body.ApplyTransform((MathTransform)transform);
            Tesselation.ApplyTransform((MathTransform)transform);
        }
    }
}
