using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using SolidworksAddinFramework.EventHandlers;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public class GLDoubleBuffer : DoubleBuffer<ImmutableDictionary<IRenderable, Tuple<int, IRenderable>>>
    {
        private ModelView _ModelView;

        public GLDoubleBuffer(ModelView modelView) : base(ImmutableDictionary<IRenderable, Tuple<int, IRenderable>>.Empty)
        {
            _ModelView = modelView;
        }

        public override void OnSwitchToFront()
        {
            _ModelView.GraphicsRedraw(null);
        }

        public override void OnSwitchToBack()
        {
            _ModelView.GraphicsRedraw(null);
        }

        public T RunWithBackBuffer<T>(Func<T> fn)
        {
            using (SwitchToBackBufferTemporarily())
            {
                return fn();
            }
        }
        public void  RunWithBackBuffer(Action fn)
        {
            using (SwitchToBackBufferTemporarily())
            {
                fn();
            }
        }

        public IDisposable DisposeOnBackBuffer(IDisposable d)
        {
            return Disposable.Create
                (() =>
                {
                    using (SwitchToBackBufferTemporarily())
                    {
                        d.Dispose();
                    }

                });
        }


        public IDisposable RunWithBackBuffer(Func<IDisposable> fn)
        {
            return DisposeOnBackBuffer(RunWithBackBuffer<IDisposable>(fn));
        }
    }

}