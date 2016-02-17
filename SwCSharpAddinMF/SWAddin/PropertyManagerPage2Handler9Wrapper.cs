using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF.SWAddin
{

    /// <summary>
    /// Passing objects that use generics as IDispatch during COM interop causes a invalid cast exception.
    /// This is a proxy for implementations of IPropertyManagerPage2Handler9. It
    /// seems that passing an instance of this interface that uses generics somewhere in the inheritence
    /// chain causes COM to bork.
    /// 
    /// See: http://stackoverflow.com/questions/35438895/passing-an-object-in-c-sharp-that-inherits-from-a-generic-causes-a-cast-exceptio/35449486#35449486
    /// 
    /// The solution is just to create a proxy for the interface that wraps the original instance. There
    /// are probably some nuget packages around that can do this automagically but I just used resharper
    /// to generate the below code.
    /// </summary>
    public class PropertyManagerPage2Handler9Wrapper : IPropertyManagerPage2Handler9
    {
        private readonly IPropertyManagerPage2Handler9 _Implementation;

        public PropertyManagerPage2Handler9Wrapper(IPropertyManagerPage2Handler9 implementation)
        {
            _Implementation = implementation;
        }

        public void AfterActivation()
        {
            _Implementation.AfterActivation();
        }

        public void OnClose(int reason)
        {
            _Implementation.OnClose(reason);
        }

        public void AfterClose()
        {
            _Implementation.AfterClose();
        }

        public bool OnHelp()
        {
            return _Implementation.OnHelp();
        }

        public bool OnPreviousPage()
        {
            return _Implementation.OnPreviousPage();
        }

        public bool OnNextPage()
        {
            return _Implementation.OnNextPage();
        }

        public bool OnPreview()
        {
            return _Implementation.OnPreview();
        }

        public void OnWhatsNew()
        {
            _Implementation.OnWhatsNew();
        }

        public void OnUndo()
        {
            _Implementation.OnUndo();
        }

        public void OnRedo()
        {
            _Implementation.OnRedo();
        }

        public bool OnTabClicked(int id)
        {
            return _Implementation.OnTabClicked(id);
        }

        public void OnGroupExpand(int id, bool expanded)
        {
            _Implementation.OnGroupExpand(id, expanded);
        }

        public void OnGroupCheck(int id, bool Checked)
        {
            _Implementation.OnGroupCheck(id, Checked);
        }

        public void OnCheckboxCheck(int id, bool Checked)
        {
            _Implementation.OnCheckboxCheck(id, Checked);
        }

        public void OnOptionCheck(int id)
        {
            _Implementation.OnOptionCheck(id);
        }

        public void OnButtonPress(int id)
        {
            _Implementation.OnButtonPress(id);
        }

        public void OnTextboxChanged(int id, string text)
        {
            _Implementation.OnTextboxChanged(id, text);
        }

        public void OnNumberboxChanged(int id, double value)
        {
            _Implementation.OnNumberboxChanged(id, value);
        }

        public void OnComboboxEditChanged(int id, string text)
        {
            _Implementation.OnComboboxEditChanged(id, text);
        }

        public void OnComboboxSelectionChanged(int id, int item)
        {
            _Implementation.OnComboboxSelectionChanged(id, item);
        }

        public void OnListboxSelectionChanged(int id, int item)
        {
            _Implementation.OnListboxSelectionChanged(id, item);
        }

        public void OnSelectionboxFocusChanged(int id)
        {
            _Implementation.OnSelectionboxFocusChanged(id);
        }

        public void OnSelectionboxListChanged(int id, int count)
        {
            _Implementation.OnSelectionboxListChanged(id, count);
        }

        public void OnSelectionboxCalloutCreated(int id)
        {
            _Implementation.OnSelectionboxCalloutCreated(id);
        }

        public void OnSelectionboxCalloutDestroyed(int id)
        {
            _Implementation.OnSelectionboxCalloutDestroyed(id);
        }

        public bool OnSubmitSelection(int id, object selection, int selType, ref string itemText)
        {
            return _Implementation.OnSubmitSelection(id, selection, selType, ref itemText);
        }

        public int OnActiveXControlCreated(int id, bool status)
        {
            return _Implementation.OnActiveXControlCreated(id, status);
        }

        public void OnSliderPositionChanged(int id, double value)
        {
            _Implementation.OnSliderPositionChanged(id, value);
        }

        public void OnSliderTrackingCompleted(int id, double value)
        {
            _Implementation.OnSliderTrackingCompleted(id, value);
        }

        public bool OnKeystroke(int wparam, int message, int lparam, int id)
        {
            return _Implementation.OnKeystroke(wparam, message, lparam, id);
        }

        public void OnPopupMenuItem(int id)
        {
            _Implementation.OnPopupMenuItem(id);
        }

        public void OnPopupMenuItemUpdate(int id, ref int retval)
        {
            _Implementation.OnPopupMenuItemUpdate(id, ref retval);
        }

        public void OnGainedFocus(int id)
        {
            _Implementation.OnGainedFocus(id);
        }

        public void OnLostFocus(int id)
        {
            _Implementation.OnLostFocus(id);
        }

        public int OnWindowFromHandleControlCreated(int id, bool status)
        {
            return _Implementation.OnWindowFromHandleControlCreated(id, status);
        }

        public void OnListboxRMBUp(int id, int posX, int posY)
        {
            _Implementation.OnListboxRMBUp(id, posX, posY);
        }

        public void OnNumberBoxTrackingCompleted(int id, double value)
        {
            _Implementation.OnNumberBoxTrackingCompleted(id, value);
        }
    }
}