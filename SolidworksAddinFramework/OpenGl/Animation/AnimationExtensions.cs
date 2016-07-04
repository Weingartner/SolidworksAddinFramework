using System;
using System.Collections.Generic;
using System.Numerics;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public static class AnimationExtensions
    {
        
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IReadOnlyList<IRenderable> children,
            IObserver<Matrix4x4> renderObserver = null)
        {
            return new Animator(animationSections, children, renderObserver);

        }
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IRenderable child,
            IObserver<Matrix4x4> renderObserver = null)
        {
            return animationSections.CreateAnimator(new[] { child }, renderObserver);

        }

        public static MultiAnimator ConcatAnimators(this IEnumerable<Animator> animators, Func<TimeSpan, double> getCurrentValue = null)
        {
            return new MultiAnimator(animators, getCurrentValue);
        }
    }
}