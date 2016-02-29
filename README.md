Solidworks Addin Framework
==========================

Writing Solidworks addins, property manager pages and macro features from scratch is a painful
excercise. The COM API's take and return objects so intellisense in Visual Studio is of little help.

The contained framework wraps up some of the COM apis into a series of friendly base classes


* SwAddinBase			- Base class for solidworks addins
* MacroFeatureBase		- Base class for macro features
* PropertyManagerPageBase	- Base class for property manager pages bound to macro features
* MacroFeatureDataBase		- Base class for data classes to be serialized to macro features.

There is an example project in SwCSharpAddinMF which shows the complete
set of operations for taking an input object and slicing it into two
new solid bodies and returning them to the feature manager.

For example creating property manager pages and serializing them to data is now
very easy.

        protected override  IEnumerable<IDisposable> AddControlsImpl()
        {
            //Add the groups

            _PageGroup = Page.CreateGroup(Group1Id, "Sample Group 1",
			 new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            yield return CreateLabel(_PageGroup, "Alpha", "Alpha");
            yield return CreateNumberBox(_PageGroup, "Alpha", "Alpha",
			 ()=>MacroFeature.Database.Alpha,v=>MacroFeature.Database.Alpha=v,
			 box =>
			    {
				box.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble,
					 0.0, 1.0, 0.01, true);
			    });

            yield return CreateLabel(_PageGroup, "Select solid to split", "Select solid to split");
            yield return CreateSelectionBox(_PageGroup, "Sample Selection", "Displays features selected in main view",
                (selectionBox) =>
                {
                    if (selectionBox != null)
                    {
                        int[] filter = { (int)swSelectType_e.swSelSOLIDBODIES};
                        selectionBox.Height = 40;
                        selectionBox.SetSelectionFilters(filter);
                        selectionBox.SingleEntityOnly = false;
                    }

                });
	}

and the data object with fields marked with an attribute for serialization to the macro feature data.


    public class SampleMacroFeatureDataBase : MacroFeatureDataBase
    {
        public SampleMacroFeatureDataBase(IMacroFeatureData featureData)
        {
            ReadFrom(featureData);
        }
        public SampleMacroFeatureDataBase()
        {
        }


        [MacroFeatureDataField]
        public double Alpha { get; set; } = 0.25;
    }



There are also a growing number of extension methods to wrap the COM api's in
a type safe and LINQ friendly manner.

Please fork and add new capabilities as you require in your projects and
then send us pull requests to share with the community.


Creating a new project
======================

The project file for building Solidworks addins is a bit pernickity. It contains custom MSBuild steps that are fiddly to replicate by hand. The easiest way to create a new project is to create a template. __File -> ExportTemplate__ and select the demo project. Once you have this as a template project you can create another project based on it.

Then you need to change the COM guids. The sample project is defined in SwAddin.cs as 

    [Guid("7612e834-6277-4122-9e8f-675258162910"), ComVisible(true)]
    [SwAddin(
        Description = "SwCSharpAddinMF description",
        Title = "SwCSharpAddinMF",
        LoadAtStartup = true
        )]

Just change the GUID to another GUID. If you have Resharper just type __nguid__ and then __tab__ and you get a new guid.

Then refactor rename all the class names as you wish.



