using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
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

        public IDisposable DisplayUndoable(IModelDoc2 doc, int layer = 0)
        {
            if (SectionTimes == null)
            {
                CalculateSectionTimes(DateTime.Now);
            }

            var d0 = Redraw(doc/*, () => SectionTimes.Last().EndTime > DateTime.Now*/, Framerate);
            var d1 = Disposable.Create(() => SectionTimes = null);

            var d2 = OpenGlRenderer.DisplayUndoable(this, doc, layer);
            return new CompositeDisposable(d0, d1, d2);
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

        public async Task ToTask(IModelDoc2 doc, CancellationToken token, int layer = 0)
        {
            using (DisplayUndoable(doc,layer).DisposeWith(token))
                await CompletionTask;
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