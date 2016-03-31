using System;
using System.Collections.Generic;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class PropertyManagerGroupExtensions
    {
        public static IPropertyManagerPageGroup
            CreateGroup(this IPropertyManagerPage2 page,
                int id,
                string caption,
                IEnumerable<swAddGroupBoxOptions_e> options
            )
        {
            var optionsShort = (short) CombineToInt(options, v => (short) v);
            return (IPropertyManagerPageGroup) page.AddGroupBox(id, caption, optionsShort);
        }

        public static IPropertyManagerPageTextbox
            CreateTextBox(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null)
        {
            return AddControl<IPropertyManagerPageTextbox>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageCheckbox
            CreateCheckBox(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageCheckbox>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageButton
            CreateButton(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageButton>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageSelectionbox
            CreateSelectionBox(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageSelectionbox>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageLabel
            CreateLabel(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageLabel>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageCombobox
            CreateComboBox(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageCombobox>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageListbox
            CreateListBox(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageListbox>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageNumberbox
            CreateNumberBox(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string tip,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageNumberbox>(@group, id, caption, tip, leftAlign, options);
        }

        public static IPropertyManagerPageOption
            CreateOption(this IPropertyManagerPageGroup group,
                int id,
                string caption,
                string optionGroup,
                swPropertyManagerPageControlLeftAlign_e leftAlign = 0,
                IEnumerable<swAddControlOptions_e> options = null
            )
        {
            return AddControl<IPropertyManagerPageOption>(@group, id, caption, optionGroup, leftAlign, options);
        }

        public static T AddControl<T>(this IPropertyManagerPageGroup @group,
            int id,
            string caption,
            string tip,
            swPropertyManagerPageControlLeftAlign_e leftAlign,
            IEnumerable<swAddControlOptions_e> options)
        {
            swPropertyManagerPageControlType_e typeE;
            switch (typeof (T).Name)
            {
                case nameof(IPropertyManagerPageTextbox):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Textbox;
                    break;
                case nameof(IPropertyManagerPageCheckbox):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Checkbox;
                    break;
                case nameof(IPropertyManagerPageButton):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Button;
                    break;
                case nameof(IPropertyManagerPageSelectionbox):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Selectionbox;
                    break;
                case nameof(IPropertyManagerPageCombobox):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Combobox;
                    break;
                case nameof(IPropertyManagerPageListbox):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Listbox;
                    break;
                case nameof(IPropertyManagerPageNumberbox):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Numberbox;
                    break;
                case nameof(IPropertyManagerPageOption):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Option;
                    break;
                case nameof(IPropertyManagerPageLabel):
                    typeE = swPropertyManagerPageControlType_e.swControlType_Label;
                    break;
                default:
                    throw new ArgumentException($"Cannot handle type{typeof(T).Name}");
            }
            return (T) @group
                .AddControl2(id,
                    (short) typeE,
                    caption,
                    (short) leftAlign,
                    CombineOptions(options),
                    tip);
        }

        private static int CombineOptions(IEnumerable<swAddControlOptions_e> options)
        {
            return CombineToInt(options, v => (int) v);
        }


        private static int CombineToInt<T>(IEnumerable<T> leftAlign, Func<T,int> fn )
        {
            return leftAlign?.Aggregate(0, (acc, v) => acc | fn(v)) 
                ?? ((int)swAddControlOptions_e.swControlOptions_Enabled | (int)swAddControlOptions_e.swControlOptions_Visible);
        }
    }
}
