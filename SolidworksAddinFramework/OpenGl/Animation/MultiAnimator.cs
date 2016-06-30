using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using JetBrains.Annotations;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class MultiAnimator : IRenderable
    {
        private readonly Func<TimeSpan, double> _GetCurrentValue;
        private readonly IReadOnlyList<Animator> _Animators;

        public DateTime ReferenceTime { get; }

        public TimeSpan FullAnimationTimeSpan { get; }

        public IReadOnlyList<SectionTime> SectionTimes => _Animators.SelectMany(a => a.SectionTimes).ToList();

        public MultiAnimator([NotNull] IEnumerable<Animator> animators, Func<TimeSpan, double> getCurrentValue = null)
        {
            if (animators == null) throw new ArgumentNullException(nameof(animators));
            _GetCurrentValue = getCurrentValue ??
                (t => (t.TotalMilliseconds % FullAnimationTimeSpan.TotalMilliseconds) / FullAnimationTimeSpan.TotalMilliseconds);

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

        public IDisposable DisplayUndoable(IModelDoc2 modelDoc, int layer = 0)
        {
            var d = new CompositeDisposable();
            OpenGlRenderer.DisplayUndoable(this, modelDoc, layer).DisposeWith(d);
            Animator.Redraw(modelDoc).DisposeWith(d);
            return d;
        }

        public void Render(DateTime now)
        {
            var value = _GetCurrentValue(now - ReferenceTime);
            var time = ReferenceTime + TimeSpan.FromMilliseconds(FullAnimationTimeSpan.TotalMilliseconds * value);
            var animator = _Animators
                .Where(a => a.SectionTimes.Any())
                .First(a => a.SectionTimes.Last()?.EndTime >= time);
            animator.Render(time);
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