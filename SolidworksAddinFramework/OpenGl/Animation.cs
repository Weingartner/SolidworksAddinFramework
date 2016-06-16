using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.Interpolation;
using SolidWorks.Interop.sldworks;
using Weingartner.Exceptional.Reactive;

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
            return From.Interpolate(To, beta).Transform();
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

        public IDisposable DisplayUndoable
            (IModelDoc2 doc, int layer = 0)
        {
            if (SectionTimes != null)
                throw new Exception("You have allready added this animation");

            var cts = new CancellationDisposable();
            SectionTimes = Calculate(_Sections, TimeSpan.Zero);

            CompletionTask = Observable.Interval(TimeSpan.FromSeconds(1.0/Framerate))
                .ToObservableExceptional()
                .ObserveOnSolidworksThread()
                .TakeWhile(_ => SectionTimes.Last().EndTime > DateTime.Now && !cts.Token.IsCancellationRequested)
                .Select(l =>
                {
                    doc.GraphicsRedraw2();
                    return Unit.Default;
                })
                // Just in case the sequence terminate before there
                // is a value.
                .StartWith(Unit.Default)
                .Finally(()=>SectionTimes=null)
                .ToTask(cts.Token);

            var d = OpenGlRenderer.DisplayUndoable(this, doc);
            return new CompositeDisposable(d, cts);
        }

        public async Task ToTask(IModelDoc2 doc, CancellationToken token, int layer = 0)
        {
            using (DisplayUndoable(doc,layer).DisposeWith(token))
                await CompletionTask;
        }


        public static Animator Animate
            (IReadOnlyList<IAnimationSection> animationSections,
            IReadOnlyList<IRenderable> children,
            int framerate = 30)
        {
            return new Animator(animationSections, children, framerate);

        }

        private static List<SectionTime> Calculate(IReadOnlyList<IAnimationSection> animationSections, TimeSpan? startDelay)
        {
            var startTime = DateTime.Now + (startDelay ?? TimeSpan.Zero);
            var sectionTimes = animationSections
                .Scan(new SectionTime(null, startTime),
                    (acc, section) => new SectionTime(section, acc.EndTime + section.Duration))
                .ToList();
            return sectionTimes;
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

    public static class AnimationExtensions
    {
        
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IReadOnlyList<IRenderable> children,
            int framerate = 30)
        {
            return new Animator(animationSections, children, framerate);

        }
        public static Animator CreateAnimator
            (this IReadOnlyList<IAnimationSection> animationSections,
            IRenderable children,
            int framerate = 30)
        {
            return new Animator(animationSections, new [] { children}, framerate);

        }
    }
}