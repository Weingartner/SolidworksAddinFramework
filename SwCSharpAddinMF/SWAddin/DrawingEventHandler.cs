using SolidWorks.Interop.sldworks;

namespace SwCSharpAddinMF.SWAddin
{
    public class DrawingEventHandler : DocumentEventHandler
    {
        DrawingDoc doc;

        public DrawingEventHandler(ModelDoc2 modDoc, SwAddinBase addin)
            : base(modDoc, addin)
        {
            doc = (DrawingDoc)document;
        }

        override public bool AttachEventHandlers()
        {
            doc.DestroyNotify += new DDrawingDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify += new DDrawingDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);

            ConnectModelViews();

            return true;
        }

        override public bool DetachEventHandlers()
        {
            doc.DestroyNotify -= new DDrawingDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify -= new DDrawingDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);

            DisconnectModelViews();

            userAddin.DetachModelEventHandler(document);
            return true;
        }

        //Event Handlers
        public int OnDestroy()
        {
            DetachEventHandlers();
            return 0;
        }

        public int OnNewSelection()
        {
            return 0;
        }
    }
}