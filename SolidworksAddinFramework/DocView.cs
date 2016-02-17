using SolidWorks.Interop.sldworks;

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
            return true;
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
