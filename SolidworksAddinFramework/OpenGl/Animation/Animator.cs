using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.DoubleNumerics;
using System.Linq;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class Animator : AnimatorBase
    {
        public override TimeSpan Duration => Sections.Aggregate(TimeSpan.Zero, (sum, s) => sum + s.Duration);
        private IReadOnlyList<SectionTime> _SectionTimes;

        public override ImmutableList<IAnimationSection> Sections { get; }
        private readonly ImmutableList<IRenderer> _Children;

        public Animator(ImmutableList<IAnimationSection> sections, ImmutableList<IRenderer> children)
        {
            Sections = sections;
            _Children = children;
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

        public override void Render(DateTime t, Matrix4x4? renderTransform = null)
        {
            if (_SectionTimes == null)
                return;

            var currentSection = _SectionTimes.FirstOrDefault(o => o.EndTime > t) ?? _SectionTimes.Last();

            var startTime = currentSection.EndTime - currentSection.Section.Duration;

            var currentTransform = currentSection.Section.Transform(t - startTime);

            foreach (var child in _Children)
            {
                child.ApplyTransform(currentTransform * (renderTransform ?? Matrix4x4.Identity));
                child.Render(t);
            }
        }
    }
}