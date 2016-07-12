using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using XUnit.Solidworks.Addin;
using SwBSplineParamsExtensions = SolidworksAddinFramework.Geometry.SwBSplineParamsExtensions;



namespace SolidworksAddinFramework.Spec
{
    public class BSplineParamsSpec : SolidWorksSpec
    {
        [SolidworksFact]
        public void ForwardAndBackSplineConversionShouldWork()
        {
            CreatePartDoc(modelDoc =>
            {
                var trimCurve = Modeler.CreateTrimmedLine(new Vector3(-0.1f,-0.45f,-7.8f),new Vector3(1.3f,2.7f,3.9f));

                var parameters = trimCurve.ToBSpline3D(false);
                var swCurve = parameters.ToCurve();
                var parameters2 = swCurve.ToBSpline3D(false);

                parameters2.Should().Be(parameters);

                //#####################################
                var d0 = swCurve.CreateWireBody().DisplayUndoable(modelDoc, Color.Blue);
                var d1 = trimCurve.CreateWireBody().DisplayUndoable(modelDoc, Color.Red);
                return new CompositeDisposable(d0,d1);
            });
        }

        [SolidworksFact]
        public void ForwardAndBackArcSplineConversionShouldWork()
        {
            CreatePartDoc(modelDoc =>
            {
                var trimCurve = (ICurve) Modeler.CreateTrimmedArc
                    ( Vector3.Zero
                    , Vector3.UnitZ, 10 * Vector3.UnitX 
                    , -10 * Vector3.UnitX
                    );

                var parameters = trimCurve.ToBSpline3D(false);
                var swCurve = parameters.ToCurve();
                var parameters2 = swCurve.ToBSpline3D(false);

                parameters2.Should().Be(parameters);

                //#####################################
                var d0 = swCurve.CreateWireBody().DisplayUndoable(modelDoc, Color.Blue);
                var d1 = trimCurve.CreateWireBody().DisplayUndoable(modelDoc, Color.Red);
                return new CompositeDisposable(d0,d1);
            });
        }

        [SolidworksFact]
        public void CanRoundTripAFace()
        {
            CreatePartDoc((modelDoc,yielder) =>
            {
                var box = Modeler.CreateBox(Vector3.Zero, Vector3.UnitZ, 0.5, 0.6, 0.7);
                var faces = box.GetFaces().CastArray<IFace2>();
                var face = faces[0];

                var surface1 = ((Surface) face.GetSurface());
                var surfaceParams = surface1
                    .GetBSplineSurfaceParams(1e-5)
                    .WithCtrlPts(ctrlPts => ctrlPts.Select(v => new Vector4(v.X , v.Y, v.Z, v.W)));

                var surface2 = surfaceParams.ToSurface();
                var curves = GetCurvesForTrimming(face,c=>c);

                var trimmedSheet = (IBody2) surface2.CreateTrimmedSheet4(curves, true);

                trimmedSheet.Should().NotBe(null);

                yielder(box.DisplayUndoable(modelDoc));
                yielder(trimmedSheet.DisplayUndoable(modelDoc,Color.Blue));


                

            });

        }

        [SolidworksFact]
        public async Task CanRebuildASquareSheet()
        {
            await CreatePartDoc(async (modelDoc,yielder) =>
            {
                var box = Modeler.CreateSheet(Vector3.Zero, Vector3.UnitZ, 2);
                var faces = box.GetFaces().CastArray<IFace2>();
                var sheets = faces
                    .Select
                    (face =>
                    {
                        var faceParams = BSplineFace.Create(face)
                            .TransformSurfaceControlPoints
                              (ctrlPts => ctrlPts.Select(v => new Vector4(v.X+2, v.Y, v.Z, v.W)));

                        return faceParams.ToSheetBody();
                    })
                    .ToArray();

                using(box.DisplayUndoable(modelDoc)) {
                    await PauseTestExecution();
                };

                //foreach (var sheet in sheets)
                //{
                //    yielder(sheet.DisplayUndoable(modelDoc));
                //}


                var error = (int)swSheetSewingError_e.swSewingOk;
                var body = Modeler
                    .CreateBodiesFromSheets2(sheets, (int)swSheetSewingOption_e.swSewToSolid, 1e-5, ref error)
                    .CastArray<IBody2>();

                body.Should().NotBeEmpty();

                yielder(body.DisplayUndoable(modelDoc));

                await PauseTestExecution();




            });

        }

