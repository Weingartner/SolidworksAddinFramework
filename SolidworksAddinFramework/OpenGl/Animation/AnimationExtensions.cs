using System;
using System.Collections.Generic;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public static class AnimationExtensions
    {
        
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IReadOnlyList<IRenderable> children,
            int framerate = 30)
        {
            return new Animator(animationSections, children, framerate);

        }
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IRenderable children,
            int framerate = 30)
        {
            return new Animator(animationSections, new [] { children}, framerate);

        }

        public static MultiAnimator ConcatAnimators(this IEnumerable<Animator> animators, Func<TimeSpan, double> getCurrentValue = null)
        {
            return new MultiAnimator(animators, getCurrentValue);
        }
    }
}