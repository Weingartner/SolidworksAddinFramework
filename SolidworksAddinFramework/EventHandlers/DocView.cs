using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using OpenTK.Graphics.OpenGL;
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

        public static ConcurrentDictionary<IBody2, TessellatableBody> BodiesToRender = new ConcurrentDictionary<IBody2, TessellatableBody>();

        public static IDisposable DisplayUndoable(TessellatableBody body, Color? color, IModelDoc2 doc)
        {
            BodiesToRender[body.Body]=body;
            ((IModelView)doc.ActiveView).GraphicsRedraw(null);
            return Disposable.Create(() =>
            {
                TessellatableBody dummy;
                BodiesToRender.TryRemove(body.Body, out dummy);
            });
        }
        public static IDisposable DisplayUndoable(IBody2 body, Color? color, IModelDoc2 doc, IMathUtility math)
        {
            BodiesToRender[body]=new TessellatableBody(math,body);
            return Disposable.Create(() =>
            {
                TessellatableBody dummy;
                BodiesToRender.TryRemove(body, out dummy);
            });
        }

        private int OnBufferSwapNotify()
        {

            foreach (var o in BodiesToRender.Values)
            {
                o.Tesselation.RenderOpenGL(_ISwApp);
            }

            //foreach (var body in ModelDoc.GetBodiesTs())
            //{
            //    MeshRender.Render(body, _ISwApp);
            //}

            //const int GL_LINES = 1;

            //glLineWidth(3);

            //glBegin(GL_LINES);
            //glVertex3f(0.0F, 0.0F, 0.0F);
            //glVertex3f(0.5F, 0.5F, 0.5F);
            //glEnd();


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
