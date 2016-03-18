using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using OpenTK.Graphics.OpenGL;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using static OpenGl;


namespace SolidworksAddinFramework
{
    public class DocView
    {
        private ISldWorks _ISwApp;
        private SwAddinBase _UserAddin;
        private ModelView _MView;
        private DocumentEventHandler _Parent;

        public DocView(SwAddinBase addin, IModelView mv, DocumentEventHandler doc)
        {
            _UserAddin = addin;
            _MView = (ModelView)mv;
            _ISwApp = (ISldWorks)_UserAddin.SwApp;
            _Parent = doc;
        }

        public bool AttachEventHandlers()
        {
            _MView.DestroyNotify2 += OnDestroy;
            _MView.RepaintNotify += OnRepaint;
            _MView.BufferSwapNotify += OnBufferSwapNotify;
            return true;
        }

        public class BodyRender
        {
            public BodyRender(TessellatableBody body, Color color)
            {
                Body = body;
                Color = color;
            }

            public TessellatableBody Body { get; }
            public Color Color { get; }
        }

        public static ConcurrentDictionary<TessellatableBody, BodyRender> BodiesToRender = 
            new ConcurrentDictionary<TessellatableBody, BodyRender>();

        public static IDisposable DisplayUndoable(TessellatableBody body, Color? color, IModelDoc2 doc)
        {
            BodiesToRender[body]= new BodyRender(body, color ?? Color.Yellow);
            ((IModelView)doc.ActiveView).GraphicsRedraw(null);
            return Disposable.Create(() =>
            {
                BodyRender dummy;
                BodiesToRender.TryRemove(body, out dummy);
            });
        }

        private int OnBufferSwapNotify()
        {

            foreach (var o in BodiesToRender.Values)
            {
                o.Body.Tesselation.RenderOpenGL(_ISwApp, o.Color);
            }

            return 0;
        }

        private IModelDoc2 ModelDoc => ((IModelDoc2)_MView.GetModelDoc());

        public bool DetachEventHandlers()
        {
            _MView.DestroyNotify2 -= OnDestroy;
            _MView.RepaintNotify -= OnRepaint;
            _Parent.DetachModelViewEventHandler(_MView);
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
    }

}
