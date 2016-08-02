using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public static class AnimationExtensions
    {
        
        public static Animator CreateAnimator
            (this ImmutableList<IAnimationSection> animationSections,
            ImmutableList<IRenderable> children)
        {
            return new Animator(animationSections, children);

        }
        public static Animator CreateAnimator
            (this ImmutableList<IAnimationSection> animationSections,
            IRenderable child)
        {
            return animationSections.CreateAnimator(ImmutableList.Create(child));

        }

        public static MultiAnimator ConcatAnimators(this IEnumerable<AnimatorBase> animators)
        {
            return new MultiAnimator(animators.ToImmutableList());
        }
    }
}