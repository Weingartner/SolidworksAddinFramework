using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Weingartner.Exceptional.Reactive;
using Weingartner.WeinCad.Interfaces;

namespace DemoMacroFeatures.ManipulatorSample
{
    /// <summary>
    /// This page allows a user to select a body in the model and then move a copy of it around with
    /// a manipulator. Demonstrates how to listen for change events on the manipulator. Note we have
    /// wrapped the manipulator interfaces and objects returned by solidworks to provide a nicer
    /// interface.
    /// </summary>
    public class ManipulatorSamplePropertyManagerPage : PropertyManagerPageBase
    {
        private readonly ManipulatorSampleModel _Model = new ManipulatorSampleModel();

        private static readonly List<swPropertyManagerPageOptions_e> Options = new List<swPropertyManagerPageOptions_e>
        {
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
            swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        };


        public ManipulatorSamplePropertyManagerPage(ISldWorks swApp, IModelDoc2 modelDoc) 
            : base("Manipulator Sample", Options, modelDoc)
        {
        }

        protected override void AddSelections()
        {
            ModelDoc.AddSelectionsFromModel(_Model);
        }

        protected override IEnumerable<IDisposable> AddControlsImpl()
        {
            var group = Page.CreateGroup(1, "Sample Group 1", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            yield return CreateLabel(group, "Select object", "Select object");

            yield return CreateSelectionBox(
                group,
                "Select object",
                "Select object",
                swSelectType_e.swSelSOLIDBODIES,
                _Model,
                p => p.Body,
                config =>
                {
                    config.SingleEntityOnly = true;
                    config.AllowMultipleSelectOfSameEntity = false;
                });

            yield return BodySelector()
                .SubscribeDisposable((body, yield) =>
                {
                    // The code here execute every time a new selection is made.
                    // 'yield' is an action that you pass disposable to. These disposables
                    // will be disposed before the next time this callback is activated. Thus
                    // you can use it to "unselect" or destroy any resources made by
                    // the previous selection.

                    // Copy the selected body so we can transform it
                    var newbody = (IBody2) body().Copy();
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

                    SetManipulatorPositionToBodyCenter(SwApp, triad, body(), ModelDoc);

                    // Show the triad and register it to be removed if the selection changes
                    triad.Show(ModelDoc);
                    yield(Disposable.Create(triad.Remove));

                    // Display the copied body and register for the current copied
                    // body to be removed if the selection changes.
                    displayedBody.DisplayTs(ModelDoc);
                    yield(Disposable.Create(() => displayedBody.Hide(ModelDoc)));

                    // Hide the selected body and register it to be shown again
                    // if the selection changes
                    yield(body().HideBodyUndoable());


                }
                , e => e.Show());
        }

        private IObservableExceptional<Func<IBody2>> BodySelector()
        {
            return _Model.WhenAnyValue(p => p.Body)
                .ToObservableExceptional()
                .Where(p => !p.IsEmpty)
                .SelectOptionalFlatten(p => p.GetSingleObject<IBody2>(ModelDoc));
        }

        protected override void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
        }

        public static void SetManipulatorPositionToBodyCenter(ISldWorks sldWorks, TriadManipulatorTs manipulator, IBody2 body, IModelDoc2 model)
        {
            var box = body.GetBodyBoxTs();
            manipulator.Origin = (MathPoint) ((IMathUtility) sldWorks.GetMathUtility()).CreatePoint(box.Center.ToDoubles());
            manipulator.UpdatePosition();
        }

        public static ManipulatorSamplePropertyManagerPage Create(ISldWorks sldWorks)
        {
            return new ManipulatorSamplePropertyManagerPage(sldWorks, (IModelDoc2) sldWorks.ActiveDoc);
        }
    }
}
