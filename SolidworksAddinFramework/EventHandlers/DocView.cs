using System;
using System.Linq;
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

        private int OnBufferSwapNotify()
        {
            var faces = ((IModelDoc2)_MView.GetModelDoc())
                .GetBodiesTs()
                .SelectMany(b => b.GetFaces().CastArray<IFace2>())
                .ToArray();
            MeshRender.Render(faces, _ISwApp);

            //const int GL_LINES = 1;

            //glLineWidth(3);

            //glBegin(GL_LINES);
            //glVertex3f(0.0F, 0.0F, 0.0F);
            //glVertex3f(0.5F, 0.5F, 0.5F);
            //glEnd();


            return 0;
        }

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
