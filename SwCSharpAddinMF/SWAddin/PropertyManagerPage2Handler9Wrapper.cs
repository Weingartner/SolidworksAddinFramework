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
        readonly IPropertyManagerPage2Handler9 _Implementation;

        public PropertyManagerPage2Handler9Wrapper(IPropertyManagerPage2Handler9 implementation)
        {
            _Implementation = implementation;
        }

        public void AfterActivation()
        {
            _Implementation.AfterActivation();
        }

        public void OnClose(int Reason)
        {
            _Implementation.OnClose(Reason);
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

        public bool OnTabClicked(int Id)
        {
            return _Implementation.OnTabClicked(Id);
        }

        public void OnGroupExpand(int Id, bool Expanded)
        {
            _Implementation.OnGroupExpand(Id, Expanded);
        }

        public void OnGroupCheck(int Id, bool Checked)
        {
            _Implementation.OnGroupCheck(Id, Checked);
        }

        public void OnCheckboxCheck(int Id, bool Checked)
        {
            _Implementation.OnCheckboxCheck(Id, Checked);
        }

        public void OnOptionCheck(int Id)
        {
            _Implementation.OnOptionCheck(Id);
        }

        public void OnButtonPress(int Id)
        {
            _Implementation.OnButtonPress(Id);
        }

        public void OnTextboxChanged(int Id, string Text)
        {
            _Implementation.OnTextboxChanged(Id, Text);
        }

        public void OnNumberboxChanged(int Id, double Value)
        {
            _Implementation.OnNumberboxChanged(Id, Value);
        }

        public void OnComboboxEditChanged(int Id, string Text)
        {
            _Implementation.OnComboboxEditChanged(Id, Text);
        }

        public void OnComboboxSelectionChanged(int Id, int Item)
        {
            _Implementation.OnComboboxSelectionChanged(Id, Item);
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
            _Implementation.OnListboxSelectionChanged(Id, Item);
        }

        public void OnSelectionboxFocusChanged(int Id)
        {
            _Implementation.OnSelectionboxFocusChanged(Id);
        }

        public void OnSelectionboxListChanged(int Id, int Count)
        {
            _Implementation.OnSelectionboxListChanged(Id, Count);
        }

        public void OnSelectionboxCalloutCreated(int Id)
        {
            _Implementation.OnSelectionboxCalloutCreated(Id);
        }

        public void OnSelectionboxCalloutDestroyed(int Id)
        {
            _Implementation.OnSelectionboxCalloutDestroyed(Id);
        }

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            return _Implementation.OnSubmitSelection(Id, Selection, SelType, ref ItemText);
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            return _Implementation.OnActiveXControlCreated(Id, Status);
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {
            _Implementation.OnSliderPositionChanged(Id, Value);
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
            _Implementation.OnSliderTrackingCompleted(Id, Value);
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return _Implementation.OnKeystroke(Wparam, Message, Lparam, Id);
        }

        public void OnPopupMenuItem(int Id)
        {
            _Implementation.OnPopupMenuItem(Id);
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
            _Implementation.OnPopupMenuItemUpdate(Id, ref retval);
        }

        public void OnGainedFocus(int Id)
        {
            _Implementation.OnGainedFocus(Id);
        }

        public void OnLostFocus(int Id)
        {
            _Implementation.OnLostFocus(Id);
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return _Implementation.OnWindowFromHandleControlCreated(Id, Status);
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
            _Implementation.OnListboxRMBUp(Id, PosX, PosY);
        }

        public void OnNumberBoxTrackingCompleted(int Id, double Value)
        {
            _Implementation.OnNumberBoxTrackingCompleted(Id, Value);
        }
    }
}