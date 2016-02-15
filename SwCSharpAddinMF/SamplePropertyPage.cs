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
        private IMacroFeatureData _Data;

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
            _Data = (IMacroFeatureData) MacroFeature.SwFeature.GetDefinition();
        }

        #region PMPHandlerBase
        //Implement these methods from the interface
        public override void AfterClose()
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.
            int IndentSize;
            IndentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(IndentSize);
        }

        public override void OnClose(int reason)
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.

            if (reason == (int) swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                MacroFeature.SwFeature.ModifyDefinition(_Data, MacroFeature.ModelDoc, null);
            }else if (reason ==(int) swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel)
            {
                _Data.ReleaseSelectionAccess();
            }
            MacroFeature.ModelDoc.ClearSelection2(true);
        }

        public override bool OnSubmitSelection(int id, object selection, int selType, ref string itemText)
        {
            IBody2[]selections = {(IBody2)selection};
            _Data.EditBodies = selections;
            return true;
        }


        //Controls are displayed on the page top to bottom in the order 
        //in which they are added to the object.
        protected  override void AddControls()
        {
            //Add the groups

            group1 = Page.CreateGroup(group1ID, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            group2 = Page.CreateGroup(group2ID, "Sample Group 2", new [] {swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            //Add the controls to group1


            textbox1 = group1.CreateTextBox(textbox1ID, "Type here", "This is a text box");


            checkbox1 = group1.CreateCheckBox(checkbox1ID, "Sample Checkbox", "This is a sample checkbox");


            option1 = group1.CreateOption(option1ID, "Option1", "Radio Buttons");
            option2 = group1.CreateOption(option2ID, "Option2", "Radio Buttons");
            option3 = group1.CreateOption(option3ID, "Option3", "Radio Buttons");


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

            num1 = group1.CreateNumberBox(num1ID, "Sample numberbox", "Allows for numerical input");
            if (num1 != null)
            {
                num1.Value = 50.0;
                num1.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 0.0, 100.0, 0.01, true);
            }

            combo1 = group2.CreateComboBox(combo1ID, "Sample Combobox", "Combo list");
            if (combo1 != null)
            {
                string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                combo1.AddItems(items);
                combo1.Height = 50;

            }
        }
        #endregion

    }
}
