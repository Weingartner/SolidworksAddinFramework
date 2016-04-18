using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using Xunit;
using XUnit.Solidworks.Addin;

namespace SolidworksAddinFramework.Spec.opengl
{
    public class MeshRenderSpec : SolidWorksSpec
    {
        private static IModeler Modeler => (IModeler)SwApp.GetModeler();

        private static IMathUtility MathUtility => (IMathUtility)SwApp.GetMathUtility();


        [SolidworksFact]
        public async Task ShouldRenderCylinder()
        {
            var t = new Thread(() =>
            {
                var app = new SldWorks {Visible = true};
                var partDoc = (IPartDoc) app.NewPart();

                var modelDoc = (IModelDoc2) app.ActiveDoc;
                //var selectOptionDefault = (int)swSelectOption_e.swSelectOptionDefault;
                //Assert.True(modelDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 0, null, selectOptionDefault));
                //var plane1 = (RefPlane)modelDoc.FeatureManager.InsertRefPlane((int)swRefPlaneReferenceConstraints_e.swRefPlaneReferenceConstraint_Distance, 0.01, 0, 0, 0, 0);
                //Assert.True(modelDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 0, null, selectOptionDefault));
                //var plane2 = (RefPlane)modelDoc.FeatureManager.InsertRefPlane((int)swRefPlaneReferenceConstraints_e.swRefPlaneReferenceConstraint_Distance, 0.02, 0, 0, 0, 0);

                //Assert.True(modelDoc.Extension.SelectByID2("Plane2", "PLANE", 0, 0, 0, false, 0, null, selectOptionDefault));
                //var circle = modelDoc.SketchManager.CreateCircleByRadius(0, 0, 0, 0.5);
                //// Sketch to extrude
                //Assert.True(modelDoc.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, selectOptionDefault));
                //// Start condition reference
                //Assert.True(modelDoc.Extension.SelectByID2("Plane2", "PLANE", 0.00105020593408751, -0.00195369982668282, 0.0248175428318827, true, 32, null, selectOptionDefault));
                //// End condition reference
                //Assert.True(modelDoc.Extension.SelectByID2("Plane1", "PLANE", 0.0068370744701368, -0.004419862088339, 0.018892268568016, true, 1, null, selectOptionDefault));

                //// Boss extrusion start condition reference is Plane2, and the extrusion end is offset 3 mm from the end condition reference, Plane1
                //var feature = modelDoc.FeatureManager.FeatureExtrusion3(true, false, true, (int)swEndConditions_e.swEndCondOffsetFromSurface, 0, 0.003, 0.003, false, false, false,
                //    false, 0.0174532925199433, 0.0174532925199433, false, false, false, false, true, true, true,
                //    (int)swStartConditions_e.swStartSurface, 0, false);

                var modelView = (ModelView) modelDoc.ActiveView;
                //await Observable.FromEvent<DModelViewEvents_BufferSwapNotifyEventHandler, Unit>
                //    ( h => (() => { h(Unit.Default); return 0; })
                //    , h => modelView.BufferSwapNotify += h
                //    , h => modelView.BufferSwapNotify -= h
                //    )
                //    .FirstAsync();

                //foreach (var body in feature.GetFaces().CastArray<IFace2>().Select(x => (IBody2)x.GetBody()))
                //{
                //    MeshRender.Render(body, App);
                //    body.HideBodyUndoable();
                //}

                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

                modelView.BufferSwapNotify += () =>
                {
                    dispatcher.Invoke(() =>
                    {
                        const int GL_LINES = 1;

                        global::OpenGl.glLineWidth(3);

                        global::OpenGl.glBegin(GL_LINES);
                        global::OpenGl.glVertex3f(0.0F, 0.0F, 0.0F);
                        global::OpenGl.glVertex3f(0.5F, 0.5F, 0.5F);
                        global::OpenGl.glEnd();
                    });
                    return 0;
                };

                //var synchronizationContext = SynchronizationContext.Current;
                //Observable.FromEvent<DModelViewEvents_BufferSwapNotifyEventHandler, Unit>
                //    ( h => (() => { h(Unit.Default); return 0; })
                //    , h => modelView.BufferSwapNotify += h
                //    , h => modelView.BufferSwapNotify -= h
                //    )
                //    .ObserveOn(Scheduler.Immediate)
                //    .Subscribe(_ =>
                //    {
                //        const int GL_LINES = 1;

                //        OpenGl.glLineWidth(10);
                //        OpenGl.glBegin(GL_LINES);
                //        OpenGl.glVertex3f(0.0F, 0.0F, 0.0F);
                //        OpenGl.glVertex3f(100F, 100F, 100F);
                //        OpenGl.glEnd();

                //        //foreach (var body in feature.GetFaces().CastArray<IFace2>().Select(x => (IBody2) x.GetBody()))
                //        //{
                //        //    MeshRender.Render(body, App);
                //        //    body.HideBodyUndoable();
                //        //}
                //    });
                Dispatcher.Run();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        [SolidworksFact]
        public void RenderFaceShouldWork()
        {
            CreatePartDoc(true, modelDoc =>
            {
                var body = Modeler.CreateBox(1, 1, 1);

                //var face = (ISurface)Modeler.CreateToroidalSurface(new[] {0, 0, 0.0}, new[] {0, 1, 0.0}, new[] {0, 0, 1.0}, 1, 0.5);
                var face = body.GetFaces().CastArray<IFace2>().First();
                //var clonedFace = (IFace2) face.
                //var clonedSurface = (ISurface) clonedFace.Copy();
                var faceBody = (IBody2)face.GetBody();
                var faceMesh = new Mesh(faceBody);
                var d1 = faceMesh.DisplayUndoable(modelDoc, Color.Green);

                //return new CompositeDisposable(d, d1);
                return new CompositeDisposable(d1);
            });
        }
    }
}
