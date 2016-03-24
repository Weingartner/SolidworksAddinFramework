using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public class AssemblyEventHandler : DocumentEventHandler
    {
        private readonly AssemblyDoc _Doc;
        private readonly SwAddinBase _SwAddin;

        public AssemblyEventHandler(IModelDoc2 modDoc, SwAddinBase addin)
            : base(modDoc, addin)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            _Doc = (AssemblyDoc)Document;
            _SwAddin = addin;
        }

        public override bool AttachEventHandlers()
        {
            _Doc.DestroyNotify += OnDestroy;
            _Doc.NewSelectionNotify += OnNewSelection;
            _Doc.ComponentStateChangeNotify2 += ComponentStateChangeNotify2;
            _Doc.ComponentStateChangeNotify += ComponentStateChangeNotify;
            _Doc.ComponentVisualPropertiesChangeNotify += ComponentVisualPropertiesChangeNotify;
            _Doc.ComponentDisplayStateChangeNotify += ComponentDisplayStateChangeNotify;
            ConnectModelViews();

            return true;
        }

        public override bool DetachEventHandlers()
        {
            _Doc.DestroyNotify -= OnDestroy;
            _Doc.NewSelectionNotify -= OnNewSelection;
            _Doc.ComponentStateChangeNotify2 -= ComponentStateChangeNotify2;
            _Doc.ComponentStateChangeNotify -= ComponentStateChangeNotify;
            _Doc.ComponentVisualPropertiesChangeNotify -= ComponentVisualPropertiesChangeNotify;
            _Doc.ComponentDisplayStateChangeNotify -= ComponentDisplayStateChangeNotify;
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

        //attach events to a component if it becomes resolved
        protected int ComponentStateChange(object componentModel, short newCompState)
        {
            var modDoc = (ModelDoc2)componentModel;
            var newState = (swComponentSuppressionState_e)newCompState;


            switch (newState)
            {

                case swComponentSuppressionState_e.swComponentFullyResolved:
                {
                    if (modDoc != null && !_SwAddin.OpenDocs.Contains(modDoc))
                    {
                        _SwAddin.AttachModelDocEventHandler(modDoc);
                    }
                    break;
                }

                case swComponentSuppressionState_e.swComponentResolved:
                {
                    if (modDoc != null && !_SwAddin.OpenDocs.Contains(modDoc))
                    {
                        _SwAddin.AttachModelDocEventHandler(modDoc);
                    }
                    break;
                }

            }
            return 0;
        }

        protected int ComponentStateChange(object componentModel)
        {
            ComponentStateChange(componentModel, (short)swComponentSuppressionState_e.swComponentResolved);
            return 0;
        }


        public int ComponentStateChangeNotify2(object componentModel, string compName, short oldCompState, short newCompState)
        {
            return ComponentStateChange(componentModel, newCompState);
        }

        private int ComponentStateChangeNotify(object componentModel, short oldCompState, short newCompState)
        {
            return ComponentStateChange(componentModel, newCompState);
        }

        private int ComponentDisplayStateChangeNotify(object swObject)
        {
            var component = (Component2)swObject;
            var modDoc = (ModelDoc2)component.GetModelDoc();

            return ComponentStateChange(modDoc);
        }

        private int ComponentVisualPropertiesChangeNotify(object swObject)
        {
            var component = (Component2)swObject;
            var modDoc = (ModelDoc2)component.GetModelDoc();

            return ComponentStateChange(modDoc);
        }




    }
}