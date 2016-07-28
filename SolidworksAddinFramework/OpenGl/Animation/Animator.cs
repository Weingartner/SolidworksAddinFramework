using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Reactive;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class Animator : AnimatorBase
    {
        public override TimeSpan Duration => Sections.Aggregate(TimeSpan.Zero, (sum, s) => sum + s.Duration);
        private IReadOnlyList<SectionTime> _SectionTimes;

        public override ImmutableList<IAnimationSection> Sections { get; }
        private readonly ImmutableList<IRenderable> _Children;
        private readonly IObserver<AnimationData> _RenderObserver;

        public Animator(ImmutableList<IAnimationSection> sections, ImmutableList<IRenderable> children, IObserver<AnimationData> renderObserver)
        {
            Sections = sections;
            _Children = children;
            _RenderObserver = renderObserver ?? Observer.Create<AnimationData>(_ => {});
        }

        public override void OnStart(DateTime startTime)
        {
            if (_SectionTimes != null)
                throw new Exception("You have allready added this animation");
            _SectionTimes = Calculate(Sections, startTime);
        }

        private static List<SectionTime> Calculate(IReadOnlyList<IAnimationSection> animationSections, DateTime startTime)
        {
            return animationSections
                .Scan(new SectionTime(null, startTime), (acc, section) => new SectionTime(section, acc.EndTime + section.Duration))
                .Skip(1)
                .ToList();
        }

        public override void Render(DateTime t)
        {
            if (_SectionTimes == null)
                return;

            var currentSection = _SectionTimes.FirstOrDefault(o => o.EndTime > t) ?? _SectionTimes.Last();

            var startTime = currentSection.EndTime - currentSection.Section.Duration;

            var currentTransform = currentSection.Section.Transform(t - startTime);

            currentSection.Section.Notify(t-startTime);

            foreach (var child in _Children)
            {
                child.ApplyTransform(currentTransform);
                child.Render(t);
            }

            _RenderObserver.OnNext(new AnimationData(currentSection.Section));
        }
    }
}