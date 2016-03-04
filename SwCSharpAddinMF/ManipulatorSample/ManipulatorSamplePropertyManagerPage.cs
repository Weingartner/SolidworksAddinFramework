using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SwCSharpAddinMF.ManipulatorSample
{
    /// <summary>
    /// This page allows a user to select a body in the model and then move a copy of it around with
    /// a manipulator. Demonstrates how to listen for change events on the manipulator. Note we have
    /// wrapped the manipulator interfaces and objects returned by solidworks to provide a nicer
    /// interface.
    /// </summary>
    public class ManipulatorSamplePropertyManagerPage : PropertyManagerPageBase<ManipulatorSampleDatabase>
    {
        private IPropertyManagerPageGroup _PageGroup;
        private int Group1Id = 1;

        static List<swPropertyManagerPageOptions_e> options = new List<swPropertyManagerPageOptions_e>()
        {
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        };


        public ManipulatorSamplePropertyManagerPage(ISldWorks swApp, IModelDoc2 modelDoc) 
            : base("Maniuplator Sample", options, swApp, modelDoc)
        {
        }

        protected override IEnumerable<IDisposable> AddControlsImpl()
        {
            _PageGroup = Page.CreateGroup(Group1Id, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            yield return CreateLabel(_PageGroup, "Select object", "Select object");

            yield return CreateSelectionBox(_PageGroup, "Select object", "Select object", config =>
            {
                config.SingleEntityOnly = true;
                config.SetSelectionFilters(new [] {swSelectType_e.swSelSOLIDBODIES});
                config.AllowMultipleSelectOfSameEntity = false;
            });

            yield return SingleSelectionChangedObservable<IBody2>((type,mark)=>type==swSelectType_e.swSelSOLIDBODIES)
                .Finally(()=>Console.WriteLine("Finally"))
                .SubscribeDisposable((body, yield) =>
                {
                    // The code here execute every time a new selection is made.
                    // 'yield' is an action that you pass disposable to. These disposables
                    // will get run before the next time this callback is activated. Thus
                    // you can use it to "unselect" or destroy any resources made by
                    // the previous selection.

                    if (body == null)
                        return;

                    // Copy the selected body so we can transform it
                    var newbody = (IBody2) body.Copy();
                    var mathUtility = (IMathUtility)SwApp.GetMathUtility();

                    // Create our triad. This is a custom class to make working with triads easier
                    var triad = new TriadManipulatorTs(ModelDoc); 

                    var displayedBody = newbody;

                    // Listen for changes to the axis. The subscribe callback
                    // must accept a Tuple<swTriadManipulatorControPoints_e, double> which
                    // lets you know which control point was changed and what it's
                    // current value is.
                    yield(triad.DoubleChangedObservable.Subscribe(o =>
                    {
                        var handleIndex = o.Item1;
                        var transform = triad.CreateTranslationTransform(handleIndex,mathUtility,o.Item2);

                        displayedBody.Hide(ModelDoc);
                        displayedBody = (IBody2)newbody.Copy();
                        if (!displayedBody.ApplyTransform(transform))
                            throw new Exception("Unable to shift");

                        displayedBody.DisplayTs(ModelDoc);

                        ((IModelView)ModelDoc.ActiveView).GraphicsRedraw(null);

                    }));

                    // Listen for end drag so we can move the triad to the
                    // new position.
                    yield(triad.EndDragObservable.Subscribe(handle =>
                    {
                        newbody = displayedBody;
                        SetManipulatorPositionToBodyCenter(SwApp,triad,newbody, ModelDoc);
                        GC.Collect();

                    }));

                    SetManipulatorPositionToBodyCenter(SwApp, triad, body, ModelDoc);

                    // Show the triad and register it to be removed if the selection changes
                    triad.Show(ModelDoc);
                    yield(Disposable.Create(triad.Remove));

                    // Display the copied body and register for the current copied
                    // body to be removed if the selection changes.
                    displayedBody.DisplayTs(ModelDoc);
                    yield(Disposable.Create(() => displayedBody.Hide(ModelDoc)));

                    // Hide the selected body and register it to be shown again
                    // if the selection changes
                    yield(body.HideBodyUndoable());


                });
        }

        public static void SetManipulatorPositionToBodyCenter(ISldWorks sldWorks, TriadManipulatorTs manipulator, IBody2 body, IModelDoc2 model)
        {
            var box = body.GetBodyBoxTs();
            manipulator.Origin = (MathPoint) ((IMathUtility) sldWorks.GetMathUtility()).CreatePoint(box.Center);
            manipulator.UpdatePosition();
        }

        private static ManipulatorSamplePropertyManagerPage _ManipulatorSamplePropertyManagerPage;
        public static ManipulatorSamplePropertyManagerPage Create(ISldWorks sldWorks)
        {
            _ManipulatorSamplePropertyManagerPage = new ManipulatorSamplePropertyManagerPage(sldWorks, (IModelDoc2) sldWorks.ActiveDoc);
            return _ManipulatorSamplePropertyManagerPage;
        }
    }


    public class ManipulatorSampleDatabase : MacroFeatureDataBase
    {
    }
}
