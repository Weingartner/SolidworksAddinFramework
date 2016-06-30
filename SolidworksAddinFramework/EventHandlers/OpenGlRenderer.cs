using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.Events;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;


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
                    Lookup.GetOrAdd(modelDoc, mv => new OpenGlRenderer(modelView));

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

        public static IDisposable DisplayUndoable(IRenderable renderable, IModelDoc2 doc, int layer = 0)
        {
            if (renderable == null)
                throw new ArgumentNullException(nameof(renderable));
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            OpenGlRenderer openGlRenderer;
            if (Lookup.TryGetValue(doc, out openGlRenderer))
            {
                return openGlRenderer.DisplayUndoableImpl(renderable, doc, layer);
            }
            throw new Exception("Can't render OpenGL content, because the model view wasn't setup properly.");
        }

        private readonly ModelView _MView;

        private readonly IDisposable _Disposable;

        private OpenGlRenderer(ModelView mv)
        {
            if (mv == null)
                throw new ArgumentNullException(nameof(mv));

            GLDoubleBuffer = new GLDoubleBuffer(mv);

            _MView = mv;

            DoSetup();
            _Disposable = _MView.BufferSwapNotifyObservable().Subscribe(args =>
            {
                var time = DateTime.Now;
                var layers =
                    GLDoubleBuffer.Front.GroupBy(o => o.Value.Item1)
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
                        r.Render(time);
                }
            });
        }

        private readonly GLDoubleBuffer GLDoubleBuffer;

        private IDisposable DisplayUndoableImpl(IRenderable renderable, IModelDoc2 doc, int layer)
        {
            if (renderable == null)
                throw new ArgumentNullException(nameof(renderable));
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            GLDoubleBuffer.Update(b => b.SetItem(renderable, Tuple.Create(layer, renderable)));

            Redraw(doc);

            return Disposable.Create(() =>
            {
                GLDoubleBuffer.Update(btr =>
                {
                    if (btr.ContainsKey(renderable))
                    {
                        return btr.Remove(renderable);
                    }
                    else
                    {
                        return btr;
                    }
                });
                Redraw(doc);
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
                    if (renderer.GLDoubleBuffer.FrontIsActive)
                        activeView.GraphicsRedraw(null);
                });
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


        public static IDisposable DeferRedraw(IModelDoc2 doc, Func<IDisposable> fn  )
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));
            if (fn == null)
                throw new ArgumentNullException(nameof(fn));


            return  ((IReadOnlyDictionary<IModelDoc2,OpenGlRenderer>) Lookup)
                .TryGetValue(doc)
                .Match
                    ( v=>v.GLDoubleBuffer.RunWithBackBuffer(fn)
                    , ()=>Disposable.Empty
                    );
        }

        public static IDisposable DeferRedraw(IModelDoc2 doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            return  ((IReadOnlyDictionary<IModelDoc2,OpenGlRenderer>) Lookup)
                .TryGetValue(doc)
                .Match
                    ( v=>v.GLDoubleBuffer.SwitchToBackBufferTemporarily()
                    , ()=>Disposable.Empty
                    );
        }

    }
}
