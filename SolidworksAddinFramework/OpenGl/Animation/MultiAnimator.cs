using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public sealed class MultiAnimator : AnimatorBase
    {
        private readonly Func<TimeSpan, double> _GetCurrentValue;
        private readonly ImmutableList<AnimatorBase> _Animators;
        private DateTime _ReferenceTime;

        public override TimeSpan Duration => _Animators.Aggregate(TimeSpan.Zero, (sum, a) => sum + a.Duration);
        public override ImmutableList<IAnimationSection> Sections => _Animators.SelectMany(a => a.Sections).ToImmutableList();

        public MultiAnimator([NotNull] ImmutableList<AnimatorBase> animators, Func<TimeSpan, double> getCurrentValue = null)
        {
            if (animators == null) throw new ArgumentNullException(nameof(animators));

            _GetCurrentValue = getCurrentValue ??
                (t => (t.TotalMilliseconds % Duration.TotalMilliseconds) / Duration.TotalMilliseconds);

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

        public override void Render(DateTime now)
        {
            if (_Animators.Count == 0) return;

            var value = _GetCurrentValue(now - _ReferenceTime);
            var time = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds * value);

            FindAnimator(time).Render(_ReferenceTime + time);
        }

        private AnimatorBase FindAnimator(TimeSpan time)
        {
            var duration = TimeSpan.Zero;
            foreach (var animator in _Animators)
            {
                duration += animator.Duration;
                if (duration >= time)
                {
                    return animator;
                }
            }
            throw new IndexOutOfRangeException("Can't find animator.");
        }
    }
}