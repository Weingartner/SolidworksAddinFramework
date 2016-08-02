using System;
using System.Diagnostics;
using System.Numerics;
using ReactiveUI;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class LinearAnimation<T> : ReactiveObject, IAnimationSection
        where T : IInterpolatable<T>
    {
        public T From { get; } 
        public T To { get; } 
        public TimeSpan Duration { get; }

        public LinearAnimation(TimeSpan duration, T from, T to)
        {
            Duration = duration;
            From = from;
            To = to;
        }

        public Matrix4x4 Transform( TimeSpan deltaTime)
        {
            var beta = DeltaTimeToBlend(deltaTime);

            return BlendTransform(beta);
        }

        private double DeltaTimeToBlend(TimeSpan deltaTime)
        {
            return deltaTime.TotalMilliseconds/Duration.TotalMilliseconds;
        }

        public Matrix4x4 BlendTransform(double beta)
        {
            Debug.Assert(beta>=0 && beta<=1);
            return From.Interpolate(To, beta).Transform();
        }
    }

    public static class LinearAnimation
    {
        public static LinearAnimation<T> Create<T>(TimeSpan duration, T from, T to)
            where T : IInterpolatable<T>
        {
            return new LinearAnimation<T>(duration, from, to);
        } 
    }
}