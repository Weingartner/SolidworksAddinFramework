using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using SolidWorks.Interop.sldworks;
using Weingartner.Exceptional.Reactive;

namespace SolidworksAddinFramework.OpenGl.Animation
{
    public abstract class AnimatorBase : IRenderable
    {
        public abstract TimeSpan Duration { get; }
        public abstract ImmutableList<IAnimationSection> Sections { get; }

        public IDisposable DisplayUndoable(IModelDoc2 modelDoc, double framerate = 30, int layer = 0)
        {
            OnStart(DateTime.Now);

            var d = new CompositeDisposable();
            OpenGlRenderer.DisplayUndoable(this, modelDoc, layer).DisposeWith(d);
            Redraw(modelDoc, framerate).DisposeWith(d);
            return d;
        }

        public abstract void OnStart(DateTime startTime);

        public abstract void Render(DateTime time);

        public Tuple<Vector3, float> BoundingSphere
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ApplyTransform(Matrix4x4 transform)
        {
            throw new NotImplementedException();
        }

        private static IDisposable Redraw(IModelDoc2 doc, double framerate = 30)
        {
            return Observable.Interval(TimeSpan.FromSeconds(1.0 / framerate))
                .ToObservableExceptional()
                .ObserveOnSolidworksThread()
                .Subscribe(_ => doc.GraphicsRedraw2());
        }
    }
}