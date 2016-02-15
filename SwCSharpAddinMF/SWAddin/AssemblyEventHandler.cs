using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SwCSharpAddinMF.SWAddin
{
    public class AssemblyEventHandler : DocumentEventHandler
    {
        AssemblyDoc doc;
        SwAddinBase swAddin;

        public AssemblyEventHandler(ModelDoc2 modDoc, SwAddinBase addin)
            : base(modDoc, addin)
        {
            doc = (AssemblyDoc)document;
            swAddin = addin;
        }

        override public bool AttachEventHandlers()
        {
            doc.DestroyNotify += new DAssemblyDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify += new DAssemblyDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);
            doc.ComponentStateChangeNotify2 += new DAssemblyDocEvents_ComponentStateChangeNotify2EventHandler(ComponentStateChangeNotify2);
            doc.ComponentStateChangeNotify += new DAssemblyDocEvents_ComponentStateChangeNotifyEventHandler(ComponentStateChangeNotify);
            doc.ComponentVisualPropertiesChangeNotify += new DAssemblyDocEvents_ComponentVisualPropertiesChangeNotifyEventHandler(ComponentVisualPropertiesChangeNotify);
            doc.ComponentDisplayStateChangeNotify += new DAssemblyDocEvents_ComponentDisplayStateChangeNotifyEventHandler(ComponentDisplayStateChangeNotify);
            ConnectModelViews();

            return true;
        }

        override public bool DetachEventHandlers()
        {
            doc.DestroyNotify -= new DAssemblyDocEvents_DestroyNotifyEventHandler(OnDestroy);
            doc.NewSelectionNotify -= new DAssemblyDocEvents_NewSelectionNotifyEventHandler(OnNewSelection);
            doc.ComponentStateChangeNotify2 -= new DAssemblyDocEvents_ComponentStateChangeNotify2EventHandler(ComponentStateChangeNotify2);
            doc.ComponentStateChangeNotify -= new DAssemblyDocEvents_ComponentStateChangeNotifyEventHandler(ComponentStateChangeNotify);
            doc.ComponentVisualPropertiesChangeNotify -= new DAssemblyDocEvents_ComponentVisualPropertiesChangeNotifyEventHandler(ComponentVisualPropertiesChangeNotify);
            doc.ComponentDisplayStateChangeNotify -= new DAssemblyDocEvents_ComponentDisplayStateChangeNotifyEventHandler(ComponentDisplayStateChangeNotify);
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

        //attach events to a component if it becomes resolved
        protected int ComponentStateChange(object componentModel, short newCompState)
        {
            ModelDoc2 modDoc = (ModelDoc2)componentModel;
            swComponentSuppressionState_e newState = (swComponentSuppressionState_e)newCompState;


            switch (newState)
            {

                case swComponentSuppressionState_e.swComponentFullyResolved:
                {
                    if ((modDoc != null) & !this.swAddin.OpenDocs.Contains(modDoc))
                    {
                        this.swAddin.AttachModelDocEventHandler(modDoc);
                    }
                    break;
                }

                case swComponentSuppressionState_e.swComponentResolved:
                {
                    if ((modDoc != null) & !this.swAddin.OpenDocs.Contains(modDoc))
                    {
                        this.swAddin.AttachModelDocEventHandler(modDoc);
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


        public int ComponentStateChangeNotify2(object componentModel, string CompName, short oldCompState, short newCompState)
        {
            return ComponentStateChange(componentModel, newCompState);
        }

        int ComponentStateChangeNotify(object componentModel, short oldCompState, short newCompState)
        {
            return ComponentStateChange(componentModel, newCompState);
        }

        int ComponentDisplayStateChangeNotify(object swObject)
        {
            Component2 component = (Component2)swObject;
            ModelDoc2 modDoc = (ModelDoc2)component.GetModelDoc();

            return ComponentStateChange(modDoc);
        }

        int ComponentVisualPropertiesChangeNotify(object swObject)
        {
            Component2 component = (Component2)swObject;
            ModelDoc2 modDoc = (ModelDoc2)component.GetModelDoc();

            return ComponentStateChange(modDoc);
        }




    }
}