using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.Interpolation;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IAnimationSection
    {
        TimeSpan Duration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime">time from the begining of the start of this section</param>
        /// <returns></returns>
        Matrix4x4 Transform(TimeSpan deltaTime);
    }

    public class LinearAnimation<T> : ReactiveObject, IAnimationSection
        where T : IInterpolatable<T>
    {
        private readonly IMathUtility _Math;
        public T From { get; } 
        public T To { get; } 
        public TimeSpan Duration { get; }

        public LinearAnimation
            (TimeSpan duration, T @from, T to, IMathUtility math)  
        {
            Duration = duration;
            From = @from;
            To = to;
            _Math = math;
        }

        public Matrix4x4 Transform( TimeSpan deltaTime)
        {
            var beta = deltaTime.TotalMilliseconds/Duration.TotalMilliseconds;

            return BlendTransform(beta);
        }

        public Matrix4x4 BlendTransform(double beta)
        {
            Debug.Assert(beta>=0 && beta<=1);
            return From.Interpolate(To, beta).Transform(_Math);
        }
    }


    public class SectionTime
    {
        public IAnimationSection Section { get; }

        public DateTime EndTime { get; }

        public SectionTime(IAnimationSection section, DateTime endTime)
        {
            Section = section;
            EndTime = endTime;
        }
    }

    public class Animator : IRenderable
    {
        public IReadOnlyList<SectionTime> SectionTimes { get; }
        private readonly IReadOnlyList<IRenderable> _Children;

        private Animator(IReadOnlyList<SectionTime> sectionTimes, IReadOnlyList<IRenderable> children)
        {
            SectionTimes = sectionTimes;
            _Children = children;
        }

        public static async Task Animate
            (IEnumerable<IAnimationSection> animationSections,
            IReadOnlyList<IRenderable> children,
            IModelDoc2 doc,
            CancellationToken token,
            TimeSpan? startDelay = null,
            int framerate = 30)
        {
            if (token.IsCancellationRequested)
                return;

            var startTime = DateTime.Now + (startDelay ?? TimeSpan.Zero);
            var sectionTimes = animationSections
                .Scan(new SectionTime(null, startTime), (acc, section) => new SectionTime(section, acc.EndTime + section.Duration))
                .ToList();
            var animator = new Animator(sectionTimes, children);
            using (OpenGlRenderer.DisplayUndoable(animator, doc))
            {
                await Observable.Interval(TimeSpan.FromSeconds(1.0/framerate))
                    .ObserveOnUiDispatcher()
                    .TakeWhile(_=>sectionTimes.Last().EndTime > DateTime.Now && !token.IsCancellationRequested)
                    .Select(l =>
                    {
                        doc.GraphicsRedraw2();
                        return Unit.Default;
                    })
                    // Just in case the sequence terminate before there
                    // is a value.
                    .StartWith(Unit.Default);

            }
        }

        public void Render(DateTime t)
        {

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
    }
}