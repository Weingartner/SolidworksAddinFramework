using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using XUnit.Solidworks.Addin;
using SwBSplineParamsExtensions = SolidworksAddinFramework.SwBSplineParamsExtensions;

namespace SolidworksAddinFramework.Spec
{
    public class SwBSplineParamsSpec : SolidWorksSpec
    {
        [SolidworksFact]
        public void ForwardAndBackSplineConversionShouldWork()
        {
            CreatePartDoc(true, modelDoc =>
            {
                var trimCurve = Modeler.CreateTrimmedLine(new Vector3(0,0,0f),new Vector3(1,0,0f));

                var parameters = trimCurve.GetBSplineParams(false, 1e-8);
                var swCurve = parameters.ToCurve();
                var parameters2 = swCurve.GetBSplineParams(false, 1e-8);

                parameters2.Should().Be(parameters);

                //#####################################
                var d0 = swCurve.CreateWireBody().DisplayUndoable(modelDoc, Color.Blue);
                //var d1 = trimCurve.CreateWireBody().DisplayUndoable(modelDoc, Color.Red);
                return new CompositeDisposable(d0);
            });
        }

        [SolidworksFact]
        public void CanRoundTripAFace()
        {
            CreatePartDoc(true, (modelDoc,yielder) =>
            {
                var box = Modeler.CreateBox(Vector3.Zero, Vector3.UnitZ, 0.5, 0.6, 0.7);
                var faces = box.GetFaces().CastArray<IFace2>();
                var face = faces[0];

                var surface1 = ((Surface) face.GetSurface());
                var surfaceParams = surface1
                    .GetBSplineSurfaceParams(1e-5)
                    .WithCtrlPts(ctrlPts => ctrlPts.Select(v => new Vector4(v.X , v.Y, v.Z, v.W)));

                var surface2 = SurfaceExtensions.CreateBSplineSurface(surfaceParams);
                var curves = GetCurvesForTrimming(face,c=>c);

                var trimmedSheet = (IBody2) surface2.CreateTrimmedSheet4(curves, true);

                trimmedSheet.Should().NotBe(null);

                yielder(box.DisplayUndoable(modelDoc));
                yielder(trimmedSheet.DisplayUndoable(modelDoc,Color.Blue));

                

            });

        }
        [SolidworksFact]
        public void CanRebuildABox ()
        {
            CreatePartDoc(true, (modelDoc,yielder) =>
            {
                var box = Modeler.CreateBox(Vector3.Zero, Vector3.UnitZ, 0.5, 0.6, 0.7);
                var faces = box.GetFaces().CastArray<IFace2>();
                var sheets = faces
                    .Select
                    (face =>
                    {
                        var surface1 = ((Surface) face.GetSurface());
                        var surfaceParams = surface1
                            .GetBSplineSurfaceParams(1e-5)
                            .WithCtrlPts(ctrlPts => ctrlPts.Select(v => new Vector4(v.X+1, v.Y, v.Z, v.W)));

                        var surface2 = SurfaceExtensions.CreateBSplineSurface(surfaceParams);
                        var curves = GetCurvesForTrimming(face,
                            curve =>
                            {
                                //curve.DisplayUndoable(modelDoc, Color.Red, 3);
                                var param = curve.GetBSplineParams(curve.StartPoint() == curve.EndPoint(), 1e-5);
                                param = param.WithControlPoints
                                    (ctrlPts => ctrlPts.Select(v => new Vector4(v.X, v.Y, v.Z, v.W)).ToArray());
                                var curve2 = param.ToCurve();
                                curve2.DisplayUndoable(modelDoc, Color.Blue, 3);

                                return curve2;

                            });

                        return (IBody2) surface2.CreateTrimmedSheet4(curves, true);

                    })
                    .ToArray();

                //foreach (var sheet in sheets)
                //{
                //    yielder(sheet.DisplayUndoable(modelDoc));
                //}


                //var error = (int) swSheetSewingError_e.swSewingOk;
                //var body = Modeler
                //    .CreateBodiesFromSheets2(sheets, (int) swSheetSewingOption_e.swSewToSolid, 1e-5, ref error)
                //    .CastArray<IBody2>();

                //body.Should().NotBeEmpty();

                //yielder(body.DisplayUndoable(modelDoc));

                

            });

        }

        /// <summary>
        /// Returns all the curves of the face in a single array with each loop
        /// seperated by a null entry as required by Surface::CreateTrimmedSheet4
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private static ICurve[] GetCurvesForTrimming(IFace2 face,Func<ICurve,ICurve> transformer )
        {
            return face.GetLoops().CastArray<ILoop2>()
                .SelectMany(
                    l =>
                        l.GetEdges()
                            .CastArray<IEdge>()
                            .Select(edge => (ICurve) edge.GetCurve() )
                            .Select(c=>transformer((ICurve)c.Copy()))
                            .Concat(new ICurve[] {null}))
                .SkipLast(1)
                .ToArray();
        }


        [SolidworksFact]
        public void BoxConversionShouldWork()
        {
            CreatePartDoc(false, modelDoc =>
            {

                var planeSurf = (Surface)Modeler.CreatePlanarSurface2(new[] {0,0,0.0}, new[] {0,0,1.0}, new[] {1,0,0.0});
                var parameters = planeSurf.GetBSplineSurfaceParams(1e-5);

                var foo = SurfaceExtensions.CreateBSplineSurface(parameters);

                //#####################################
                var d0 = foo.ToSheetBody().DisplayUndoable(modelDoc, Color.Blue);
                return new CompositeDisposable(d0);
            });
        }

        
    }
}