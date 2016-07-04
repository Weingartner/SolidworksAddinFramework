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
            Action<Matrix4x4> onRender = null)
        {
            return new Animator(animationSections, children, onRender);

        }
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IRenderable child,
            Action<Matrix4x4> onRender = null)
        {
            return animationSections.CreateAnimator(new[] { child }, onRender);

        }

        public static MultiAnimator ConcatAnimators(this IEnumerable<Animator> animators, Func<TimeSpan, double> getCurrentValue = null)
        {
            return new MultiAnimator(animators, getCurrentValue);
        }
    }
}