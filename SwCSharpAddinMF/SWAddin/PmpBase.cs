using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SwCSharpAddinMF.SWAddin
{
    public abstract class PmpBase : IPropertyManagerPage2Handler9
    {
        public readonly ISldWorks SwApp;

        public enum StateEnum {  Insert, Edit }

        public StateEnum State{ get; }

        protected PmpBase(ISldWorks swApp, string name, IEnumerable<swPropertyManagerPageOptions_e> optionsE, StateEnum state)
        {
            SwApp = swApp;
            State = state;
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

        protected void AddControls()
        {
            _Disposables = AddControlsImpl().ToList();
        }

        protected abstract IEnumerable<IDisposable> AddControlsImpl();


        public IPropertyManagerPage2 Page { get; }

        public virtual void AfterActivation()
        {
        }

        public void OnClose(int reason)
        {
            OnClose((swPropertyManagerPageCloseReasons_e)reason);
        }

        protected virtual void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
        }

        public void AfterClose()
        {
            _Disposables?.ForEach(d=>d.Dispose());
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

        #region checkbox
        readonly Subject<Tuple<int,bool>> _CheckBoxChanged = new Subject<Tuple<int,bool>>();
        public virtual void OnCheckboxCheck(int id, bool @checked)
        {
            _CheckBoxChanged.OnNext(Tuple.Create(id,@checked));
        }
        public IObservable<bool> CheckBoxChangedObservable(int id) => _CheckBoxChanged
            .Where(t=>t.Item1==id).Select(t=>t.Item2);
        #endregion


        readonly Subject<int> _OptionChecked = new Subject<int>();
        public virtual void OnOptionCheck(int id)
        {
            _OptionChecked.OnNext(id);
        }

        public IObservable<int> OptionCheckedObservable(int id) => _OptionChecked.Where(i => i == id);

        public virtual void OnButtonPress(int id)
        {
        }

        #region textbox
        public virtual void OnTextboxChanged(int id, string text)
        {
            _TextBoxChanged.OnNext(Tuple.Create(id,text));
        }

        readonly Subject<Tuple<int,string>> _TextBoxChanged = new Subject<Tuple<int,string>>();

        public IObservable<string> TextBoxChangedObservable(int id) => _TextBoxChanged
            .Where(t=>t.Item1==id).Select(t=>t.Item2);
        #endregion

        private List<IDisposable> _Disposables;

        #region numberbox
        readonly Subject<Tuple<int,double>> _NumberBoxChanged = new Subject<Tuple<int,double>>();

        public IObservable<double> NumberBoxChangedObservable(int id) => _NumberBoxChanged
            .Where(t=>t.Item1==id).Select(t=>t.Item2);

        public virtual void OnNumberboxChanged(int id, double value)
        {
            _NumberBoxChanged.OnNext(Tuple.Create(id,value));
        }
        #endregion

        #region combobox
        public virtual void OnComboboxEditChanged(int id, string text)
        {
        }

        readonly Subject<Tuple<int,int>> _ComboBoxSelectionSubject = new Subject<Tuple<int, int>>();
        public virtual void OnComboboxSelectionChanged(int id, int item)
        {
            _ComboBoxSelectionSubject.OnNext(Tuple.Create(id,item));
        }
        public IObservable<int> ComboBoxSelectionObservable(int id) => _ComboBoxSelectionSubject
            .Where(i => i.Item1 == id).Select(t => t.Item2);

        #endregion

        #region listbox

        readonly Subject<Tuple<int,int>> _ListBoxSelectionSubject = new Subject<Tuple<int, int>>();
        private int _NextId = 0;

        public virtual void OnListboxSelectionChanged(int id, int item)
        {
            _ListBoxSelectionSubject.OnNext(Tuple.Create(id,item));
        }

        public IObservable<int> ListBoxSelectionObservable(int id) => _ListBoxSelectionSubject
            .Where(i => i.Item1 == id).Select(t => t.Item2);


        public virtual void OnListboxRMBUp(int id, int posX, int posY)
        {
        }
        #endregion

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

        public bool OnSubmitSelection(int id, object selection, int selType, ref string itemText)
        {
            return OnSubmitSelection(id, selection, (swSelectType_e) selType, ref itemText );
        }

        protected virtual bool OnSubmitSelection(int id, object selection, swSelectType_e selType, ref string itemText)
        {
            _SelectionChangedSubject.OnNext(id);
            return true;
        }
        Subject<int> _SelectionChangedSubject = new Subject<int>();

        public IObservable<Unit> SelectionChangedObservable(int id) => _SelectionChangedSubject.Where(i => id == i).Select(_=>Unit.Default); 

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


        public virtual void OnNumberBoxTrackingCompleted(int id, double value)
        {
        }

        protected IDisposable CreateListBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<int> get, Action<int> set, Action<IPropertyManagerPageListbox> config)
        {
            var id = NextId();
            var list = @group.CreateListBox(id, caption, tip);
            config(list);
            list.CurrentSelection = (short) get();
            return ListBoxSelectionObservable(id).Subscribe(set);
        }
        protected IDisposable CreateComboBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<int> get, Action<int> set, Action<IPropertyManagerPageCombobox> config)
        {
            var id = NextId();
            var comboBox = @group.CreateComboBox(id, caption, tip);
            config(comboBox);
            comboBox.CurrentSelection = (short) get();
            return ComboBoxSelectionObservable(id).Subscribe(set);
        }


        protected IDisposable CreateTextBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<string> get, Action<string> set)
        {
            var id = NextId();
            var text = @group.CreateTextBox(id, caption, tip);
            text.Text = get();
            return TextBoxChangedObservable(id).Subscribe(set);
        }

        protected IDisposable CreateCheckBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<bool> get, Action<bool> set)
        {
            var id = NextId();
            var text = @group.CreateCheckBox(id, caption, tip);
            text.Checked = get();
            return CheckBoxChangedObservable(id).Subscribe(set);
        }

        protected IDisposable CreateNumberBox(IPropertyManagerPageGroup @group, string tip, string caption, Func<double> get, Action<double> set, Action<IPropertyManagerPageNumberbox> config = null)
        {
            var id = NextId();
            var box = @group.CreateNumberBox(id, caption, tip);
            box.Value = get();
            config?.Invoke(box);
            return NumberBoxChangedObservable(id).Subscribe(set);
        }

        protected IDisposable CreateOption<T>(IPropertyManagerPageGroup @group, string tip, string caption, Func<T> get, Action<T> set, T match)
        {
            var id = NextId();
            if (match == null) throw new ArgumentNullException(nameof(match));

            var option = @group.CreateOption(id, tip, caption);
            if (get().Equals(match))
            {
                option.Checked = true;
            }
            return OptionCheckedObservable(id).Subscribe(v=>set(match));
        }

        protected IDisposable CreateSelectionBox(IPropertyManagerPageGroup @group, string tip, string caption,
            Func<IPropertyManagerPageSelectionbox, IObservable<object[]>,  IDisposable> config)
        {
            var id = NextId();
            var box = @group.CreateSelectionBox(id, caption, tip);
            return config(box, SelectionChangedObservable(id).Select(_=>box.GetSelectedItems() as object[]));
            // For the moment we don't have any callbacks / rx stuff to register.
        }
        protected IDisposable CreateSelectionBox(IPropertyManagerPageGroup @group, string tip, string caption,
            Action<IPropertyManagerPageSelectionbox> config)
        {
            var id = NextId();
            var box = @group.CreateSelectionBox(id, caption, tip);
            config(box);
            // For the moment we don't have any callbacks / rx stuff to register.
            return Disposable.Empty;
        }


        private int NextId()
        {
            _NextId++;
            return _NextId;
        }
    }
}