        [SolidworksFact]
        public async Task CanRebuildACirclularSheet()
        {
            await CreatePartDoc(async (modelDoc,yielder) =>
            {
                var box = Modeler.CreateCirclularSheet(Vector3.Zero, Vector3.UnitZ, 2);
                var faces = box.GetFaces().CastArray<IFace2>();
                var sheets = faces
                    .Select
                    (face =>
                    {
                        var faceParams = BSplineFace.Create(face);

                        return faceParams.ToSheetBody();
                    })
                    .ToArray();

                using(box.DisplayUndoable(modelDoc)) {
                    await PauseTestExecution();
                };

                //foreach (var sheet in sheets)
                //{
                //    yielder(sheet.DisplayUndoable(modelDoc));
                //}


                var error = (int)swSheetSewingError_e.swSewingOk;
                var body = Modeler
                    .CreateBodiesFromSheets2(sheets, (int)swSheetSewingOption_e.swSewToSolid, 1e-5, ref error)
                    .CastArray<IBody2>();

                body.Should().NotBeEmpty();

                yielder(body.DisplayUndoable(modelDoc));

                await PauseTestExecution();




            });

        }
        [SolidworksFact]
        public async Task CanRebuildABox ()
        {
            await CreatePartDoc(async (modelDoc,yielder) =>
            {
                var box = Modeler.CreateBox(Vector3.Zero, Vector3.UnitZ, 0.5, 0.5, 0.5);
                var faces = box.GetFaces().CastArray<IFace2>();
                var sheets = faces
                    .Select
                    (face =>
                    {
                        var faceParams = BSplineFace.Create(face)
                            .TransformSurfaceControlPoints
                              (ctrlPts => ctrlPts.Select(v => new Vector4(v.X+2, v.Y, v.Z, v.W)));

                        return faceParams.ToSheetBody();
                    })
                    .ToArray();

                using(box.DisplayUndoable(modelDoc)) {
                    await PauseTestExecution();
                };

                //foreach (var sheet in sheets)
                //{
                //    yielder(sheet.DisplayUndoable(modelDoc));
                //}


                var error = (int)swSheetSewingError_e.swSewingOk;
                var body = Modeler
                    .CreateBodiesFromSheets2(sheets, (int)swSheetSewingOption_e.swSewToSolid, 1e-5, ref error)
                    .CastArray<IBody2>();

                body.Should().NotBeEmpty();

                yielder(body.DisplayUndoable(modelDoc));

                await PauseTestExecution();




            });

        }

        [SolidworksFact]
        public async Task CanRebuildCylinder()
        {
            await CreatePartDoc(async (modelDoc,yielder) =>
            {
                var m = SwAddinBase.Active.Modeler;

                var cylinder = m.CreateCylinder(0.8, 5);
                cylinder = cylinder.GetProcessedBody2(Math.PI/3, Math.PI/3);

                using(cylinder.DisplayUndoable(modelDoc)) {
                    await PauseTestExecution();
                };

                var body = RoundTrip(cylinder);

                body.Should().NotBeEmpty();

                using(body.DisplayUndoable(modelDoc)) {
                    await PauseTestExecution();
                };

            });

        }

        private static IBody2[] RoundTrip(IBody2 cutBody)
        {
            var faces = cutBody.GetFaces().CastArray<IFace2>();
            var sheets = faces.Select(face => BSplineFace.Create(face).ToSheetBody()).ToArray();

            //foreach (var sheet in sheets)
            //{
            //    yielder(sheet.DisplayUndoable(modelDoc));
            //}


            var error = (int) swSheetSewingError_e.swSewingOk;
            var body = Modeler
                .CreateBodiesFromSheets2(sheets, (int) swSheetSewingOption_e.swSewToSolid, 1e-5, ref error)
                .CastArray<IBody2>();
            return body;
        }

        [SolidworksFact]
        public async Task CanRebuildComplexFeature()
        {
            await CreatePartDoc(async (modelDoc,yielder) =>
            {
                var m = SwAddinBase.Active.Modeler;
                var cone = m.CreateCone(0.8,0.2, 5);
                var box = m.CreateBox(1, 2, 3);
                var solid = box.Cut(cone);
                var cutBody = solid.Bodies[0];

                var faces = cutBody.GetFaces().CastArray<IFace2>();
                var sheets = faces
                    .Select
                    (face =>
                    {
                        var faceParams = BSplineFace.Create(face)
                            .TransformSurfaceControlPoints
                              (ctrlPts => ctrlPts.Select(v => new Vector4(v.X+1, v.Y, v.Z, v.W)));

                        return faceParams.ToSheetBody();
                    })
                    .ToArray();

                //foreach (var sheet in sheets)
                //{
                //    yielder(sheet.DisplayUndoable(modelDoc));
                //}


                var error = (int)swSheetSewingError_e.swSewingOk;
                var body = Modeler
                    .CreateBodiesFromSheets2(sheets, (int)swSheetSewingOption_e.swSewToSolid, 1e-5, ref error)
                    .CastArray<IBody2>();

                body.Should().NotBeEmpty();

                yielder(body.DisplayUndoable(modelDoc));

                await PauseTestExecution();




            });

        }

        /// <summary>
        /// Returns all the curves of the face in a single array with each loop
        /// seperated by a null entry as required by Surface::CreateTrimmedSheet4
        /// </summary>
        /// <param name="face"></param>
        /// <param name="transformer"></param>
        /// <returns></returns>
        private static ICurve[] GetCurvesForTrimming(IFace2 face,Func<ICurve,ICurve> transformer )
        {
            return face
                .GetTrimLoops()
                .Select(curves=>curves.Select(transformer).ToList())
                .PackForTrimming();
        }


        [SolidworksFact]
        public void BoxConversionShouldWork()
        {
            CreatePartDoc(modelDoc =>
            {

                var planeSurf = (Surface)Modeler.CreatePlanarSurface2(new[] {0,0,0.0}, new[] {0,0,1.0}, new[] {1,0,0.0});
                var parameters = planeSurf.GetBSplineSurfaceParams(1e-5);

                var foo = parameters.ToSurface();

                //#####################################
                var d0 = foo.ToSheetBody().DisplayUndoable(modelDoc, Color.Blue);
                return new CompositeDisposable(d0);
            });
        }

        
    }
}