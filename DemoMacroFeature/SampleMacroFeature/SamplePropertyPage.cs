using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using ReactiveUI;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using Weingartner.Exceptional.Reactive;

namespace DemoMacroFeatures.SampleMacroFeature
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IPropertyManagerPage2Handler9))]
    public class SamplePropertyPage : MacroFeaturePropertyManagerPageBase<SampleMacroFeature,SampleMacroFeatureDataBase>
    {
        private static IEnumerable<swPropertyManagerPageOptions_e> Options => new[]
        {
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        };

        public SamplePropertyPage(SampleMacroFeature macroFeature)
            : base(Options, macroFeature)
        {
        }

        #region PMPHandlerBase
        //Implement these methods from the interface


        //Controls are displayed on the page top to bottom in the order 
        //in which they are added to the object.
        protected override  IEnumerable<IDisposable> AddControlsImpl()
        {
            var group = Page.CreateGroup(1, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            yield return CreateLabel(group, "Alpha", "Alpha");
            yield return CreateNumberBox(group, "Alpha", "Alpha", MacroFeature.Database, p=>p.Alpha, box =>
            {
                box.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 0.0, 1.0, 0.1, true);
                return Disposable.Empty;
            });

            yield return CreateLabel(group, "Select solid to split", "Select solid to split");
            yield return CreateSelectionBox(
                group,
                "Sample Selection",
                "Displays features selected in main view",
                swSelectType_e.swSelSOLIDBODIES,
                MacroFeature.Database,
                p => p.Body,
                config =>
                {
                    config.Height = 40;
                    config.SingleEntityOnly = true;
                });


            // When the alpha value or the selection changes we want to 
            // show a temporary body with the split in it
            yield return ObservableExceptional
                .CombineLatest
                (
                    MacroFeature.Database.WhenAnyValue(p=>p.Alpha).ToObservableExceptional(),
                    MacroFeature.Database.WhenAnyValue(p => p.Body).ToObservableExceptional().Where(p => !p.IsEmpty),
                    (alpha, selection) => new { alpha, selection }
                )
                .SelectOptionalFlatten(o =>
                {
                    return from bodyFn in o.selection.GetSingleObject<IBody2>(ModelDoc)
                           let newBody = (IBody2) bodyFn().Copy()
                           let splits = SampleMacroFeature.SplitBodies((IModeler)MacroFeature.SwApp.GetModeler(), newBody, MacroFeature.Database)
                           select splits == null ? null : new { body = bodyFn(), splits = splits.ToList() };
                })
                .SubscribeDisposable((v, yield) =>
                {
                    yield(v.body.HideBodyUndoable());
                    yield(v.splits.DisplayBodiesUndoable(MacroFeature.ModelDoc));

                }, e => e.Show());




            /* Some example of other things you can create */

            /*

            yield return CreateLabel(_PageGroup, "Dummy", "Dummy");
            yield return CreateTextBox(_PageGroup, "Param0", "tool tip", ()=> MacroFeature.Database.Param0, v=>MacroFeature.Database.Param0=v);


            yield return CreateLabel(_PageGroup, "Checkbox", "Checkbox");
            yield return CreateCheckBox(_PageGroup, "Param2", "tool tip", ()=>MacroFeature.Database.Param2, v=>MacroFeature.Database.Param2=v);


            yield return CreateLabel(_PageGroup, "Options", "Options");
            yield return CreateOption(_PageGroup, "Option1", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 0);
            yield return CreateOption(_PageGroup, "Option2", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 1);
            yield return CreateOption(_PageGroup, "Option3", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 2);


            yield return
                CreateListBox(_PageGroup, "Listbox", "List of items", () => MacroFeature.Database.ListItem, v => MacroFeature.Database.ListItem = v,
                    listBox =>
                    {
                        string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                        listBox.Height = 50;
                        listBox.AddItems(items);
                        
                    });

            yield return
                CreateComboBox(_PageGroup, "Listbox", "List of items", () => MacroFeature.Database.ComboBoxItem, v => MacroFeature.Database.ComboBoxItem = v,
                    comboBox =>
                    {
                        string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                        comboBox.Height = 50;
                        comboBox.AddItems(items);
                        
                    });

            */




        }

        #endregion

    }
}
