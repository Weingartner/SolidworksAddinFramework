using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class MultiAnimator : IRenderable
    {
        private readonly Func<double> _GetCurrentValue;
        private readonly IReadOnlyList<Animator> _Animators;

        public DateTime ReferenceTime { get; }

        public TimeSpan FullAnimationTimeSpan { get; }

        public IReadOnlyList<SectionTime> SectionTimes => _Animators.SelectMany(a => a.SectionTimes).ToList();

        public MultiAnimator([NotNull] IEnumerable<Animator> animators, [NotNull] Func<double> getCurrentValue)
        {
            if (animators == null) throw new ArgumentNullException(nameof(animators));
            if (getCurrentValue == null) throw new ArgumentNullException(nameof(getCurrentValue));

            _GetCurrentValue = getCurrentValue;
            _Animators = animators.ToList();
            ReferenceTime = DateTime.Now;
            if (_Animators.Count > 0)
            {
                _Animators.Buffer(2, 1)
                    .Where(b => b.Count == 2)
                    .Select(b => new {StartTime = b[0].SectionTimes.Last().EndTime, Animator = b[1]})
                    .StartWith(new {StartTime = ReferenceTime, Animator = _Animators.First()})
                    .ForEach(p =>
                    {
                        p.Animator.CalculateSectionTimes(p.StartTime);
                    });
                var endTime = _Animators.LastOrDefault(a => a.SectionTimes.Any())?.SectionTimes.Last().EndTime ?? ReferenceTime;
                FullAnimationTimeSpan = endTime - ReferenceTime;
            }
        }

        public void Render(DateTime _)
        {
            var value = _GetCurrentValue();
            var time = ReferenceTime + TimeSpan.FromMilliseconds(FullAnimationTimeSpan.TotalMilliseconds * value);
            try
            {
                var animator = _Animators
                    .Where(a => a.SectionTimes.Any())
                    .First(a => a.SectionTimes.Last()?.EndTime >= time);
                animator.Render(time);
            }
            catch (Exception e)
            {
            }
        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            throw new NotImplementedException();
        }

        public Tuple<Vector3, float> BoundingSphere
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}