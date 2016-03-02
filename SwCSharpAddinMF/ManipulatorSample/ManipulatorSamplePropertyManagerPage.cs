using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

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
                .SubscribeDisposable((body, yielder) =>
                {
                    if (body == null)
                        return;

                    var newbody = (IBody2) body.Copy();
                    var handler = new SampleManipulatorHandler(newbody, (IMathUtility)SwApp.GetMathUtility(), ModelDoc);
                    var manipulator = ModelDoc.ModelViewManager.CreateManipulator((int)swManipulatorType_e.swTriadManipulator, handler);
                    var spec = (ITriadManipulator)manipulator.GetSpecificManipulator();
                    var box = body.GetBodyBoxTs();
                    spec.Origin = (MathPoint) ((IMathUtility)SwApp.GetMathUtility()).CreatePoint(box.Center);
                    manipulator.Show(ModelDoc);

                    yielder(Disposable.Create(manipulator.Remove));
                    yielder(newbody.DisplayUndoable(ModelDoc));
                    yielder(body.HideBodyUndoable());


                });
        }

        public override void AfterActivation()
        {
            base.AfterActivation();

        }
    }

    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class SampleManipulatorHandler : ManipulatorHandler
    {
        private readonly IMathUtility _Math;
        private readonly IModelDoc2 _ModelDoc;
        public IBody2 Body { get; set; }

        public SampleManipulatorHandler(IBody2 body, IMathUtility math, IModelDoc2 modelDoc)
        {
            _Math = math;
            _ModelDoc = modelDoc;
            Body = body;
        }

        public override bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double Value)
        {
            Console.WriteLine($"{handleIndex} - {Value}");
            var xAxis = (MathVector)_Math.CreateVector(new[] {1, 0, 0});
            var yAxis = (MathVector)_Math.CreateVector(new[] {0, 1, 0});
            var zAxis = (MathVector)_Math.CreateVector(new[] {0, 0, 1});
            var value = Value;
            var translate =(MathVector)_Math.CreateVector(new[] {handleIndex == 1 ? value : 0, handleIndex == 2 ? value : 0, handleIndex == 3 ? value : 0});
            var transform = _Math.ComposeTransform(xAxis, yAxis, zAxis, translate,1);
            if(!Body.ApplyTransform(transform))
                throw new Exception("Unable to shift");
            //Body.Hide(_ModelDoc);
            //Body.DisplayTs(_ModelDoc, Color.Green, swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            IModelView view = ((IModelView)_ModelDoc.ActiveView);
            //view.EnableGraphicsUpdate = (true);
            //view.UpdateAllGraphicsLayers = true;
            view.GraphicsRedraw(null);
            
            return true;
        }
    };

    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ManipulatorHandler : SwManipulatorHandler2
    {
        public virtual void OnDirectionFlipped(object pManipulator)
        {
        }

        public virtual void OnHandleSelected(object pManipulator, int handleIndex)
        {
        }

        public virtual void OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
        }

        public virtual void OnEndDrag(object pManipulator, int handleIndex)
        {
        }

        public virtual void OnEndNoDrag(object pManipulator, int handleIndex)
        {
        }

        public virtual void OnHandleRmbSelected(object pManipulator, int handleIndex)
        {
        }
        public virtual void OnItemSetFocus(object pManipulator, int handleIndex)
        {
        }

        #region cancelable
        public virtual bool OnHandleLmbSelected(object pManipulator)
        {
            return true;
        }

        public virtual bool OnDelete(object pManipulator)
        {
            return true;
        }

        public virtual bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double Value)
        {
            return true;
        }

        public virtual bool OnStringValueChanged(object pManipulator, int handleIndex, ref string Value)
        {
            return true;
        }
#endregion

    }


    public class ManipulatorSampleDatabase : MacroFeatureDataBase
    {
    }
}
