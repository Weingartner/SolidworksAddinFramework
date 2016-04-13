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

The project file for building Solidworks addins is a bit persnickity. It contains custom MSBuild steps that are fiddly to replicate by hand. The easiest way to create a new project is to create a template. __File -> ExportTemplate__ and select the __DemoMacroFeature__ project. Once you have this as a template project you can create another project based on it.

Then you need to change the COM guids. The sample project is defined in SwAddin.cs as 

    [Guid("7612e834-6277-4122-9e8f-675258162910"), ComVisible(true)]
    [SwAddin(
        Description = "SwCSharpAddinMF description",
        Title = "SwCSharpAddinMF",
        LoadAtStartup = true
        )]

Just change the GUID to another GUID. If you have Resharper just type __nguid__ and then __tab__ and you get a new guid.

Then refactor rename all the class names as you wish.

Strong naming and plugin robustness in the face of confliciting DLL's
=====================================================================
You should / must strong name your dll's if you wish to create a solidworks addin. The reason for this is that you or your client
may install another plugin that uses different versions of the same libraries that your plugin uses. This leads to a disaster if
solidworks can't figure out which dll to choose in the face of multiple options.

The solution is to strong name every dll that the registered dll may itself reference. There is however a problem that if 
you use nuget packages which are not strong named then your build will fail. We have solved this for you by integrating
a [strongnamer.ps1](./strongnamer.ps1) using a library from [brutaldev](https://github.com/brutaldev/StrongNameSigner) 
which collects all the dlls from your build project, signs them and then registers the dll that solidworks needs to know about. 

strongnamer.ps1 should only be called for addin projects, not for support libraries that are not addins themselves.


Sample project
==============

The sample project creates an addin that can split a solid body into two pieces. There is one
parameter __alpha__ that determines the position of the split plane. See a video for the results

https://dl.dropboxusercontent.com/s/dwx4h2kbioamtic/2016-02-29_11-11-09.mp4?dl=0


Solidworks API Unit Testing
===========================

Building the unit test solidworks addin engine
----------------------------------------------

We have written XUnit extensions that enable running SolidWorks tests __in process__. If you try and write normal tests by first creating an instance of __ISldWorks__ and then calling methods on it you will get bitten by the fact that COM marshelling out of process is crazy slow. 

Our XUnit addin loads the tests directly into the solidworks process and ensures tests are dispatched on the main thread. Again if you dispatch code on the wrong thread it runs slow or has errors.

The first thing to do is checkout this repo and then build the solution. 

There is a subproject call __XUnit.Solidworks.Addin__. When built it will register a special addin with solidworks. This addin contains the service that our XUnit interprocess extensions will talk to. This project contains solidworks specialization of the generic XUnit extensions we have written for interprocess unit tests. See https://github.com/Weingartner/XUnitRemote.

Writing solidworks unit tests
-----------------------------

There are tests you can look at for inspiration here.

https://github.com/Weingartner/SolidworksAddinFramework/blob/master/SolidworksAddinFramework.Spec/MathUtilitySpec.cs

but the basics are that instead of using

__Theory__ and __Fact__

you use

__SolidworksTheory__ and __SolidworksFact__

Anatomy of a solidworks test
----------------------------

	namespace MyTests
	{
	    // You should inherit from SolidWorksSpec
	    public class SampleMacroFeatureSpec : SolidWorksSpec 
	    {
	
	        // Mark a fact with the following attribute and it will be loaded into the
	        // solidworks process. ( it won't run within VisualStudio )
	        [SolidworksFact]
	        public void ShouldBeAbleToApplySampleMacroFeature()
	        {
	            // SwApp is a member of SolidWorksSpec
	            ISldWorks sw = SwApp;
	            // Create a new document
	            var partDoc = (IPartDoc) SwApp.NewPart();
	  
	            // Now do some solidworks stuff. It will run fast
	            
	        }
	    }
	}

everything else is basically the same.
