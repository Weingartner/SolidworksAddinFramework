using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using static OpenGl;


namespace SolidworksAddinFramework
{
    public class DocView
    {
        private ISldWorks _ISwApp;
        private ModelView _MView;

        public DocView(ISldWorks app, IModelView mv)
        {
            _MView = (ModelView)mv;
            _ISwApp = (ISldWorks)app;
        }

        public bool AttachEventHandlers()
        {
            _MView.DestroyNotify2 += OnDestroy;
            _MView.RepaintNotify += OnRepaint;
            _MView.BufferSwapNotify += OnBufferSwapNotify;
            return true;
        }

        public static ConcurrentDictionary<IRenderable, IRenderable> BodiesToRender = 
            new ConcurrentDictionary<IRenderable, IRenderable>();

        public static IDisposable DisplayUndoable(IRenderable body, Color? color, IModelDoc2 doc)
        {
            BodiesToRender[body] = body;
            ((IModelView)doc.ActiveView).GraphicsRedraw(null);
            return Disposable.Create(() =>
            {
                IRenderable dummy;
                BodiesToRender.TryRemove(body, out dummy);
            });
        }

        private int OnBufferSwapNotify()
        {

            var time = DateTime.Now;
            DoSetup(_ISwApp);
            foreach (var o in BodiesToRender.Values)
            {
                o.Render(time);
            }

            ((IModelView) ModelDoc.ActiveView).GraphicsRedraw(null);

            return 0;
        }

        private IModelDoc2 ModelDoc => ((IModelDoc2)_MView.GetModelDoc());

        public bool DetachEventHandlers()
        {
            _MView.DestroyNotify2 -= OnDestroy;
            _MView.RepaintNotify -= OnRepaint;
            return true;
        }

        //EventHandlers
        public int OnDestroy(int destroyType)
        {
            return 0;
        }

        public int OnRepaint(int repaintType)
        {
            return 0;
        }

        public static bool _Setup;

        public static void DoSetup(ISldWorks app)
        {
            if (!_Setup)
            {
                _Setup = true;
                var modelDoc = (IModelDoc2) app.ActiveDoc;
                ////modelDoc.ViewOglShading();
                var view = (IModelView) modelDoc.ActiveView;
                ////view.InitializeShading();
                //var windowHandle = (IntPtr) view.GetViewHWndx64();
                view.UpdateAllGraphicsLayers = true;
                view.InitializeShading();
                //Toolkit.Init();
                //var windowInfo = Utilities.CreateWindowsWindowInfo(windowHandle);
                ////var context = new GraphicsContext(GraphicsMode.Default, windowInfo);
                //var contextHandle = new ContextHandle(windowHandle);
                //var context = new GraphicsContext(contextHandle, Wgl.GetProcAddress, () => contextHandle);
                //context.MakeCurrent(windowInfo);
                //context.LoadAll();
                new GLControl().CreateGraphics();
                //Toolkit.Init();
                //IGraphicsContext context = new GraphicsContext(
                //    new ContextHandle(windowHandle),null );
                //context.LoadAll();
            }
        }
    }
}
