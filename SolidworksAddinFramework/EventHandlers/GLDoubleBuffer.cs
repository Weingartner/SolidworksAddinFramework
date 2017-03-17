using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using SolidworksAddinFramework.EventHandlers;

using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Drawing;
using LogViewer = Weingartner.WeinCad.Interfaces.LogViewer;

namespace SolidworksAddinFramework
{
    public class GLDoubleBuffer : DoubleBuffer<ImmutableDictionary<IRenderer, Tuple<int, IRenderer>>>
    {
        private readonly ModelView _ModelView;

        public GLDoubleBuffer(ModelView modelView) : base(ImmutableDictionary<IRenderer, Tuple<int, IRenderer>>.Empty)
        {
            _ModelView = modelView;
        }

        public override void OnSwitchToFront()
        {
            Redraw();
        }

        public override void OnSwitchToBack()
        {
            Redraw();
        }

        private void Redraw()
        {
            try
            {
                _ModelView.GraphicsRedraw(null);
            }
            catch (COMException e)
            {
                LogViewer.Log($"Exception was expected '{e.Message}' but logging it anyway");
            }
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