using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SwCSharpAddinMF.SWAddin;

namespace SwCSharpAddinMF
{
    public class UserPmPage
    {
        //Local Objects
        IPropertyManagerPage2 swPropertyPage = null;
        PMPHandler handler = null;

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

        public UserPmPage(ISldWorks swApp)
        {
            if (swApp == null) throw new ArgumentNullException(nameof(swApp));
            SwApp = swApp;
            CreatePropertyManagerPage();
        }

        public ISldWorks SwApp { get; set; }


        protected void CreatePropertyManagerPage()
        {
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            handler = new PMPHandler(this);
            swPropertyPage = (IPropertyManagerPage2)SwApp.CreatePropertyManagerPage("Sample PMP", options, handler, ref errors);
            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                try
                {
                    AddControls();
                }
                catch (Exception e)
                {
                    SwApp.SendMsgToUser2(e.Message, 0, 0);
                }
            }
        }


        //Controls are displayed on the page top to bottom in the order 
        //in which they are added to the object.
        protected void AddControls()
        {
            //Add the groups

            group1 = swPropertyPage.CreateGroup(group1ID, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            group2 = swPropertyPage.CreateGroup(group2ID, "Sample Group 2", new [] {swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox ,
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

        public void Show()
        {
            swPropertyPage?.Show();
        }
    }
}
