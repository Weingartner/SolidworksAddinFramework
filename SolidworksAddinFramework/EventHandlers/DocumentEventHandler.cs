using System.Collections;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public class DocumentEventHandler
    {
        protected ISldWorks SwApp;
        protected ModelDoc2 Document;
        protected SwAddinBase UserAddin;

        protected Hashtable OpenModelViews;

        public DocumentEventHandler(ModelDoc2 modDoc, SwAddinBase addin)
        {
            Document = modDoc;
            UserAddin = addin;
            SwApp = UserAddin.SwApp;
            OpenModelViews = new Hashtable();
        }

        public virtual bool AttachEventHandlers()
        {
            return true;
        }

        public virtual bool DetachEventHandlers()
        {
            return true;
        }

        public bool ConnectModelViews()
        {
            var mView = (IModelView)Document.GetFirstModelView();

            while (mView != null)
            {
                if (!OpenModelViews.Contains(mView))
                {
                    var dView = new DocView(UserAddin.SwApp, mView);
                    dView.AttachEventHandlers();
                    OpenModelViews.Add(mView, dView);
                }
                mView = (IModelView)mView.GetNext();
            }
            return true;
        }

        public bool DisconnectModelViews()
        {
            //Close events on all currently open docs
            var numKeys = OpenModelViews.Count;

            if (numKeys == 0)
            {
                return false;
            }


            var keys = new object[numKeys];

            //Remove all ModelView event handlers
            OpenModelViews.Keys.CopyTo(keys, 0);
            foreach (ModelView key in keys)
            {
                var dView = (DocView)OpenModelViews[key];
                dView.DetachEventHandlers();
                this.DetachModelViewEventHandler(key);
                OpenModelViews.Remove(key);
            }
            return true;
        }

        public bool DetachModelViewEventHandler(ModelView mView)
        {
            if (OpenModelViews.Contains(mView))
            {
                OpenModelViews.Remove(mView);
            }
            return true;
        }
    }
}