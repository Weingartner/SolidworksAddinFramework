using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using Weingartner.Exceptional.Reactive;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public class Animator : AnimatorBase
    {
        private IReadOnlyList<SectionTime> _SectionTimes;
        public override TimeSpan Duration => Sections.Aggregate(TimeSpan.Zero, (sum, s) => sum + s.Duration);
        public IReadOnlyList<SectionTime> SectionTimes => _SectionTimes;
        public override IReadOnlyList<IAnimationSection> Sections { get; }
        private readonly IReadOnlyList<IRenderable> _Children;

        public Task CompletionTask { get; set; }

        public Animator(IReadOnlyList<IAnimationSection> sections, IReadOnlyList<IRenderable> children)
        {
            Sections = sections;
            _Children = children;
        }

        public override void CalculateSectionTimes(DateTime startTime)
        {
            if (SectionTimes != null)
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
            if (SectionTimes == null)
                return;

            var currentSection = SectionTimes.FirstOrDefault(o => o.EndTime >= t);
            if (currentSection == null)
                return;

            var startTime = currentSection.EndTime - currentSection.Section.Duration;

            var currentTransform = currentSection.Section.Transform(t - startTime);

            foreach (var child in _Children)
            {
                child.ApplyTransform(currentTransform);
                child.Render(t);
            }
        }
    }
}