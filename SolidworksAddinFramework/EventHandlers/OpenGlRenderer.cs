using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using JetBrains.Annotations;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.Events;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using Weingartner.WeinCad.Interfaces.Drawing;


namespace SolidworksAddinFramework
{
    public class OpenGlRenderer : IDisposable
    {

        public static ConcurrentDictionary<IModelDoc2, OpenGlRenderer> Lookup = 
            new ConcurrentDictionary<IModelDoc2, OpenGlRenderer>();

        private static int _isInitialized;

        public static IDisposable Setup(SldWorks swApp)
        {
            if (swApp == null)
                throw new ArgumentNullException(nameof(swApp));

            if (Interlocked.CompareExchange(ref _isInitialized, 1, 0) == 1) return Disposable.Empty;

            var d0 = swApp
                .DoWithOpenDoc(modelDoc =>
                {
                    if (!modelDoc.Visible)
                        return Disposable.Empty;

                    var modelView = (ModelView) modelDoc.GetFirstModelView();
                    Lookup.GetOrAdd(modelDoc, mv => new OpenGlRenderer(modelView ));

                    return Disposable.Create(() =>
                    {
                        OpenGlRenderer openGlRenderer;
                        if (Lookup.TryRemove(modelDoc, out openGlRenderer))
                        {
                            openGlRenderer.Dispose();
                        }
                    });
                });

            var d1 = Disposable.Create(() =>
            {
                foreach (var modelView in Lookup.Keys)
                {
                    OpenGlRenderer openGlRenderer;
                    Lookup.TryRemove(modelView, out openGlRenderer);
                    openGlRenderer.Dispose();
                }
                Debug.Assert(Lookup.IsEmpty);
            });

            return new CompositeDisposable(d0, d1);
        }

        public static IDisposable DisplayUndoable(IRenderer renderer, IModelDoc2 doc, int layer = 0)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            OpenGlRenderer openGlRenderer;
            if (Lookup.TryGetValue(doc, out openGlRenderer))
            {
                return openGlRenderer.DisplayUndoableImpl(renderer, doc, layer);
            }
            throw new Exception("Can't render OpenGL content, because the model view wasn't setup properly.");
        }

        private readonly ModelView _MView;

        private readonly IDisposable _Disposable;

        private OpenGlRenderer(ModelView mv)
        {
            if (mv == null)
                throw new ArgumentNullException(nameof(mv));

            _GlDoubleBuffer = new GLDoubleBuffer(mv);
            _Scheduler = DispatcherScheduler.Current;

            _MView = mv;

            DoSetup();
            _Disposable = _MView.BufferSwapNotifyObservable().Subscribe(args =>
            {
                var time = DateTime.Now;
                var layers =
                    _GlDoubleBuffer.Front.GroupBy(o => o.Value.Item1)
                        .OrderBy(o => o.Key)
                        .Select(o => new {Index = o.Key, Renderables = o.Select(q => q.Value.Item2).ToList()})
                        .ToList();
                foreach (var layer in layers)
                {
                    // Clear the depth buffer after each subsequent layer. This
                    // will ensure that they are drawn on top of each other.
                    if(layer.Index!=0)
                        GL.Clear(ClearBufferMask.DepthBufferBit);
                    foreach (var r in layer.Renderables)
                        r.Render(time, _DrawContext);
                }
            });
            _DrawContext = new OpenGLDrawContext();
        }

        private readonly GLDoubleBuffer _GlDoubleBuffer;
        private readonly IScheduler _Scheduler;
        private readonly IDrawContext _DrawContext;

        private IDisposable DisplayUndoableImpl(IRenderer renderer, IModelDoc2 doc, int layer)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            _GlDoubleBuffer.Update(b => b.SetItem(renderer, Tuple.Create(layer, renderer)));

            Redraw(doc);

            var s = renderer.NeedsRedraw.Subscribe(v => Redraw(doc));

