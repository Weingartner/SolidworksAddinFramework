using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiffLib;
using ReactiveUI;
using SolidworksAddinFramework.Reflection;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using Weingartner.Exceptional.Reactive;
using Weingartner.ReactiveCompositeCollections;
using Unit = System.Reactive.Unit;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Base class for all property manager pages. See sample for more info
    /// </summary>
    /// <typeparam name="TMacroFeature">The type of the macro feature this page is designed for</typeparam>
    [ComVisible(false)]
    public abstract class PropertyManagerPageBase : ReactiveObject, IPropertyManagerPage2Handler9, IDisposable
    {
        public readonly ISldWorks SwApp;
        private readonly string _Name;
        private readonly IEnumerable<swPropertyManagerPageOptions_e> _OptionsE;
        private readonly CompositeDisposable _Disposable = new CompositeDisposable();
        private IDisposable _Deselect;

        /// <summary>
        /// We default to true so that in the absense of any explicit validation
        /// the accept button on the property manager page is always on
        /// </summary>
        bool _IsValid = true;
        public bool IsValid 
        {
            get { return _IsValid; }
            set { this.RaiseAndSetIfChanged(ref _IsValid, value); }
        }

        protected BehaviorSubject<Unit> ValidationSubject = new BehaviorSubject<Unit>(Unit.Default);

        protected PropertyManagerPageBase(string name, IEnumerable<swPropertyManagerPageOptions_e> optionsE, ISldWorks swApp, IModelDoc2 modelDoc)
        {

            SwApp = swApp;
            ModelDoc = modelDoc;
            _Name = name;
            _OptionsE = optionsE;
        }

        private readonly Subject<Unit> _ShowSubject = new Subject<Unit>();
        public IObservable<Unit> ShowObservable => _ShowSubject.AsObservable(); 

        /// <summary>
        /// Creates a new SolidWorks property manager page, adds controls, and shows the page.
        /// </summary>
        public virtual void Show()
        {
            using (Disposable.Create(() => _ShowSubject.OnNext(Unit.Default)))
            {
                if (Page != null)
                {
                    Page.Show();
                    return;
                }

                var options = _OptionsE.Aggregate(0, (acc, v) => (int)v | acc);
                var errors = 0;
                _PropertyManagerPage2Handler9Wrapper = new PropertyManagerPage2Handler9Wrapper(this);
                var propertyManagerPage = SwApp.CreatePropertyManagerPage(_Name, options,
                    _PropertyManagerPage2Handler9Wrapper, ref errors);

                Page = (IPropertyManagerPage2)propertyManagerPage;
                if (errors != (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
                {
                    throw new Exception("Unable to Create PMP");
                }
                var selectionMgr = (ISelectionMgr)ModelDoc.SelectionManager;
                _Deselect = selectionMgr.DeselectAllUndoable();
                AddControls();

                Page.Show();

                // Force validation of the page
                ValidationSubject.OnNext(Unit.Default);

                var d  = this.WhenAnyValue(p => p.IsValid)
                    .Subscribe(isValid => Page.EnableButton((int)swPropertyManagerPageButtons_e.swPropertyManagerPageButton_Ok, isValid));

                DisposeOnClose(d);


                AddSelections();
            }
        }

        protected void DisposeOnClose(IDisposable disposable)
        {
            _Disposable.Add(disposable);
        }

        protected abstract void AddSelections();

        private void AddControls()
        {
            _Disposable.Add(AddControlsImpl().ToCompositeDisposable());
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
        public IPropertyManagerPage2 Page { get; private set; }


        private readonly Subject<Unit> _AfterActivation = new Subject<Unit>();
        public IObservable<Unit> AfterActivationObs => _AfterActivation.AsObservable();
        public virtual void AfterActivation()
        {
            _AfterActivation.OnNext(Unit.Default);
        }

        public void OnClose(int reason)
        {
            _Disposable.Clear();
            OnClose((swPropertyManagerPageCloseReasons_e) reason);
            _Deselect.Dispose();
        }

        protected abstract void OnClose(swPropertyManagerPageCloseReasons_e reason);

        private readonly Subject<Unit> _AfterCloseSubject = new Subject<Unit>();
        public IObservable<Unit> AfterCloseObservable => _AfterCloseSubject.AsObservable(); 
        public void AfterClose()
        {
            Page = null;
            _AfterCloseSubject.OnNext(Unit.Default);
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
        private readonly Subject<int> _SelectionChangedSubject = new Subject<int>();
        private PropertyManagerPage2Handler9Wrapper _PropertyManagerPage2Handler9Wrapper;


        private IObservable<Unit> SelectionChangedObservable(int id) => _SelectionChangedSubject.Where(i => id == i).Select(_=>Unit.Default);

        protected IModelDoc2 ModelDoc { get; }

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

        private readonly Subject<Tuple<int, double>> _SliderPositionChangedSubject = new Subject<Tuple<int, double>>();
        private IObservable<int> SliderPositionChangedObservable(int id) =>
            _SliderPositionChangedSubject
            .Where(p => id == p.Item1)
            .Select(p => (int)p.Item2);
        public virtual void OnSliderPositionChanged(int id, double value)
        {
            _SliderPositionChangedSubject.OnNext(Tuple.Create(id, value));
        }

        public virtual void OnSliderTrackingCompleted(int id, double value)
        {
            _SliderPositionChangedSubject.OnNext(Tuple.Create(id, value));
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
            return ControlHolder.Create(@group, list, d);
        }
        protected IDisposable CreateComboBox<T, TModel>(
            IPropertyManagerPageGroup @group,
            string caption,
            string tip,
            ICompositeList<T> items,
            IEqualityComparer<T> itemEqualityComparer,
            Func<T, string> itemToStringFn,
            TModel model,
            Expression<Func<TModel, T>> selectedItemExpression,
            Action<IPropertyManagerPageCombobox> config = null)
        {
            var id = NextId();
            var comboBox = @group.CreateComboBox(id, caption, tip);
            config?.Invoke(comboBox);

            // Sync source collection with SW ComboBox collection
            Action<short, T> insert = (index, element) =>
            {
                var insertIndex = comboBox.InsertItem(index, itemToStringFn(element));
                Debug.Assert(insertIndex != -1, "Item couldn't be inserted");
            };

            Action<short> delete = index =>
            {
                var deleteIndex = comboBox.DeleteItem(index);
                Debug.Assert(deleteIndex != -1, "Item couldn't be deleted");
            };

            var swComboBoxUpdated = new Subject<Unit>();
            var d0 = AfterActivationObs
                .Select(_ => items.ChangesObservable(itemEqualityComparer))
                .Switch()
                .Subscribe(changes =>
                {
                    short index = 0;
                    foreach (var change in changes)
                    {
                        switch (change.Operation)
                        {
                            case DiffOperation.Match:
                                break;
                            case DiffOperation.Insert:
                                insert(index++, change.ElementFromCollection2.Value);
                                break;
                            case DiffOperation.Delete:
                                delete(index);
                                break;
                            case DiffOperation.Replace:
                                delete(index);
                                insert(index++, change.ElementFromCollection2.Value);
                                break;
                            case DiffOperation.Modify:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    swComboBoxUpdated.OnNext(Unit.Default);
                });

            // Sync source to SW selection
            var d1 = swComboBoxUpdated
                .Select(_ => model.WhenAnyValue(selectedItemExpression))
                .Switch()
                .Select(selectedItem => items
                    .Items
                    .Select(list => (short)list.IndexOf(selectedItem))
                )
                .Switch()
                .AssignTo(comboBox, box => box.CurrentSelection);

            // Sync SW to source selection
            var selectedItemProxy = selectedItemExpression.GetProxy(model);
            var d2 = ComboBoxSelectionObservable(id)
                .Select(index => items
                    .Items
                    .Select(list => list[index])
                )
                .Switch()
                .AssignTo(selectedItemProxy, p=> p.Value);
            return ControlHolder.Create(@group, comboBox, d0, d1, d2, swComboBoxUpdated);
        }

        protected IDisposable CreateTextBox(IPropertyManagerPageGroup @group, string caption, string tip, Func<string> get, Action<string> set)
        {
            var id = NextId();
            var text = PropertyManagerGroupExtensions.CreateTextBox(@group, id, caption, tip);
            text.Text = get();
            var d = TextBoxChangedObservable(id).Subscribe(set);
            return ControlHolder.Create(@group, text, d);
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
            return InitControl(@group, box, config, c => c.Value, NumberBoxChangedObservable(id), source, selector);
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
            return InitControl(@group, box, config, c => c.Checked, CheckBoxChangedObservable(id), source, selector);
        }

        private static IDisposable InitControl<T, TContrl, TCtrlProp, TDataProp>(
            IPropertyManagerPageGroup @group,
            TContrl control,
            Func<TContrl, IDisposable> controlConfig,
            Expression<Func<TContrl, TCtrlProp>> ctrlPropSelector,
            IObservable<TCtrlProp> ctrlPropChangeObservable,
            T propParent,
            Expression<Func<T, TDataProp>> propSelector,
            Func<TCtrlProp, TDataProp> controlToDataConversion ,
            Func<TDataProp, TCtrlProp> dataToControlConversion)
        {
            var proxy = propSelector.GetProxy(propParent);
            var ctrlProxy = ctrlPropSelector.GetProxy(control);

            var d5 = controlConfig?.Invoke(control);

            var d2 = propParent
                .WhenAnyValue(propSelector)
                .Select(dataToControlConversion)
                .Subscribe(v => ctrlProxy.Value = v);

            var d1 = ctrlPropChangeObservable
                .Select(controlToDataConversion)
                .Subscribe(v => proxy.Value = v);

            return ControlHolder.Create(@group, control, d1, d2, d5);
        }

        private static IDisposable InitControl<T, TContrl, TProp>(
            IPropertyManagerPageGroup @group,
            TContrl control,
            Func<TContrl, IDisposable> controlConfig,
            Expression<Func<TContrl, TProp>> ctrlPropSelector,
            IObservable<TProp> ctrlPropChangeObservable,
            T propParent,
            Expression<Func<T, TProp>> propSelector)
        {
            return InitControl(@group, control, controlConfig, ctrlPropSelector, ctrlPropChangeObservable, propParent, propSelector, x => x, x => x);
        }

        public IDisposable CreateLabel(IPropertyManagerPageGroup @group, string tip, string caption = null, Func<IPropertyManagerPageLabel, IDisposable> config = null)
        {
            caption = caption ?? tip;
            return CreateLabel(@group, tip, Observable.Return(caption), config);
        }

        private IDisposable CreateLabel(IPropertyManagerPageGroup @group, string tip, IObservable<string> captionObs, Func<IPropertyManagerPageLabel, IDisposable> config)
        {
            var id = NextId();
            var box = @group.CreateLabel(id, string.Empty, tip);
            var d1 = config?.Invoke(box) ?? Disposable.Empty;
            var d0 = captionObs.Subscribe(caption => box.Caption = caption);
            return ControlHolder.Create(@group, box, d0, d1);
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


        protected IDisposable CreateSelectionBox<TModel>(
            IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            swSelectType_e selectType,
            TModel model,
            Expression<Func<TModel, SelectionData>> propertyExpr,
            Action<IPropertyManagerPageSelectionbox> config)
        {
            return CreateSelectionBox(@group, tip, caption, new[] { selectType}, model, propertyExpr, config, () => { });
        }

        protected IDisposable CreateSelectionBox<TModel>(
            IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            swSelectType_e[] selectType,
            TModel model,
            Expression<Func<TModel, SelectionData>> propertyExpr,
            Action<IPropertyManagerPageSelectionbox> config)
        {
            return CreateSelectionBox(@group, tip, caption,  selectType, model, propertyExpr, config, () => { });
        }

        protected IDisposable CreateSelectionBox<TModel>
            (
            IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            swSelectType_e selectType,
            TModel model,
            Expression<Func<TModel, SelectionData>> propertyExpr,
            Action<IPropertyManagerPageSelectionbox> config,
            Action onFocus)
        {
            return CreateSelectionBox(@group, tip, caption, new[] {selectType}, model, propertyExpr, config, onFocus);
        }

        protected IDisposable CreateSelectionBox<TModel>(
            IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            swSelectType_e[] selectType,
            TModel model,
            Expression<Func<TModel, SelectionData>> propertyExpr,
            Action<IPropertyManagerPageSelectionbox> config,
            Action onFocus)
        {
            var d = new CompositeDisposable();

            var id = NextId();
            var box = @group.CreateSelectionBox(id, caption, tip);
            config(box);
            box.SetSelectionFilters(selectType);

            SelectionBoxFocusChangedObservable(id)
                .Subscribe(_ => onFocus())
                .DisposeWith(d);

            var canSetSelectionSubject = new BehaviorSubject<bool>(true).DisposeWith(d);
            Func<IDisposable> startTransaction = () =>
            {
                canSetSelectionSubject.OnNext(false);
                return Disposable.Create(() => canSetSelectionSubject.OnNext(true));
            };
            BindFromSource(model, propertyExpr, startTransaction)
                .DisposeWith(d);

            var inTransactionObservable = canSetSelectionSubject.AsObservable();
            BindFromTarget(selectType, model, propertyExpr, id, inTransactionObservable, box)
                .DisposeWith(d);

            return ControlHolder.Create(@group, box, d);
        }

        private IDisposable BindFromSource<TModel>(TModel model, Expression<Func<TModel, SelectionData>> propertyExpr, Func<IDisposable> startTransaction)
        {
            return model.WhenAnyValue(propertyExpr)
                .Buffer(2, 1)
                .Where(b => b.Count == 2)
                .Select(b =>
                {
                    var previous = b[0].ObjectIds.ToList();
                    var current = b[1].ObjectIds.ToList();
                    var sections = Diff.CalculateSections(previous, current);
                    var alignedElements = Diff
                        .AlignElements(previous, current, sections,
                            new BasicInsertDeleteDiffElementAligner<SelectionData.ObjectId>())
                        .ToList();
                    var oldObjectIds = alignedElements
                        .Where(e => e.Operation == DiffOperation.Delete || e.Operation == DiffOperation.Replace)
                        .Select(e => e.ElementFromCollection1.Value);
                    
                    var newObjectIds = alignedElements
                        .Where(e => e.Operation == DiffOperation.Insert || e.Operation == DiffOperation.Replace)
                        .Select(e => e.ElementFromCollection2.Value);

                    return new { oldSelectionData = b[0], oldObjectIds, newSelectionData = b[1], newObjectIds };
                })
                .Subscribe(o =>
                {
                    using (startTransaction())
                    {
                        ModelDoc.ClearSelection(o.oldSelectionData.WithObjectIds(o.oldObjectIds));
                        ModelDoc.AddSelection(o.newSelectionData.WithObjectIds(o.newObjectIds));
                    }
                });
        }

        private IDisposable BindFromTarget<TModel>(swSelectType_e[] selectType, TModel model, Expression<Func<TModel, SelectionData>> propertyExpr, int id, IObservable<bool> inTransactionObservable, IPropertyManagerPageSelectionbox box)
        {
            return SelectionChangedObservable(id)
                .CombineLatest(inTransactionObservable, (_, canSetSelection) => canSetSelection)
                .Where(v => v)
                .Subscribe(_ => SetSelection(box, selectType, model, propertyExpr));
        }

        private void SetSelection<TModel>(IPropertyManagerPageSelectionbox box, swSelectType_e[] selectType, TModel model, Expression<Func<TModel, SelectionData>> propertyExpr)
        {
            var selectionManager = (ISelectionMgr) ModelDoc.SelectionManager;

            var selectedItems = selectionManager.GetSelectedObjects((type, mark) => selectType.Any(st => type == st) && box.Mark == mark);
            var expressionChain = ReactiveUI.Reflection.Rewrite(propertyExpr.Body).GetExpressionChain().ToList();
            var newSelection = SelectionData.Create(selectedItems, box.Mark, ModelDoc);
            ReactiveUI.Reflection.TrySetValueToPropertyChain(model, expressionChain, newSelection);
        }

        protected IDisposable CreateButton(IPropertyManagerPageGroup @group, string tip, string caption, Action onClick, Func<IPropertyManagerPageButton,IDisposable> config = null)
        {
            var id = NextId();
            var box = PropertyManagerGroupExtensions.CreateButton(@group, id, caption, tip);
            var d0 = ButtonPressedObservable(id).Subscribe(_ => onClick());
            return ControlHolder.Create(@group, box, d0,config?.Invoke(box) ?? Disposable.Empty);
        }

        protected IDisposable CreateSlider<T, TProp>(
            IPropertyManagerPageGroup @group,
            string tip,
            string caption,
            T source,
            Expression<Func<T, TProp>> selector,
            Func<IPropertyManagerPageSlider, IDisposable> config,
            Func<int, TProp> controlToDataConversion,
            Func<TProp, int> dataToControlConversion)
        {
            var id = NextId();
            var control = group.CreateSlider(id, caption, tip);
            return InitControl(group, control, config, c => c.Position, SliderPositionChangedObservable(id), source, selector, controlToDataConversion, dataToControlConversion);
        }

        protected IDisposable SetupControlEnabling(IPropertyManagerPageControl control, IObservable<bool> enabledObservable)
        {
            return AfterActivationObs
                .Select(_ => enabledObservable)
                .Switch()
                .Subscribe(v => control.Enabled = v);
        }

        public int NextId()
        {
            _NextId++;
            return _NextId;
        }

        public virtual void Dispose()
        {
            Page?.Close(true);
        }


        protected static void ConfigAngleNumberBox(IPropertyManagerPageNumberbox config)
        {
            config.SetRange2((int) swNumberboxUnitType_e.swNumberBox_Angle, -10, 10, true, 0.005, 0.010, 0.001);
            config.DisplayedUnit = (int) swAngleUnit_e.swDEGREES;
        }

        public IDisposable CreatePMPControlFromForm(IPropertyManagerPageGroup @group,
            Form host,
            Action<IPropertyManagerPageWindowFromHandle> config = null) => CreatePMPControlFromForm
                (@group,
                    host,
                    c =>
                    {
                        config?.Invoke(c);
                        return Disposable.Empty;
                    });

        public IDisposable CreatePMPControlFromForm(IPropertyManagerPageGroup @group, Form host, Func<IPropertyManagerPageWindowFromHandle, IDisposable> config = null)
        {
            host.Dock = DockStyle.Fill;
            host.TopLevel = false;


            short controlType = (short) swPropertyManagerPageControlType_e.swControlType_WindowFromHandle;
            short align = 0;
            var options = (int) swAddControlOptions_e.swControlOptions_Enabled |
                          (int) swAddControlOptions_e.swControlOptions_Visible;

            var dotnet3 =
                (IPropertyManagerPageWindowFromHandle)
                    @group.AddControl2
                        (NextId(), controlType, "", align, options, "");

            dotnet3.SetWindowHandlex64(host.Handle.ToInt64());

            var d = config?.Invoke(dotnet3) ?? Disposable.Empty;

            //wrapper.SizeChanged += (sender, args) => host.Size = wrapper.Size; 

            return new ControlHolder(@group, dotnet3, d);
        }

        /// <summary>
        /// Creates a simple label that when the option is some it becomes
        /// visibile and displays the text in red.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="errorObservable"></param>
        /// <returns></returns>
        protected IDisposable CreateErrorLabel(IPropertyManagerPageGroup @group, IObservable<LanguageExt.Option<string>> errorObservable)
        {
            return CreateLabel(@group, "", "",
                label =>
                {
                    var ctrl = (IPropertyManagerPageControl) label;
                    ctrl.TextColor = ColorTranslator.ToWin32(System.Drawing.Color.Red);
                    return errorObservable.Subscribe
                        (errorOpt =>
                        {
                            errorOpt.Match
                                (text =>
                                {
                                    label.Caption = text;
                                    ctrl.Visible = true;

                                },
                                    () =>
                                    {
                                        ctrl.Visible = false;
                                    });
                        });
                });
        }
    }

    #region control reference holding

    /// <summary>
    /// It is neccessary to keep reference to the property manager page controls. If you
    /// lose the reference then the garbage collector may call the finalize method on the
    /// control. The finalize method then will detach all callback or possibly remove
    /// the control completely from the page. 
    /// 
    /// This object just allows the control to be help along with another IDisposable
    /// which will get disposed when the dispose method on this class is called. 
    /// </summary>
    public class ControlHolder : IDisposable
    {
        private readonly List<object> _Objects = new List<object>(); 
        private readonly IDisposable _Disposable = Disposable.Empty;

        public ControlHolder(IPropertyManagerPageGroup pageGroup, object control, IDisposable disposable)
        {
            _Objects.Add(pageGroup);
            _Objects.Add(control);
            _Disposable = disposable;
        }
        public ControlHolder(IPropertyManagerPage2 object0 )
        {
            _Objects.Add(object0);
        }

        public void Dispose()
        {
            _Disposable.Dispose();
        }

        public static IDisposable Create(IPropertyManagerPageGroup @group, object control, params IDisposable[] d)
        {
            return new ControlHolder(group, control, d.ToCompositeDisposable());
        }
        public static IDisposable Create(IPropertyManagerPage2 page )
        {
            return new ControlHolder(page);
        }
    }

    #endregion

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

            var d = ControlHolder.Create(_PageGroup, box, d1, d2);

            _Disposable.Add(d);
        }
    }
}