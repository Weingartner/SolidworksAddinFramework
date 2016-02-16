using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
        IPropertyManagerPageGroup pageGroup;

        //Controls
        IPropertyManagerPageSelectionbox selection1;
        IPropertyManagerPageNumberbox num1;
        IPropertyManagerPageCombobox combo1;

        private string featureName = "Foo Feature";

        //Control IDs
        public const int group1ID = 0;
        public const int group2ID = 1;

        public const int selection1ID = 8;
        public const int combo1ID = 10;
        #endregion

        private static IEnumerable<swPropertyManagerPageOptions_e> Options => new[]
        {
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        };

        public SamplePropertyPage(SampleMacroFeature macroFeature, StateEnum state) : base(macroFeature.SwApp, "Sample PMP", Options, state)
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
                    
                //this.MacroFeature.SwFeatureData.EditBodies = editBodies;
                if(State==StateEnum.Edit)
                {
                    IBody2[] objects = this.MacroFeature.SelectionMgr.GetSelectedObjects((type,mark)=>type==swSelectType_e.swSelSOLIDBODIES)
                        .Cast<IBody2>()
                        .ToArray();

                    int[] marks =
                        Enumerable.Range(1, objects.Length)
                            .Select(i => this.MacroFeature.SelectionMgr.GetSelectedObjectMark(i))
                            .ToArray();

                    this.MacroFeature.SwFeatureData.SetSelections(objects,marks);
                    Debug.Assert(this.MacroFeature.SwFeatureData.GetSelectionCount()==objects.Length);
                    this.MacroFeature.Write();
                    MacroFeature.ModifyDefinition();
                }
                else
                {
                    IBody2 editBody = null;
                    const int opts = 0;
                    FeatureManagerExtensions.InsertMacroFeature
                        (this.MacroFeature.ModelDoc.FeatureManager, featureName, editBody, opts, this.MacroFeature.Database);
                }
            }else if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel)
            {
                MacroFeature.ReleaseSelectionAccess();
            }
        }


        //Controls are displayed on the page top to bottom in the order 
        //in which they are added to the object.
        protected override  IEnumerable<IDisposable> AddControlsImpl()
        {
            //Add the groups

            pageGroup = Page.CreateGroup(group1ID, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            yield return CreateTextBox(pageGroup, "Param0", "tool tip", ()=> MacroFeature.Database.Param0, v=>MacroFeature.Database.Param0=v);

            yield return CreateNumberBox(pageGroup, "Sample numberbox", "Allows for numerical input", ()=>MacroFeature.Database.Param1,v=>MacroFeature.Database.Param1=v, box =>
            {
                box.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 0.0, 100.0, 0.01, true);
            });

            yield return CreateCheckBox(pageGroup, "Param2", "tool tip", ()=>MacroFeature.Database.Param2, v=>MacroFeature.Database.Param2=v);


            yield return CreateOption(pageGroup, "Option1", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 0);
            yield return CreateOption(pageGroup, "Option2", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 1);
            yield return CreateOption(pageGroup, "Option3", "Radio buttons", () => MacroFeature.Database.Param3 , v => MacroFeature.Database.Param3 = v, 2);


            yield return
                CreateListBox(pageGroup, "Listbox", "List of items", () => MacroFeature.Database.ListItem, v => MacroFeature.Database.ListItem = v,
                    listBox =>
                    {
                        string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                        listBox.Height = 50;
                        listBox.AddItems(items);
                        
                    });

            yield return
                CreateComboBox(pageGroup, "Listbox", "List of items", () => MacroFeature.Database.ComboBoxItem, v => MacroFeature.Database.ComboBoxItem = v,
                    comboBox =>
                    {
                        string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                        comboBox.Height = 50;
                        comboBox.AddItems(items);
                        
                    });


            yield return CreateSelectionBox(pageGroup, "Sample Selection", "Displays features selected in main view",
                (selectionBox, observable) =>
                {
                    if (selectionBox != null)
                    {
                        int[] filter = { (int)swSelectType_e.swSelSOLIDBODIES};
                        selectionBox.Height = 40;
                        selectionBox.SetSelectionFilters(filter);
                        selectionBox.SingleEntityOnly = false;
                    }

                    return observable.Subscribe(v => MacroFeature.Database.SelectedObjects = v);
                });


        }

        #endregion

    }
}
