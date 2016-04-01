using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using Reactive.Bindings;
using ReactiveUI;
using SolidworksAddinFramework.ReactiveProperty;
using SolidworksAddinFramework.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SolidworksAddinFramework
{

    public abstract class MacroFeaturePropertyManagerPageBase<TMacroFeature, TData> : PropertyManagerPageBase
        where TData : MacroFeatureDataBase, new()
        where TMacroFeature : MacroFeatureBase<TMacroFeature,TData>
    {
        protected MacroFeaturePropertyManagerPageBase( string name, IEnumerable<swPropertyManagerPageOptions_e> options,TMacroFeature macroFeature) 
            : base(name, options, macroFeature.SwApp, macroFeature.ModelDoc)
        {
            MacroFeature = macroFeature;
        }

        public TMacroFeature MacroFeature { get; private set; }
        protected override void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
            base.OnClose(reason);
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.

            if (reason ==  swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                MacroFeature.Commit();
            }else if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel)
            {
                MacroFeature.Cancel();
            }
        }
    }

    /// <summary>
    /// Base class for all property manager pages. See sample for more info
    /// </summary>
    /// <typeparam name="TMacroFeature">The type of the macro feature this page is designed for</typeparam>
    [ComVisible(false)]
    public abstract class PropertyManagerPageBase : IPropertyManagerPage2Handler9
    {
        public readonly ISldWorks SwApp;
        private readonly string _Name;
        private readonly IEnumerable<swPropertyManagerPageOptions_e> _OptionsE;


        protected PropertyManagerPageBase(string name, IEnumerable<swPropertyManagerPageOptions_e> optionsE, ISldWorks swApp, IModelDoc2 modelDoc)
        {

            SwApp = swApp;
            ModelDoc = modelDoc;
            _Name = name;
            _OptionsE = optionsE;
        }

        private bool _ControlsAdded = false;

        public void Show()
        {
            var options = _OptionsE.Aggregate(0,(acc,v)=>(int)v | acc);
            var errors = 0;
            _PropertyManagerPage2Handler9Wrapper = new PropertyManagerPage2Handler9Wrapper(this);
            var propertyManagerPage = SwApp.CreatePropertyManagerPage(_Name, options, _PropertyManagerPage2Handler9Wrapper, ref errors);


            Page = (IPropertyManagerPage2)propertyManagerPage;
            if (Page != null && errors == (int) swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
            }
            else
            {
                throw new Exception("Unable to Create PMP");
            }
            if(!_ControlsAdded)
                AddControls();
            _ControlsAdded = true;


            Page?.Show();
        }

        private void AddControls()
        {
            _Disposables = AddControlsImpl().ToList();
        }

        /// <summary>
        /// Implement this method to add all controls to the page. See sample for more info 
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IDisposable> AddControlsImpl();


        /// <summary>
        /// The instance of the real solid works property manager page. You will still have to call some
        /// methods on this. Not all magic is done automatically.
        /// </summary>
        public IPropertyManagerPage2 Page { get; set; }


        private readonly Subject<Unit> _AfterActivation = new Subject<Unit>();
        public IObservable<Unit> AfterActivationObs => _AfterActivation.AsObservable();
        public virtual void AfterActivation()
        {
            _AfterActivation.OnNext(Unit.Default);
        }

        public void OnClose(int reason)
        {
            OnClose((swPropertyManagerPageCloseReasons_e)reason);
        }

        protected virtual void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.

            _Disposables?.ForEach(d=>d.Dispose());
        }

        public void AfterClose()
        {
            Page = null;
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

        private readonly Subject<Tuple<int,bool>> _CheckBoxChanged = new Subject<Tuple<int,bool>>();
        public virtual void OnCheckboxCheck(int id, bool @checked)
        {
            _CheckBoxChanged.OnNext(Tuple.Create(id,@checked));
        }
        public IObservable<bool> CheckBoxChangedObservable(int id) => _CheckBoxChanged
            .Where(t=>t.Item1==id).Select(t=>t.Item2);
        #endregion

        private readonly Subject<int> _OptionChecked = new Subject<int>();
        public virtual void OnOptionCheck(int id)
        {
            _OptionChecked.OnNext(id);
        }

        public IObservable<int> OptionCheckedObservable(int id) => _OptionChecked.DistinctUntilChanged().Where(i => i == id);

        private readonly Subject<int> _ButtonPressed = new Subject<int>();
        public IObservable<int> ButtonPressedObservable(int id) => _ButtonPressed.Where(i => i == id);
        public virtual void OnButtonPress(int id)
        {
            _ButtonPressed.OnNext(id);
        }

        #region textbox
        public virtual void OnTextboxChanged(int id, string text)
        {
            _TextBoxChanged.OnNext(Tuple.Create(id,text));
        }

        private readonly Subject<Tuple<int,string>> _TextBoxChanged = new Subject<Tuple<int,string>>();

        public IObservable<string> TextBoxChangedObservable(int id) => _TextBoxChanged
            .Where(t=>t.Item1==id).Select(t=>t.Item2);
        #endregion

        private List<IDisposable> _Disposables;

        #region numberbox

        private readonly Subject<Tuple<int,double>> _NumberBoxChanged = new Subject<Tuple<int,double>>();

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

        private readonly Subject<Tuple<int,int>> _ComboBoxSelectionSubject = new Subject<Tuple<int, int>>();
        public virtual void OnComboboxSelectionChanged(int id, int item)
        {
            _ComboBoxSelectionSubject.OnNext(Tuple.Create(id,item));
        }
        public IObservable<int> ComboBoxSelectionObservable(int id) => _ComboBoxSelectionSubject
            .Where(i => i.Item1 == id).Select(t => t.Item2);

        #endregion

        #region listbox

        private readonly Subject<Tuple<int,int>> _ListBoxSelectionSubject = new Subject<Tuple<int, int>>();
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

        private readonly Subject<int> _SelectionBoxFocusChangedSubject = new Subject<int>();
        public IObservable<Unit> SelectionBoxFocusChangedObservable(int id) => _SelectionBoxFocusChangedSubject.Where(i => id == i).Select(_=>Unit.Default);
        public void OnSelectionboxFocusChanged(int id)
        {
            _SelectionBoxFocusChangedSubject.OnNext(id);
        }

        #region selection changed observables
        private Subject<int> _SelectionChangedSubject = new Subject<int>();
        private PropertyManagerPage2Handler9Wrapper _PropertyManagerPage2Handler9Wrapper;


        public IObservable<Unit> SelectionChangedObservable(int id) => _SelectionChangedSubject.Where(i => id == i).Select(_=>Unit.Default);
        /// <summary>
        /// Observe when selections change. Does not generate an event on subscription. 
        /// </summary>
        /// <returns></returns>
        public IObservable<int> SelectionChangedObservable() => _SelectionChangedSubject; 

        /// <summary>
        /// An observable of the current selection state.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IObservable<object[]>SelectionChangedObservable(Func<swSelectType_e, int, bool> predicate)
        {
            return _SelectionChangedSubject
                .StartWith(0)
                .Select(_ => ((ISelectionMgr)ModelDoc.SelectionManager).GetSelectedObjects(predicate).ToArray());
        }

        public IModelDoc2 ModelDoc { get; }

        /// <summary>
        /// An observable of the current selection state filtered by mark and type. The generic
        /// type just casts the return objects to that type. Usefull if you know all your selections
        /// are of a single type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IObservable<T[]>SelectionChangedObservable<T>(Func<swSelectType_e, int, bool> predicate)
        {
            return SelectionChangedObservable(predicate).Select(list => list.Cast<T>().ToArray());
        }
        /// <summary>
        /// Shorthand for when you know there will be only a single object selected. T is the type
        /// you expect back, IBody2 for example and the predicate allows you to filter selections
        /// based on type and mark.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IObservable<T>SingleSelectionChangedObservable<T>(Func<swSelectType_e, int, bool> predicate)
        {
            return SelectionChangedObservable(predicate).Select(list => list.Cast<T>().FirstOrDefault()).DistinctUntilChanged();
        }
        #endregion

        public virtual void OnSelectionboxListChanged(int id, int count)
        {
            _SelectionChangedSubject.OnNext(id);
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


        public virtual void OnNumberBoxTrackingCompleted(int id, double value)
        {
        }

        protected IDisposable CreateListBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<int> get, Action<int> set, Action<IPropertyManagerPageListbox> config)
        {
            var id = NextId();
            var list = PropertyManagerGroupExtensions.CreateListBox(@group, id, caption, tip);
            config(list);
            list.CurrentSelection = (short) get();
            var d = ListBoxSelectionObservable(id).Subscribe(set);
            return WrapControlAndDisposable(list, d);
        }
        protected IDisposable CreateComboBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<int> get, Action<int> set, Action<IPropertyManagerPageCombobox> config)
        {
            var id = NextId();
            var comboBox = PropertyManagerGroupExtensions.CreateComboBox(@group, id, caption, tip);
            config(comboBox);
            comboBox.CurrentSelection = (short) get();
            var d = ComboBoxSelectionObservable(id).Subscribe(set);
            return WrapControlAndDisposable(comboBox, d);
        }


        protected IDisposable CreateTextBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<string> get, Action<string> set)
        {
            var id = NextId();
            var text = PropertyManagerGroupExtensions.CreateTextBox(@group, id, caption, tip);
            text.Text = get();
            var d = TextBoxChangedObservable(id).Subscribe(set);
            return WrapControlAndDisposable(text, d);
        }

        protected IDisposable CreateCheckBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<bool> get, Action<bool> set)
        {
            var id = NextId();
            var checkBox = PropertyManagerGroupExtensions.CreateCheckBox(@group, id, caption, tip);
            checkBox.Checked = get();
            var d = CheckBoxChangedObservable(id).Subscribe(set);
            return WrapControlAndDisposable(checkBox, d);
        }

        protected IDisposable CreateNumberBox(IPropertyManagerPageGroup @group, string tip, string caption, Func<double> get, Action<double> set, Action<IPropertyManagerPageNumberbox> config = null)
        {
            var id = NextId();
            var box = PropertyManagerGroupExtensions.CreateNumberBox(@group, id, caption, tip);
            box.Value = get();
            config?.Invoke(box);
            var d = NumberBoxChangedObservable(id).Subscribe(set);
            return WrapControlAndDisposable(box, d);
        }
        protected IDisposable CreateNumberBox(IPropertyManagerPageGroup @group, string tip, string caption, ReactiveProperty<double> prop, Action<IPropertyManagerPageNumberbox> config = null)
        {
            return CreateNumberBox(@group, tip, caption, Observable.Return(prop), config);
        }

        protected IDisposable CreateNumberBox(IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            IObservable<Reactive.Bindings.ReactiveProperty<double>> propObservable,
            Action<IPropertyManagerPageNumberbox> config = null)
        {
            return CreateNumberBox(@group, tip, caption, propObservable,
                Disposify(config));
        }

        public static Func<T,IDisposable> Disposify<T>(Action<T> a)
        {
            return t =>
            {
                a(t);
                return Disposable.Empty;
            };
        }

        /// <summary>
        /// ReactiveUI version of CreateNumberBox
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <param name="tip"></param>
        /// <param name="caption"></param>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        protected IDisposable CreateNumberBox<T>(IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            T source,
            Expression<Func<T,double>> selector,
            Func<IPropertyManagerPageNumberbox, IDisposable> config = null)
        {
            var id = NextId();
            var box = @group.CreateNumberBox(id, caption, tip);
            return InitControl(box, config, c => c.Value, NumberBoxChangedObservable(id), source, selector);
        }

        protected IDisposable CreateCheckBox<T>(IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            T source,
            Expression<Func<T,bool>> selector,
            Func<IPropertyManagerPageCheckbox, IDisposable> config = null,
            bool enable = true)
        {
            var id = NextId();
            var box = @group.CreateCheckBox(id, caption, tip);
            return InitControl(box, config, c => c.Checked, CheckBoxChangedObservable(id), source, selector);
        }

        private IDisposable InitControl<T, TContrl, TProp>
            (TContrl control,
            Func<TContrl, IDisposable> controlConfig,
            Expression<Func<TContrl, TProp>> ctrlPropSelector,
            IObservable<TProp> ctrlPropChangeObservable, T propParent,
            Expression<Func<T, TProp>> propSelector)
        {
            var proxy = propSelector.GetProxy(propParent);
            var ctrlProxy = ctrlPropSelector.GetProxy(control);

            var d5 = controlConfig?.Invoke(control);

            var d2 = propParent
                .WhenAnyValue(propSelector)
                .Subscribe(v => ctrlProxy.Value = v);

            var d1 = ctrlPropChangeObservable.Subscribe(v => proxy.Value = v);

            return WrapControlAndDisposable(control, new CompositeDisposable(d1, d2, d5));
        }


        protected IDisposable CreateNumberBox(IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            IObservable<Reactive.Bindings.ReactiveProperty<double>> propObservable,
            Func<IPropertyManagerPageNumberbox, IDisposable> config = null,
            bool enable = true)
        {
            var id = NextId();
            var box = @group.CreateNumberBox(id, caption, tip);
            var d5 = config?.Invoke(box);

            var d2 = propObservable
                .SubscribeDisposable(prop =>
                {
                    Action<double> set = v => prop.Value = v;
                    var d0 = prop.WhenAnyValue().DistinctUntilChanged().Subscribe(v => box.Value = v);
                    var d1 = NumberBoxChangedObservable(id).Subscribe(set);
                    return (IDisposable) new CompositeDisposable(d0,d1);
                });
            // When calling e.g. `IPropertyManagerPageNumberbox::SetRange2` disabling the control doesn't work before the PMP is activated.
            // That's why we wait for the PMP activation here to deactivate the control.
            var d4 = _AfterActivation.Subscribe(_ => ((IPropertyManagerPageControl) box).Enabled = enable);
            return WrapControlAndDisposable(box, new CompositeDisposable(d2, d4, d5));
        }

        protected IDisposable CreateLabel(IPropertyManagerPageGroup @group, string tip, string caption)
        {
            var id = NextId();
            var box = PropertyManagerGroupExtensions.CreateLabel(@group, id, caption, tip);
            return WrapControlAndDisposable(box);
        }

        /// <summary>
        /// Creates an options group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOption"></typeparam>
        /// <param name="pageGroup">The page group the options group belongs to</param>
        /// <param name="source">The model object</param>
        /// <param name="selector">The binding selector for the property on the model</param>
        /// <param name="builder">A callback which is passed the options group object. You can add options here. </param>
        /// <returns></returns>
        protected IDisposable CreateOptionGroup<T,TOption>
            (IPropertyManagerPageGroup pageGroup,T source, Expression<Func<T,TOption>> selector, Action<OptionGroup<T,TOption>> builder)
        {
            var optionGroup = new OptionGroup<T,TOption>(this, pageGroup, source, selector);
            builder(optionGroup);
            return optionGroup;
        }


        protected IDisposable CreateSelectionBox(IPropertyManagerPageGroup @group, string tip, string caption,
            Action<IPropertyManagerPageSelectionbox> config)
        {
            return CreateSelectionBox(@group, tip, caption, config, () => { });
        }

        protected IDisposable CreateSelectionBox(IPropertyManagerPageGroup @group, string tip, string caption, Action<IPropertyManagerPageSelectionbox> config,
            Action onFocus)
        {
            var id = NextId();
            var box = PropertyManagerGroupExtensions.CreateSelectionBox(@group, id, caption, tip);
            config(box);
            var d0 = SelectionBoxFocusChangedObservable(id).Subscribe(_ => onFocus());
            return WrapControlAndDisposable(box, d0);
        }

        protected IDisposable CreateButton(IPropertyManagerPageGroup @group, string tip, string caption, Action onClick)
        {
            var id = NextId();
            var box = PropertyManagerGroupExtensions.CreateButton(@group, id, caption, tip);
            var d0 = ButtonPressedObservable(id).Subscribe(_ => onClick());
            return WrapControlAndDisposable(box, d0);
        }

        #region control reference holding

        internal IDisposable WrapControlAndDisposable(object control, params IDisposable[] d)
        {
            return new ControlHolder(control, d.ToCompositeDisposable());
        }

        /// <summary>
        /// It is neccessary to keep reference to the property manager page controls. If you
        /// lose the reference then the garbage collector may call the finalize method on the
        /// control. The finalize method then will detach all callback or possibly remove
        /// the control completely from the page. 
        /// 
        /// This object just allows the control to be help along with another IDisposable
        /// which will get disposed when the dispose method on this class is called. 
        /// </summary>
        internal class ControlHolder : IDisposable
        {
            public ControlHolder(object control, IDisposable disposable)
            {
                Control = control;
                Disposable = disposable;
            }

            public object Control { get; }
            public IDisposable Disposable { get; }
            public void Dispose()
            {
                Disposable.Dispose();
            }
        }

        #endregion

        internal int NextId()
        {
            _NextId++;
            return _NextId;
        }
    }

    public class OptionGroup<T,TOption> : IDisposable
    {
        private List<IPropertyManagerPageOption> _Options = new List<IPropertyManagerPageOption>(); 
        IPropertyManagerPageGroup _PageGroup;
        private CompositeDisposable _Disposable = new CompositeDisposable();

        public void Dispose()
        {
            _Disposable.Dispose();
        }

        private PropertyManagerPageBase _Page;
        private readonly T _Source;
        private readonly Expression<Func<T,TOption>> _Selector;

        public OptionGroup(PropertyManagerPageBase page, IPropertyManagerPageGroup pageGroup, T source, Expression<Func<T, TOption>> selector)
        {
            _Page = page;
            _PageGroup = pageGroup;
            _Source = source;
            _Selector = selector;
        }

        public void CreateOption(
            string caption,
            TOption match)
        {

            var id = _Page.NextId();
            var box = _PageGroup.CreateOption(id, caption, caption);
            _Options.Add(box);
            if (_Options.Count == 1)
                box.Style = (int) swPropMgrPageOptionStyle_e.swPropMgrPageOptionStyle_FirstInGroup;

            var proxy = _Selector.GetProxy(_Source);

            var d2 = _Source
                .WhenAnyValue(_Selector)
                .Subscribe(v1 => box.Checked = v1.Equals(match));

            var d1 = _Page.OptionCheckedObservable(id).Subscribe(v2 => proxy.Value = match);

            var d = _Page.WrapControlAndDisposable(box, new CompositeDisposable(d1, d2));

            _Disposable.Add(d);
        }
    }
}