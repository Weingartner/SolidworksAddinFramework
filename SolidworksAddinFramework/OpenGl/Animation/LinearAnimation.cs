using System;
using System.Diagnostics;
using System.Numerics;
using ReactiveUI;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class LinearAnimation<T> : ReactiveObject, IAnimationSection
        where T : IInterpolatable<T>
    {
        private readonly IMathUtility _Math;
        public T From { get; } 
        public T To { get; } 
        public TimeSpan Duration { get; }

        public LinearAnimation
            (TimeSpan duration, T @from, T to, IMathUtility math)  
        {
            Duration = duration;
            From = @from;
            To = to;
            _Math = math;
        }

        public Matrix4x4 Transform( TimeSpan deltaTime)
        {
            var beta = deltaTime.TotalMilliseconds/Duration.TotalMilliseconds;

            return BlendTransform(beta);
        }

        public Matrix4x4 BlendTransform(double beta)
        {
            Debug.Assert(beta>=0 && beta<=1);
            return From.Interpolate(To, beta).Transform();
        }
    }
}