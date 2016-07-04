using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class Animator : AnimatorBase
    {
        public override TimeSpan Duration => Sections.Aggregate(TimeSpan.Zero, (sum, s) => sum + s.Duration);
        private IReadOnlyList<SectionTime> _SectionTimes;

        public override IReadOnlyList<IAnimationSection> Sections { get; }
        private readonly IReadOnlyList<IRenderable> _Children;
        private readonly IObserver<Matrix4x4> _RenderObserver;

        public Animator(IReadOnlyList<IAnimationSection> sections, IReadOnlyList<IRenderable> children, IObserver<Matrix4x4> renderObserver)
        {
            Sections = sections;
            _Children = children;
            _RenderObserver = renderObserver ?? Observer.Create<Matrix4x4>(_ => {});
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
            // ReSharper disable once UseNullPropagation
            if (_SectionTimes == null)
                return;

            var currentSection = _SectionTimes.FirstOrDefault(o => o.EndTime >= t);
            if (currentSection == null)
                return;

            var startTime = currentSection.EndTime - currentSection.Section.Duration;

            var currentTransform = currentSection.Section.Transform(t - startTime);

            foreach (var child in _Children)
            {
                child.ApplyTransform(currentTransform);
                child.Render(t);
            }

            _RenderObserver.OnNext(currentTransform);
        }
    }
}