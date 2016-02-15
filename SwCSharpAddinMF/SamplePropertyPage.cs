using System;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SwCSharpAddinMF.SWAddin;

namespace SwCSharpAddinMF
{
    public class SamplePropertyPage : PmpBase
    {
        public SampleMacroFeature MacroFeature { get; set; }

        #region Property Manager Page Controls
        //Groups
        IPropertyManagerPageGroup group1;
        IPropertyManagerPageGroup group2;

        //Controls
        IPropertyManagerPageTextbox textbox1;
        IPropertyManagerPageCheckbox checkbox1;
        IPropertyManagerPageOption option1;
        IPropertyManagerPageOption option2;
        IPropertyManagerPageOption option3;
        IPropertyManagerPageListbox list1;

        IPropertyManagerPageSelectionbox selection1;
        IPropertyManagerPageNumberbox num1;
        IPropertyManagerPageCombobox combo1;

        //Control IDs
        public const int group1ID = 0;
        public const int group2ID = 1;

        public const int textbox1ID = 2;
        public const int checkbox1ID = 3;
        public const int option1ID = 4;
        public const int option2ID = 5;
        public const int option3ID = 6;
        public const int list1ID = 7;

        public const int selection1ID = 8;
        public const int num1ID = 9;
        public const int combo1ID = 10;
        #endregion

        private static IEnumerable<swPropertyManagerPageOptions_e> Options => new[]
        {
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        };

        public SamplePropertyPage(SampleMacroFeature macroFeature) : base(macroFeature.SwApp, "Sample PMP", Options)
        {
            if (macroFeature == null) throw new ArgumentNullException(nameof(macroFeature));

            MacroFeature = macroFeature;
        }

        #region PMPHandlerBase
        //Implement these methods from the interface

        protected override void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.

            if (reason ==  swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                this.MacroFeature.Write();
                MacroFeature.ModifyDefinition();
            }else if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel)
            {
                MacroFeature.ReleaseSelectionAccess();
            }
            MacroFeature.ModelDoc.ClearSelection2(true);
        }

        protected override bool OnSubmitSelection(int id, object selection, swSelectType_e selType, ref string itemText)
        {
            IBody2[]selections = {(IBody2)selection};
            MacroFeature.SwFeatureData.EditBodies = selections;
            return true;
        }




        //Controls are displayed on the page top to bottom in the order 
        //in which they are added to the object.
        protected override  IEnumerable<IDisposable> AddControlsImpl()
        {
            //Add the groups

            group1 = Page.CreateGroup(group1ID, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});


            group2 = Page.CreateGroup(group2ID, "Sample Group 2", new [] {swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            //Add the controls to group1
            yield return CreateTextBox(textbox1ID, "Param0", "tool tip", ()=> MacroFeature.Database.Param0, v=>MacroFeature.Database.Param0=v);
            yield return CreateCheckBox(checkbox1ID, "Param2", "tool tip", ()=>MacroFeature.Database.Param2, v=>MacroFeature.Database.Param2=v);


            yield return CreateOption(group1, option1ID, "Option1", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 0);
            yield return CreateOption(group1, option2ID, "Option2", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 1);
            yield return CreateOption(group1, option3ID, "Option3", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 2);


            list1 = group1.CreateListBox(list1ID, "Sample Listbox", "List of selectable items");
            if (list1 != null)
            {
                string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                list1.Height = 50;
                list1.AddItems(items);
            }

            selection1 = group1.CreateSelectionBox(selection1ID, "Sample Selection", "Displays features selected in main view");
            if (selection1 != null)
            {
                int[] filter = { (int)swSelectType_e.swSelEDGES, (int)swSelectType_e.swSelVERTICES };
                selection1.Height = 40;
                selection1.SetSelectionFilters(filter);
            }

            yield return CreateNumberBox(num1ID, "Sample numberbox", "Allows for numerical input", ()=>MacroFeature.Database.Param1,v=>MacroFeature.Database.Param1=v, box =>
            {
                box.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 0.0, 100.0, 0.01, true);
            });

            combo1 = group2.CreateComboBox(combo1ID, "Sample Combobox", "Combo list");
            if (combo1 != null)
            {
                string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                combo1.AddItems(items);
                combo1.Height = 50;

            }
        }

        private IDisposable CreateTextBox(int id, string caption, string tip, Func<string> get, Action<string> set)
        {
            var text = group1.CreateTextBox(id, caption, tip);
            text.Text = get();
            return TextBoxChangedObservable(id).Subscribe(set);
        }

        private IDisposable CreateCheckBox(int id, string caption, string tip, Func<bool> get, Action<bool> set)
        {
            var text = group1.CreateCheckBox(id, caption, tip);
            text.Checked = get();
            return CheckBoxChangedObservable(id).Subscribe(set);
        }

        private IDisposable CreateNumberBox(int id, string tip, string caption, Func<double> get, Action<double> set, Action<IPropertyManagerPageNumberbox> config = null)
        {
            var box = group1.CreateNumberBox(id, caption, tip);
            box.Value = get();
            config?.Invoke(box);
            return NumberBoxChangedObservable(id).Subscribe(set);
        }

        private IDisposable CreateOption<T>(IPropertyManagerPageGroup group1, int id, string tip, string caption, Func<T> get, Action<T> set, T match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));

            var option = group1.CreateOption(id, tip, caption);
            if (get().Equals(match))
            {
                option.Checked = true;
            }
            return OptionCheckedObservable(id).Subscribe(v=>set(match));
        }


        #endregion

    }
}
