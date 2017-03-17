using System.Collections.Generic;
using System.Collections.Immutable;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public static class AnimationExtensions
    {
        
        public static Animator CreateAnimator
            (this ImmutableList<IAnimationSection> animationSections,
            ImmutableList<IRenderer> children)
        {
            return new Animator(animationSections, children);

        }
        public static Animator CreateAnimator
            (this ImmutableList<IAnimationSection> animationSections,
            IRenderer child)
        {
            return animationSections.CreateAnimator(ImmutableList.Create(child));

        }

        public static MultiAnimator ConcatAnimators(this IEnumerable<AnimatorBase> animators)
        {
            return new MultiAnimator(animators.ToImmutableList());
        }
    }
}