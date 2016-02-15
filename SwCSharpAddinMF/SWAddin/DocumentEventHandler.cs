using System.Collections;
using SolidWorks.Interop.sldworks;

namespace SwCSharpAddinMF.SWAddin
{
    public class DocumentEventHandler
    {
        protected ISldWorks iSwApp;
        protected ModelDoc2 document;
        protected SwAddinBase userAddin;

        protected Hashtable openModelViews;

        public DocumentEventHandler(ModelDoc2 modDoc, SwAddinBase addin)
        {
            document = modDoc;
            userAddin = addin;
            iSwApp = (ISldWorks)userAddin.SwApp;
            openModelViews = new Hashtable();
        }

        virtual public bool AttachEventHandlers()
        {
            return true;
        }

        virtual public bool DetachEventHandlers()
        {
            return true;
        }

        public bool ConnectModelViews()
        {
            IModelView mView;
            mView = (IModelView)document.GetFirstModelView();

            while (mView != null)
            {
                if (!openModelViews.Contains(mView))
                {
                    DocView dView = new DocView(userAddin, mView, this);
                    dView.AttachEventHandlers();
                    openModelViews.Add(mView, dView);
                }
                mView = (IModelView)mView.GetNext();
            }
            return true;
        }

        public bool DisconnectModelViews()
        {
            //Close events on all currently open docs
            DocView dView;
            int numKeys;
            numKeys = openModelViews.Count;

            if (numKeys == 0)
            {
                return false;
            }


            object[] keys = new object[numKeys];

            //Remove all ModelView event handlers
            openModelViews.Keys.CopyTo(keys, 0);
            foreach (ModelView key in keys)
            {
                dView = (DocView)openModelViews[key];
                dView.DetachEventHandlers();
                openModelViews.Remove(key);
                dView = null;
            }
            return true;
        }

        public bool DetachModelViewEventHandler(ModelView mView)
        {
            DocView dView;
            if (openModelViews.Contains(mView))
            {
                dView = (DocView)openModelViews[mView];
                openModelViews.Remove(mView);
                mView = null;
                dView = null;
            }
            return true;
        }
    }
}