using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.OpenGl
{
    public interface IAnimationSection
    {
        TimeSpan Duration { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="math"></param>
        /// <param name="deltaTime">time from the begining of the start of this section</param>
        /// <returns></returns>
        IMathTransform Transform(IMathUtility math, TimeSpan deltaTime);
    }

    public class LinearAnimation : ReactiveObject, IAnimationSection
    {
        public MathVector FromTrans { get; }
        public MathVector ToTrans { get; set; }
        public MathVector RotationAxis { get; }

        public MathPoint Origin { get; }
        public TimeSpan Duration { get; }
        public double FromRotationAngle { get; }
        public double ToRotationAngle { get; }


        public LinearAnimation
            (TimeSpan duration
            , MathVector fromTrans
            , MathVector toTrans
            , MathPoint origin
            , MathVector rotationAxis
            , double fromRotationAngle
            , double toRotationAngle)
        {
            Duration = duration;
            FromTrans = fromTrans;
            ToTrans = toTrans;
            Origin = origin;
            RotationAxis = rotationAxis;
            FromRotationAngle = fromRotationAngle;
            ToRotationAngle = toRotationAngle;
        }



        public IMathTransform Transform(IMathUtility math, TimeSpan deltaTime)
        {
            var alpha = deltaTime.TotalMilliseconds/Duration.TotalMilliseconds;

            var trans = math.ComposeTransform(math.XAxis(), math.YAxis(), math.ZAxis(), FromTrans.AlphaBend(ToTrans, alpha),1);
            var rot = (IMathTransform) math.CreateTransformRotateAxis(Origin, RotationAxis, alpha * ToRotationAngle + ( 1-alpha) * FromRotationAngle);

            return (IMathTransform) trans.Multiply(rot);
        }

    }


    public class Animator : IRenderable
    {
        public IReadOnlyList<IAnimationSection> AnimationSections { get; }
        public DateTime StartTime { get; }
        private readonly IReadOnlyList<IRenderable> _Children;
        private IMathUtility _Math;

        public Animator(IReadOnlyList<IAnimationSection> animationSections, IReadOnlyList<IRenderable> children, IMathUtility math, DateTime startTime)
        {
            AnimationSections = animationSections;
            StartTime = startTime;
            _Children = children;
            _Math = math;
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

            var currentTransform = currentSection.section.Transform(_Math, t - startTime);

            var transform = _Math.IdentityTransform();

            os
                .Where(o => o.endTime < t)
                .Select(o => o.section.Transform(_Math, o.section.Duration))
                .ForEach(trans =>
                {
                    transform = (MathTransform)transform.Multiply(trans);
                });

            var tr = (MathTransform)transform.Multiply(currentTransform);
            foreach (var child in _Children)
            {
                child.ApplyTransform(tr);
                child.Render(t);
            }
        }


        public void ApplyTransform(IMathTransform transform)
        {
        }
    }
}