            var d = Disposable.Create(() =>
            {
                s.Dispose();
                _GlDoubleBuffer.Update(btr =>
                {
                    if (btr.ContainsKey(renderer))
                    {
                        return btr.Remove(renderer);
                    }
                    else
                    {
                        return btr;
                    }
                });
                Redraw(doc);
            });
            var t = Thread.CurrentThread;
            return Disposable.Create
                (() =>
                 {
                     Debug.Assert(Thread.CurrentThread == t);
                     d.Dispose();
                 });
        }

        private static void Redraw(IModelDoc2 doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            ((IReadOnlyDictionary<IModelDoc2, OpenGlRenderer>)Lookup)
                .TryGetValue(doc)
                .IfSome(renderer =>
                {
                    var activeView = (IModelView) doc.ActiveView;
                    if (renderer._GlDoubleBuffer.FrontIsActive && renderer._GlDoubleBuffer.RedrawEnabled)
                        activeView.GraphicsRedraw(null);
                });
        }

        public static IDisposable DisableRedraw(IModelDoc2 doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            return ((IReadOnlyDictionary<IModelDoc2, OpenGlRenderer>)Lookup)
                .TryGetValue(doc)
                .Match(renderer =>
                {
                    var activeView = (IModelView) doc.ActiveView;
                    renderer._GlDoubleBuffer.RedrawEnabled = false;
                    return Disposable.Create(() =>
                    {
                        renderer._GlDoubleBuffer.RedrawEnabled = true;

                    });
                },
                    () => { throw new Exception("Unable to get renderer"); });
            
        }

        private void DoSetup()
        {
            ////_MView.InitializeShading();
            //var windowHandle = (IntPtr) _MView.GetViewHWndx64();
            _MView.UpdateAllGraphicsLayers = true;
            _MView.InitializeShading();
            //Toolkit.Init();
            //var windowInfo = Utilities.CreateWindowsWindowInfo(windowHandle);
            ////var context = new GraphicsContext(GraphicsMode.Default, windowInfo);
            //var contextHandle = new ContextHandle(windowHandle);
            //var context = new GraphicsContext(contextHandle, Wgl.GetProcAddress, () => contextHandle);
            //context.MakeCurrent(windowInfo);
            //context.LoadAll();

            // TODO some resources are not disposed here.
            // Really we should call `var tk = Toolkit.Init();` here (that's what `GLControl.ctor` does)
            // and then dispose `tk`.
            using (var ctrl = new GLControl())
            using (ctrl.CreateGraphics())
            {
            }
            //Toolkit.Init();
            //IGraphicsContext context = new GraphicsContext(
            //    new ContextHandle(windowHandle),null );
            //context.LoadAll();
        }

        public void Dispose()
        {
            _Disposable.Dispose();
        }

        private IDisposable DeferRedraw(Func<GLDoubleBuffer, IDisposable> action)
        {
            var d = action(_GlDoubleBuffer);
            var t = Thread.CurrentThread;
            return Disposable.Create
                (() =>
                 {
                     Debug.Assert(Thread.CurrentThread == t);
                     d.Dispose();

                 });
        }

        private static IDisposable DeferRedraw([NotNull] IModelDoc2 doc, [NotNull] Func<GLDoubleBuffer, IDisposable> action)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (action == null) throw new ArgumentNullException(nameof(action));

            return ((IReadOnlyDictionary<IModelDoc2, OpenGlRenderer>) Lookup)
                .TryGetValue(doc)
                .Match(
                    v => v.DeferRedraw(action),
                    () => Disposable.Empty
                );
        }

        public static IDisposable DeferRedraw(IModelDoc2 doc, [NotNull] Func<IDisposable> fn)
        {
            if (fn == null) throw new ArgumentNullException(nameof(fn));

            Func<GLDoubleBuffer, IDisposable> getDisposable = p => p.RunWithBackBuffer(fn);
            return DeferRedraw(doc, getDisposable);
        }

        public static IDisposable DeferRedraw(IModelDoc2 doc)
        {
            return DeferRedraw(doc, b => b.SwitchToBackBufferTemporarily());
        }

    }
}
