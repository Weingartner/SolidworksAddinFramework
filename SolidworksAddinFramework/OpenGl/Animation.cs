using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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


    public class Animator : IRenderable
    {
        public IReadOnlyList<IAnimationSection> AnimationSections { get; }
        public DateTime StartTime { get; private set; }
        private readonly IReadOnlyList<IRenderable> _Children;
        private IMathUtility _Math;

        public Animator(IReadOnlyList<IAnimationSection> animationSections, IReadOnlyList<IRenderable> children, IMathUtility math)
        {
            AnimationSections = animationSections;
            _Children = children;
            _Math = math;
        }
        public IDisposable Animate(IModelDoc2 doc, TimeSpan? startDelay = null , int framerate = 60)
        {
            StartTime = DateTime.Now + (startDelay ?? TimeSpan.Zero);
            var d = OpenGlRenderer.DisplayUndoable(this, doc);
            var interval = 1.0/framerate;
            var d2 = Observable.Interval(TimeSpan.FromSeconds(interval))
                .ObserveOnUiDispatcher()
                .Subscribe(l =>
                {
                    doc.GraphicsRedraw2();
                });

            return new CompositeDisposable(d,d2);
        }

        public void Render(DateTime t)
        {
            var os = AnimationSections
                .Scan(new
                {
                    section = (IAnimationSection) null
                    , endTime = StartTime
                }, (acc, section) => new
                {
                    section
                    , endTime = acc.endTime + section.Duration
                })
                .ToList();

            var currentSection = os.FirstOrDefault(o => o.endTime > t);
            if (currentSection == null)
                return;

            var startTime = currentSection.endTime - currentSection.section.Duration;

            var currentTransform = currentSection.section.Transform(t - startTime);

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