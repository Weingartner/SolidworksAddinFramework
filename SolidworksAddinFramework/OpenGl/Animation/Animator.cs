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
    public class Animator : IRenderable
    {
        public IReadOnlyList<SectionTime> SectionTimes { get; private set; }
        private readonly IReadOnlyList<IAnimationSection> _Sections;
        private readonly IReadOnlyList<IRenderable> _Children;
        public double Framerate { get; }

        public Task CompletionTask { get; set; }

        public Animator(IReadOnlyList<IAnimationSection> sections, IReadOnlyList<IRenderable> children, int framerate=30)
        {
            _Sections = sections;
            _Children = children;
            Framerate = framerate;
        }

        public static IDisposable Redraw(IModelDoc2 doc, double framerate = 30)
        {
            return Observable.Interval(TimeSpan.FromSeconds(1.0 / framerate))
                .ToObservableExceptional()
                .ObserveOnSolidworksThread()
                .Subscribe(_ => doc.GraphicsRedraw2());
        }

        public void CalculateSectionTimes(DateTime startTime)
        {
            if (SectionTimes != null)
                throw new Exception("You have allready added this animation");
            SectionTimes = Calculate(_Sections, startTime);
        }

        private static List<SectionTime> Calculate(IReadOnlyList<IAnimationSection> animationSections, DateTime startTime)
        {
            return animationSections
                .Scan(new SectionTime(null, startTime), (acc, section) => new SectionTime(section, acc.EndTime + section.Duration))
                .ToList();
        }

        public void Render(DateTime t)
        {
            // ReSharper disable once UseNullPropagation
            if (SectionTimes == null)
                return;

            var currentSection = SectionTimes.FirstOrDefault(o => o.EndTime > t);
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