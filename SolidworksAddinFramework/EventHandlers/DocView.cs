using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
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

        public static ConcurrentDictionary<IBody2, Tuple<IBody2, Color>> BodiesToRender = new ConcurrentDictionary<IBody2, Tuple<IBody2,Color>>();

        public static IDisposable DisplayUndoable(IBody2 body, Color? color, IModelDoc2 doc)
        {
            var check = body.Check3;
            if (!body.HasMaterialPropertyValues())
            {
                var r = body.SetMaterialProperty("Default", "solidworks materials.sldmat", "Steel");

            }
            Debug.Assert(body.IsTemporaryBody());
            //Debug.Assert(body.HasMaterialPropertyValues());

            // this is the only way to make sure that IFace2::GetTessTriangleCount always is greater than 0
            // BUT it slows down the rendering of course. I can't find another way to force the body to
            // tesselate the object.
            //body.Display3(doc, 0, 0);
            //body.Hide(doc);
            var faces = body.GetFaces().CastArray<IFace2>();
            foreach (var face in faces)
            {
                var norms = face.GetTessTriStripNorms();
                var tri = face.GetTessTriangleCount();
            }

            BodiesToRender[body]=Tuple.Create(body,(color ?? Color.Yellow));
            return Disposable.Create(() =>
            {
                Tuple<IBody2, Color> dummy;
                BodiesToRender.TryRemove(body, out dummy);
            });
        }

        private int OnBufferSwapNotify()
        {

            foreach (var o in BodiesToRender.Values)
            {
                var b = o.Item1;
                //var tess = b.GetTessellation(b.GetFaces()) as ITessellation;
                //tess?.Tessellate();
                MeshRender.Render(b.GetFaces().CastArray<IFace2>(), _ISwApp, o.Item2, 2.0f);
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
