using System;
using System.Collections.Immutable;
using System.DoubleNumerics;
using System.Linq;
using JetBrains.Annotations;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public sealed class MultiAnimator : AnimatorBase
    {
        private readonly ImmutableList<AnimatorBase> _Animators;
        private DateTime _ReferenceTime;

        public override TimeSpan Duration => _Animators.Aggregate(TimeSpan.Zero, (sum, a) => sum + a.Duration);
        public override ImmutableList<IAnimationSection> Sections => _Animators.SelectMany(a => a.Sections).ToImmutableList();

        public MultiAnimator([NotNull] ImmutableList<AnimatorBase> animators)
        {
            if (animators == null) throw new ArgumentNullException(nameof(animators));

            _Animators = animators;
        }

        public override void OnStart(DateTime startTime)
        {
            if (_Animators.Count == 0) return;

            _ReferenceTime = startTime;
            var animatorStartTime = _ReferenceTime;
            foreach (var animator in _Animators)
            {
                animator.OnStart(animatorStartTime);
                animatorStartTime += animator.Duration;
            }
        }

        public override void Render(DateTime now, Matrix4x4? renderTransform = null)
        {
            if (_Animators.Count == 0) return;

            var passedTime = now - _ReferenceTime;
            var value = (passedTime.TotalMilliseconds % Duration.TotalMilliseconds) / Duration.TotalMilliseconds;
            var time = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds * value);

            FindAnimator(time).Render(_ReferenceTime + time, renderTransform);
        }

        private AnimatorBase FindAnimator(TimeSpan time)
        {
            var duration = TimeSpan.Zero;
            foreach (var animator in _Animators)
            {
                duration += animator.Duration;
                if (duration > time)
                {
                    return animator;
                }
            }
            return _Animators.Last();
        }
    }
}