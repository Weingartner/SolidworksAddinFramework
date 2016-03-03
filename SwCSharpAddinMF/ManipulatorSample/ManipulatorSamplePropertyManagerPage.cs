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
                    var handler = new SampleManipulatorHandler(newbody, (IMathUtility)SwApp.GetMathUtility(), ModelDoc, SwApp);
                    var manipulator = ModelDoc.ModelViewManager.CreateManipulator((int)swManipulatorType_e.swTriadManipulator, handler);
                    SetManipulatorPositionToBodyCenter(SwApp, manipulator, body, ModelDoc);
                    manipulator.Show(ModelDoc);
                    yielder(Disposable.Create(manipulator.Remove));
                    yielder(newbody.DisplayUndoable(ModelDoc));
                    yielder(body.HideBodyUndoable());


                });
        }

        public static void SetManipulatorPositionToBodyCenter(ISldWorks sldWorks, IManipulator manipulator, IBody2 body, IModelDoc2 model)
        {
            var spec = (ITriadManipulator) manipulator.GetSpecificManipulator();
            var box = body.GetBodyBoxTs();
            spec.Origin = (MathPoint) ((IMathUtility) sldWorks.GetMathUtility()).CreatePoint(box.Center);
            spec.UpdatePosition();
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
        private ISldWorks swApp;
        public IBody2 Body { get; set; }
        public IBody2 DisplayedBody { get; set; }

        public SampleManipulatorHandler(IBody2 body, IMathUtility math, IModelDoc2 modelDoc, ISldWorks swApp)
        {
            _Math = math;
            _ModelDoc = modelDoc;
            this.swApp = swApp;
            Body = body;
            DisplayedBody = (IBody2)Body.Copy();
        }

        public override bool OnDoubleValueChanged(object pManipulator, int handleIndexInt, ref double value)
        {
            //var handleIndex = (swTriadManipulatorControlPoints_e) handleIndexInt;
            //var transform = CreateTranslationTransform(value, handleIndex);

            //DisplayedBody.Hide(_ModelDoc);
            //DisplayedBody = (IBody2) Body.Copy();
            //GC.Collect();
            //if(!DisplayedBody.ApplyTransform(transform))
            //    throw new Exception("Unable to shift");
            //DisplayedBody.DisplayTs(_ModelDoc, Color.Green, swTempBodySelectOptions_e.swTempBodySelectOptionNone);

            //IModelView view = ((IModelView)_ModelDoc.ActiveView);
            //////view.EnableGraphicsUpdate = (true);
            //////view.UpdateAllGraphicsLayers = true;
            //view.GraphicsRedraw(null);
            
            return true;
        }


        private MathVector CreateTranslationVector(swTriadManipulatorControlPoints_e handleIndex, double value)
        {
            return (MathVector)_Math.CreateVector(new[] {
                handleIndex == swTriadManipulatorControlPoints_e.swTriadManipulatorXAxis ? value : 0,
                handleIndex == swTriadManipulatorControlPoints_e.swTriadManipulatorYAxis ? value : 0,
                handleIndex == swTriadManipulatorControlPoints_e.swTriadManipulatorZAxis ? value : 0});
        }

        public override void OnEndDrag(object pManipulator, int handleIndex)
        {
            IManipulator m = (IManipulator) pManipulator;
            Body = DisplayedBody;
            ManipulatorSamplePropertyManagerPage.SetManipulatorPositionToBodyCenter(swApp,m,Body, _ModelDoc);
        }

        public override void OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
            var m = (IManipulator) pManipulator;
            var t = (ITriadManipulator) m.GetSpecificManipulator();

            var p = (IMathPoint) newPosMathPt;

            var modelView = ((IModelView) _ModelDoc.ActiveView);
            var mathUtility = _Math;
            var viewVector = ViewVector(modelView, mathUtility, p);

            var translation = t.Project((swTriadManipulatorControlPoints_e)handleIndex, p, viewVector, _Math);


            DisplayedBody.Hide(_ModelDoc);
            DisplayedBody = (IBody2)Body.Copy();
            GC.Collect();

            var transform = _Math.ComposeTransform(t.XAxis, t.YAxis, t.ZAxis, translation, 1);
            if (!DisplayedBody.ApplyTransform(transform))
                throw new Exception("Unable to shift");
            DisplayedBody.DisplayTs(_ModelDoc, Color.Green, swTempBodySelectOptions_e.swTempBodySelectOptionNone);

            IModelView view = modelView;
            ////view.EnableGraphicsUpdate = (true);
            ////view.UpdateAllGraphicsLayers = true;
            view.GraphicsRedraw(null);

        }


        /// <summary>
        /// Find the vector in model space from the point to the viewers eye.
        /// </summary>
        /// <param name="modelView"></param>
        /// <param name="mathUtility"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static MathVector ViewVector(IModelView modelView, IMathUtility mathUtility, IMathPoint p)
        {
            var world2screen = modelView.Transform;
            var pScreen = (MathPoint) p.MultiplyTransform(world2screen);
            var vv = (IMathVector) mathUtility.CreateVector(new[] {0.0, 0, 1});
            var pScreenUp = (MathPoint) pScreen.AddVector(vv);
            var pWorldDelta = (MathPoint) pScreenUp.MultiplyTransform((MathTransform) world2screen.Inverse());
            var viewVector = (MathVector) p.Subtract(pWorldDelta);
            return viewVector;
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

        public virtual bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double value)
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
