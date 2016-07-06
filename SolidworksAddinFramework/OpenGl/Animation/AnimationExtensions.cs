using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public static class AnimationExtensions
    {
        
        public static Animator CreateAnimator
            (this ImmutableList<IAnimationSection> animationSections,
            ImmutableList<IRenderable> children,
            IObserver<AnimationData> renderObserver = null)
        {
            return new Animator(animationSections, children, renderObserver);

        }
        public static Animator CreateAnimator
            (this ImmutableList<IAnimationSection> animationSections,
            IRenderable child,
            IObserver<AnimationData> renderObserver = null)
        {
            return animationSections.CreateAnimator(ImmutableList.Create(child), renderObserver);

        }

        public static MultiAnimator ConcatAnimators(this IEnumerable<AnimatorBase> animators, Func<TimeSpan, double> getCurrentValue = null)
        {
            return new MultiAnimator(animators.ToImmutableList(), getCurrentValue);
        }
    }
}