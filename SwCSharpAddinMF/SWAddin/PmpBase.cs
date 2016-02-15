using System;
using System.Collections.Generic;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF.SWAddin
{
    public abstract class PmpBase : IPropertyManagerPage2Handler9
    {
        public readonly ISldWorks SwApp;

        protected PmpBase(ISldWorks swApp, string name, IEnumerable<swPropertyManagerPageOptions_e> optionsE )
        {
            SwApp = swApp;
            int options = optionsE.Aggregate(0,(acc,v)=>(int)v | acc);
            int errors = 0;
            Page = (IPropertyManagerPage2)SwApp.CreatePropertyManagerPage(name, options, this, ref errors);
            if (Page != null && errors == (int) swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
            }
            else
            {
                throw new Exception("Unable to Create PMP");
            }
        }

        private bool ControlsAdded = false;
        public void Show()
        {
            if(!ControlsAdded)
                AddControls();
            ControlsAdded = true;
            Page?.Show();
        }

        protected abstract void AddControls();


        public IPropertyManagerPage2 Page { get; }

        public virtual void AfterActivation()
        {
        }

        public virtual void OnClose(int reason)
        {
        }

        public virtual void AfterClose()
        {
        }

        public virtual bool OnHelp()
        {
            return true;
        }

        public virtual bool OnPreviousPage()
        {
            return true;
        }

        public virtual bool OnNextPage()
        {
            return true;
        }

        public virtual bool OnPreview()
        {
            return true;
        }

        public virtual void OnWhatsNew()
        {
        }

        public virtual void OnUndo()
        {
        }

        public virtual void OnRedo()
        {
        }

        public virtual bool OnTabClicked(int id)
        {
            return true;
        }

        public virtual void OnGroupExpand(int id, bool expanded)
        {
        }

        public virtual void OnGroupCheck(int id, bool Checked)
        {
        }

        public virtual void OnCheckboxCheck(int id, bool Checked)
        {
        }

        public virtual void OnOptionCheck(int id)
        {
        }

        public virtual void OnButtonPress(int id)
        {
        }

        public virtual void OnTextboxChanged(int id, string text)
        {
        }

        public virtual void OnNumberboxChanged(int id, double value)
        {
        }

        public virtual void OnComboboxEditChanged(int id, string text)
        {
        }

        public virtual void OnComboboxSelectionChanged(int id, int item)
        {
        }

        public virtual void OnListboxSelectionChanged(int id, int item)
        {
        }

        public virtual void OnSelectionboxFocusChanged(int id)
        {
        }

        public virtual void OnSelectionboxListChanged(int id, int count)
        {
        }

        public virtual void OnSelectionboxCalloutCreated(int id)
        {
        }

        public virtual void OnSelectionboxCalloutDestroyed(int id)
        {
        }

        public virtual bool OnSubmitSelection(int id, object selection, int selType, ref string itemText)
        {
            return true;
        }

        public virtual int OnActiveXControlCreated(int id, bool status)
        {
            return -1;
        }

        public virtual void OnSliderPositionChanged(int id, double value)
        {
        }

        public virtual void OnSliderTrackingCompleted(int id, double value)
        {
        }

        public virtual bool OnKeystroke(int wparam, int message, int lparam, int id)
        {
            return true;
        }

        public virtual void OnPopupMenuItem(int id)
        {
        }

        public virtual void OnPopupMenuItemUpdate(int id, ref int retval)
        {
        }

        public virtual void OnGainedFocus(int id)
        {
        }

        public virtual void OnLostFocus(int id)
        {
        }

        public virtual int OnWindowFromHandleControlCreated(int id, bool status)
        {
            return 0;
        }

        public virtual void OnListboxRMBUp(int id, int posX, int posY)
        {
        }

        public virtual void OnNumberBoxTrackingCompleted(int id, double value)
        {
        }
    }
}