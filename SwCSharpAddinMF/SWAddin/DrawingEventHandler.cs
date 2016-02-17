using SolidWorks.Interop.sldworks;

namespace SwCSharpAddinMF.SWAddin
{
    public class DrawingEventHandler : DocumentEventHandler
    {
        private DrawingDoc _Doc;

        public DrawingEventHandler(ModelDoc2 modDoc, SwAddinBase addin)
            : base(modDoc, addin)
        {
            _Doc = (DrawingDoc)Document;
        }

        public override bool AttachEventHandlers()
        {
            _Doc.DestroyNotify += OnDestroy;
            _Doc.NewSelectionNotify += OnNewSelection;

            ConnectModelViews();

            return true;
        }

        public override bool DetachEventHandlers()
        {
            _Doc.DestroyNotify -= OnDestroy;
            _Doc.NewSelectionNotify -= OnNewSelection;

            DisconnectModelViews();

            UserAddin.DetachModelEventHandler(Document);
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