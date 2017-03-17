using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using SolidWorks.Interop.sldworks;
using Weingartner.Exceptional.Reactive;
using Weingartner.WeinCad.Interfaces.Drawing.Animation;

namespace SolidworksAddinFramework.OpenGl
{
    public static class AnimatorExtensions
    {
        public static IDisposable DisplayUndoable(this AnimatorBase animatorBase, IModelDoc2 modelDoc, double framerate = 30, int layer = 0)
        {
            animatorBase.OnStart(DateTime.Now);

            var d = new CompositeDisposable();
            OpenGlRenderer.DisplayUndoable(animatorBase, modelDoc, layer).DisposeWith(d);
            Redraw(modelDoc, framerate).DisposeWith(d);
            return d;
        }
        private static IDisposable Redraw(this IModelDoc2 doc, double framerate = 30)
        {
            return Observable.Interval(TimeSpan.FromSeconds(1.0 / framerate))
                             .ToObservableExceptional()
                             .ObserveOnSolidworksThread()
                             .Subscribe(_ => doc.GraphicsRedraw2());
        }

        
    }
}