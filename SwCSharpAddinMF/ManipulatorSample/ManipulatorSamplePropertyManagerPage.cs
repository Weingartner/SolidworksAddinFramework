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
                .SubscribeDisposable((body, yielder) =>
                {
                    if (body == null)
                        return;

                    var newbody = (IBody2) body.Copy();
                    var mathUtility = (IMathUtility)SwApp.GetMathUtility();
                    var triad = new TriadManipulatorTs(ModelDoc); 

                    var displayedBody = newbody;
                    yielder(triad.DoubleChangedObservable.Subscribe(o =>
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
                    yielder(triad.EndDragObservable.Subscribe(handle =>
                    {
                        newbody = displayedBody;
                        SetManipulatorPositionToBodyCenter(SwApp,triad,newbody, ModelDoc);
                        GC.Collect();

                    }));
                    SetManipulatorPositionToBodyCenter(SwApp, triad, body, ModelDoc);
                    triad.Show(ModelDoc);
                    yielder(Disposable.Create(triad.Remove));

                    displayedBody.DisplayTs(ModelDoc);
                    yielder(Disposable.Create(() => displayedBody.Hide(ModelDoc)));
                    yielder(body.HideBodyUndoable());


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
