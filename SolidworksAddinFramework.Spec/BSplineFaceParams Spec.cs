using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Xunit;
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

        /// <summary>
        /// Demonstrate extracting a bspline face from a disc and then
        /// reconstructing that disc using the bspline face. Shows
        /// roundtripping the b-surface and trim loop extraction
        /// </summary>
        /// <returns></returns>
        [SolidworksFact]
        public async Task CanRebuildACirclularSheet()
        {
            await CreatePartDoc(async (modelDoc,yielder) =>
            {

                var disc0 = Modeler.CreateCirclularSheet
                    ( center: Vector3.Zero
                    , vNormal: Vector3.UnitZ
                    , radius: 2
                    );

                // Display the original sheet. Visually verify it is ok.
                // Press "Continue Test Execution" button within solidworks
                // to continue the test after visual inspection of the sheet
                using (disc0.DisplayUndoable(modelDoc))
                    await PauseTestExecution();

                // The sheet should only have one face. Extract it
                var faces = disc0.GetFaces().CastArray<IFace2>();
                faces.Length.Should().Be(1);
                var face = faces[0];

                // Convert the solidworks face to a bspline face
                var bsplineFace = BSplineFace.Create(face);

                // Convert the bspline face back to a sheet body
                var disc1 = bsplineFace.ToSheetBody();

                // Display the recovered sheet. Visually verify it is ok
                // Press "Continue Test Execution" button within solidworks
                // to continue the test after visual inspection of the sheet
                using (disc1.DisplayUndoable(modelDoc))
                    await PauseTestExecution();

                await PauseTestExecution();

            });

        }

        [SolidworksFact]
        public async Task CanSpecifyTrimLoopsManually()
        {
            #region data

            var ctrlPts = new double[]
            {
                0.5, 0.0059288535267114639,
                0.51212871074676514, 0.0059288535267114639,
                0.52424293756484985, 0.0065239840187132359,
                0.53635692596435547, 0.0071191051974892616,
                0.54842746257781982, 0.0083079412579536438,
                0.5604977011680603, 0.009496753104031086,
                0.57249528169631958, 0.011276427656412125,
                0.58449268341064453, 0.013056064955890179,
                0.59638851881027222, 0.015422292053699493,
                0.6082841157913208, 0.017788467928767204,
                0.62004947662353516, 0.020735546946525574,
                0.63181465864181519, 0.023682562634348869,
                0.64342129230499268, 0.02720339223742485,
                0.65502768754959106, 0.030724143609404564,
                0.66644757986068726, 0.034810245037078857,
                0.67786717414855957, 0.038896255195140839,
                0.689072847366333, 0.04353778064250946,
                0.70027822256088257, 0.048179201781749725,
                0.71124261617660522, 0.053364973515272141,
                0.72220677137374878, 0.0585506297647953,
                0.73290354013442993, 0.064268149435520172,
                0.74360001087188721, 0.069985546171665192,
                0.75400334596633911, 0.0762210413813591,
                0.76440644264221191, 0.082456409931182861,
                0.77449125051498413, 0.089194856584072113,
                0.78457581996917725, 0.09593316912651062,
                0.79431784152984619, 0.10315833240747452,
                0.80405968427658081, 0.11038335412740707,
                0.813435435295105, 0.1180778369307518,
                0.82281100749969482, 0.125772163271904,
                0.83179789781570435, 0.1339174211025238,
                0.84078460931777954, 0.14206252992153168,
                0.84936106204986572, 0.15063893795013428,
                0.8579372763633728, 0.15921518206596375,
                0.86608254909515381, 0.16820210218429565,
                0.87422764301300049, 0.17718882858753204,
                0.88192218542099, 0.18656457960605621,
                0.88961648941040039, 0.19594015181064606,
                0.89684164524078369, 0.20568215847015381,
                0.90406668186187744, 0.21542397141456604,
                0.91080516576766968, 0.22550877928733826,
                0.91754347085952759, 0.23559336364269257,
                0.9237789511680603, 0.24599666893482208,
                0.93001431226730347, 0.25639975070953369,
                0.935731828212738, 0.26709645986557007,
                0.94144922494888306, 0.27779296040534973,
                0.94663500785827637, 0.28875735402107239,
                0.95182067155838013, 0.29972150921821594,
                0.95646220445632935, 0.310927152633667,
                0.961103618144989, 0.32213255763053894,
                0.96518975496292114, 0.33355244994163513,
                0.96927577257156372, 0.34497207403182983,
                0.972796618938446, 0.35657870769500732,
                0.97631734609603882, 0.36818510293960571,
                0.97926443815231323, 0.37995049357414246,
                0.98221147060394287, 0.3917156457901001,
                0.9845777153968811, 0.40361151099205017,
                0.98694390058517456, 0.41550707817077637,
                0.98872357606887817, 0.42750471830368042,
                0.990503191947937, 0.4395020604133606,
                0.991692066192627, 0.45157256722450256,
                0.99288088083267212, 0.46364277601242065,
                0.99347603321075439, 0.47575706243515015,
                0.99407112598419189, 0.48787111043930054,
                0.99407112598419189, 0.5,
                0.99407112598419189, 0.51212865114212036,
                0.99347603321075439, 0.52424293756484985,
                0.99288088083267212, 0.53635692596435547,
                0.991692066192627, 0.54842746257781982,
                0.99050325155258179, 0.56049764156341553,
                0.98872357606887817, 0.57249528169631958,
                0.98694396018981934, 0.58449262380599976,
                0.9845777153968811, 0.59638851881027222,
                0.98221153020858765, 0.608284056186676,
                0.97926443815231323, 0.62004947662353516,
                0.97631746530532837, 0.63181465864181519,
                0.972796618938446, 0.64342129230499268,
                0.9692758321762085, 0.65502768754959106,
                0.96518975496292114, 0.66644757986068726,
                0.96110373735427856, 0.67786717414855957,
                0.95646220445632935, 0.689072847366333,
                0.95182079076766968, 0.70027822256088257,
                0.94663500785827637, 0.71124261617660522,
                0.94144934415817261, 0.72220677137374878,
                0.935731828212738, 0.73290354013442993,
                0.930014431476593, 0.74360001087188721,
                0.9237789511680603, 0.75400334596633911,
                0.91754359006881714, 0.76440644264221191,
                0.91080516576766968, 0.77449119091033936,
                0.90406686067581177, 0.78457581996917725,
                0.89684164524078369, 0.79431784152984619,
                0.88961666822433472, 0.80405968427658081,
                0.88192218542099, 0.813435435295105,
                0.87422782182693481, 0.82281100749969482,
                0.86608254909515381, 0.83179789781570435,
                0.85793745517730713, 0.84078460931777954,
                0.84936106204986572, 0.84936106204986572,
                0.84078478813171387, 0.8579372763633728,
                0.83179789781570435, 0.86608254909515381,
                0.82281118631362915, 0.87422764301300049,
                0.813435435295105, 0.88192218542099,
                0.80405986309051514, 0.88961648941040039,
                0.79431784152984619, 0.89684164524078369,
                0.78457599878311157, 0.90406668186187744,
                0.77449125051498413, 0.91080516576766968,
                0.76440662145614624, 0.91754347085952759,
                0.75400334596633911, 0.9237789511680603,
                0.74360024929046631, 0.93001431226730347,
                0.73290354013442993, 0.935731828212738,
                0.72220700979232788, 0.94144922494888306,
                0.71124261617660522, 0.94663500785827637,
                0.70027846097946167, 0.95182067155838013,
                0.689072847366333, 0.95646220445632935,
                0.67786741256713867, 0.96110367774963379,
                0.66644757986068726, 0.96518975496292114,
                0.65502792596817017, 0.96927577257156372,
                0.64342129230499268, 0.972796618938446,
                0.63181489706039429, 0.97631734609603882,
                0.62004947662353516, 0.97926443815231323,
                0.6082843542098999, 0.98221147060394287,
                0.59638851881027222, 0.9845777153968811,
                0.58449292182922363, 0.98694390058517456,
                0.57249528169631958, 0.98872357606887817,
                0.5604979395866394, 0.990503191947937,
                0.54842746257781982, 0.991692066192627,
                0.53635722398757935, 0.99288088083267212,
                0.52424293756484985, 0.99347603321075439,
                0.51212888956069946, 0.99407112598419189,
                0.5, 0.99407112598419189,
                0.48787134885787964, 0.99407112598419189,
                0.47575709223747253, 0.99347603321075439,
                0.46364304423332214, 0.99288088083267212,
                0.45157256722450256, 0.991692066192627,
                0.43950232863426208, 0.99050325155258179,
                0.42750471830368042, 0.98872357606887817,
                0.41550734639167786, 0.98694396018981934,
                0.40361151099205017, 0.9845777153968811,
                0.39171591401100159, 0.98221153020858765,
                0.37995049357414246, 0.97926443815231323,
                0.36818534135818481, 0.97631740570068359,
                0.35657870769500732, 0.972796618938446,
                0.34497234225273132, 0.9692758321762085,
                0.33355244994163513, 0.96518975496292114,
                0.32213279604911804, 0.96110373735427856,
                0.310927152633667, 0.95646220445632935,
                0.29972174763679504, 0.95182079076766968,
                0.28875735402107239, 0.94663500785827637,
                0.27779319882392883, 0.94144934415817261,
                0.26709645986557007, 0.935731828212738,
                0.25639995932579041, 0.930014431476593,
                0.24599666893482208, 0.9237789511680603,
                0.23559358716011047, 0.91754359006881714,
                0.22550877928733826, 0.91080516576766968,
                0.21542418003082275, 0.90406686067581177,
                0.20568215847015381, 0.89684164524078369,
                0.19594034552574158, 0.88961666822433472,
                0.18656457960605621, 0.88192218542099,
                0.17718900740146637, 0.87422782182693481,
                0.16820210218429565, 0.86608254909515381,
                0.15921536087989807, 0.85793745517730713,
                0.15063893795013428, 0.84936106204986572,
                0.14206269383430481, 0.84078478813171387,
                0.133917436003685, 0.83179789781570435,
                0.12577232718467712, 0.82281118631362915,
                0.1180778369307518, 0.813435435295105,
                0.11038351804018021, 0.80405986309051514,
                0.10315833240747452, 0.79431784152984619,
                0.095933318138122559, 0.78457605838775635,
                0.089194856584072113, 0.77449119091033936,
                0.0824565514922142, 0.76440662145614624,
                0.0762210413813591, 0.75400334596633911,
                0.069985680282115936, 0.74360024929046631,
                0.064268149435520172, 0.73290354013442993,
                0.058550752699375153, 0.72220700979232788,
                0.053364973515272141, 0.71124261617660522,
                0.048179313540458679, 0.70027846097946167,
                0.04353778064250946, 0.689072847366333,
                0.0388963520526886, 0.67786747217178345,
                0.034810245037078857, 0.66644757986068726,
                0.030724231153726578, 0.65502792596817017,
                0.02720339223742485, 0.64342129230499268,
                0.023682635277509689, 0.63181489706039429,
                0.020735548809170723, 0.62004947662353516,
                0.017788529396057129, 0.6082843542098999,
                0.015422293916344643, 0.59638851881027222,
                0.013056112453341484, 0.58449292182922363,
                0.011276429519057274, 0.57249528169631958,
                0.0094967866316437721, 0.5604979395866394,
                0.008307943120598793, 0.54842746257781982,
                0.0071191261522471905, 0.53635722398757935,
                0.0065239858813583851, 0.52424293756484985,
                0.0059288586489856243, 0.51212888956069946,
                0.0059288553893566132, 0.5,
                0.005928852129727602, 0.487871378660202,
                0.0065239858813583851, 0.47575706243515015,
                0.0071191061288118362, 0.46364304423332214,
                0.008307943120598793, 0.45157256722450256,
                0.009496753104031086, 0.43950232863426208,
                0.011276429519057274, 0.42750471830368042,
                0.013056065887212753, 0.41550734639167786,
                0.015422293916344643, 0.40361151099205017,
                0.017788469791412354, 0.39171591401100159,
                0.020735548809170723, 0.37995049357414246,
                0.023682562634348869, 0.36818534135818481,
                0.02720339223742485, 0.35657870769500732,
                0.030724145472049713, 0.34497234225273132,
                0.034810245037078857, 0.33355244994163513,
                0.038896255195140839, 0.32213279604911804,
                0.04353778064250946, 0.310927152633667,
                0.048179205507040024, 0.29972177743911743,
                0.053364973515272141, 0.28875735402107239,
                0.0585506297647953, 0.27779319882392883,
                0.064268149435520172, 0.26709645986557007,
                0.069985546171665192, 0.25639998912811279,
                0.0762210413813591, 0.24599666893482208,
                0.082456402480602264, 0.23559358716011047,
                0.089194856584072113, 0.22550877928733826,
                0.095933161675930023, 0.21542419493198395,
                0.10315833240747452, 0.20568215847015381,
                0.11038334667682648, 0.19594034552574158,
                0.1180778369307518, 0.18656457960605621,
                0.125772163271904, 0.17718902230262756,
                0.133917436003685, 0.16820210218429565,
                0.14206251502037048, 0.15921537578105927,
                0.15063893795013428, 0.15063893795013428,
                0.15921518206596375, 0.142062708735466,
                0.16820210218429565, 0.1339174211025238,
                0.17718881368637085, 0.12577234208583832,
                0.18656457960605621, 0.1180778369307518,
                0.19594013690948486, 0.11038351804018021,
                0.20568215847015381, 0.10315833240747452,
                0.21542397141456604, 0.095933318138122559,
                0.22550877928733826, 0.089194856584072113,
                0.23559336364269257, 0.0824565440416336,
                0.24599666893482208, 0.0762210413813591,
                0.25639975070953369, 0.069985680282115936,
                0.26709645986557007, 0.064268149435520172,
                0.27779296040534973, 0.058550748974084854,
                0.28875735402107239, 0.053364973515272141,
                0.29972150921821594, 0.048179313540458679,
                0.310927152633667, 0.04353778064250946,
                0.32213255763053894, 0.0388963520526886,
                0.33355244994163513, 0.034810245037078857,
                0.34497207403182983, 0.030724229291081429,
                0.35657870769500732, 0.02720339223742485,
                0.36818510293960571, 0.02368263341486454,
                0.37995049357414246, 0.020735546946525574,
                0.3917156457901001, 0.01778852753341198,
                0.40361151099205017, 0.015422292053699493,
                0.41550707817077637, 0.013056110590696335,
                0.42750471830368042, 0.011276427656412125,
                0.4395020604133606, 0.0094967847689986229,
                0.45157256722450256, 0.0083079412579536438,
                0.46364277601242065, 0.0071191242896020412,
                0.47575709223747253, 0.0065239840187132359,
                0.48787117004394531, 0.0059288535267114639,
                0.5, 0.0059288535267114639,

            };

            (ctrlPts.Length%2).Should().Be(0);

            var knots = new double[]
            {
                0,
                0,
                0,
                0.0078125,
                0.0078125,
                0.015625,
                0.015625,
                0.0234375,
                0.0234375,
                0.03125,
                0.03125,
                0.0390625,
                0.0390625,
                0.046875,
                0.046875,
                0.0546875,
                0.0546875,
                0.0625,
                0.0625,
                0.0703125,
                0.0703125,
                0.078125,
                0.078125,
                0.0859375,
                0.0859375,
                0.09375,
                0.09375,
                0.1015625,
                0.1015625,
                0.109375,
                0.109375,
                0.1171875,
                0.1171875,
                0.125,
                0.125,
                0.1328125,
                0.1328125,
                0.140625,
                0.140625,
                0.1484375,
                0.1484375,
                0.15625,
                0.15625,
                0.1640625,
                0.1640625,
                0.171875,
                0.171875,
                0.1796875,
                0.1796875,
                0.1875,
                0.1875,
                0.1953125,
                0.1953125,
                0.203125,
                0.203125,
                0.2109375,
                0.2109375,
                0.21875,
                0.21875,
                0.2265625,
                0.2265625,
                0.234375,
                0.234375,
                0.2421875,
                0.2421875,
                0.25,
                0.25,
                0.2578125,
                0.2578125,
                0.265625,
                0.265625,
                0.2734375,
                0.2734375,
                0.28125,
                0.28125,
                0.2890625,
                0.2890625,
                0.296875,
                0.296875,
                0.3046875,
                0.3046875,
                0.3125,
                0.3125,
                0.3203125,
                0.3203125,
                0.328125,
                0.328125,
                0.3359375,
                0.3359375,
                0.34375,
                0.34375,
                0.3515625,
                0.3515625,
                0.359375,
                0.359375,
                0.3671875,
                0.3671875,
                0.375,
                0.375,
                0.3828125,
                0.3828125,
                0.390625,
                0.390625,
                0.3984375,
                0.3984375,
                0.40625,
                0.40625,
                0.4140625,
                0.4140625,
                0.421875,
                0.421875,
                0.4296875,
                0.4296875,
                0.4375,
                0.4375,
                0.4453125,
                0.4453125,
                0.453125,
                0.453125,
                0.4609375,
                0.4609375,
                0.46875,
                0.46875,
                0.4765625,
                0.4765625,
                0.484375,
                0.484375,
                0.4921875,
                0.4921875,
                0.5,
                0.5,
                0.5078125,
                0.5078125,
                0.515625,
                0.515625,
                0.5234375,
                0.5234375,
                0.53125,
                0.53125,
                0.5390625,
                0.5390625,
                0.546875,
                0.546875,
                0.5546875,
                0.5546875,
                0.5625,
                0.5625,
                0.5703125,
                0.5703125,
                0.578125,
                0.578125,
                0.5859375,
                0.5859375,
                0.59375,
                0.59375,
                0.6015625,
                0.6015625,
                0.609375,
                0.609375,
                0.6171875,
                0.6171875,
                0.625,
                0.625,
                0.6328125,
                0.6328125,
                0.640625,
                0.640625,
                0.6484375,
                0.6484375,
                0.65625,
                0.65625,
                0.6640625,
                0.6640625,
                0.671875,
                0.671875,
                0.6796875,
                0.6796875,
                0.6875,
                0.6875,
                0.6953125,
                0.6953125,
                0.703125,
                0.703125,
                0.7109375,
                0.7109375,
                0.71875,
                0.71875,
                0.7265625,
                0.7265625,
                0.734375,
                0.734375,
                0.7421875,
                0.7421875,
                0.75,
                0.75,
                0.7578125,
                0.7578125,
                0.765625,
                0.765625,
                0.7734375,
                0.7734375,
                0.78125,
                0.78125,
                0.7890625,
                0.7890625,
                0.796875,
                0.796875,
                0.8046875,
                0.8046875,
                0.8125,
                0.8125,
                0.8203125,
                0.8203125,
                0.828125,
                0.828125,
                0.8359375,
                0.8359375,
                0.84375,
                0.84375,
                0.8515625,
                0.8515625,
                0.859375,
                0.859375,
                0.8671875,
                0.8671875,
                0.875,
                0.875,
                0.8828125,
                0.8828125,
                0.890625,
                0.890625,
                0.8984375,
                0.8984375,
                0.90625,
                0.90625,
                0.9140625,
                0.9140625,
                0.921875,
                0.921875,
                0.9296875,
                0.9296875,
                0.9375,
                0.9375,
                0.9453125,
                0.9453125,
                0.953125,
                0.953125,
                0.9609375,
                0.9609375,
                0.96875,
                0.96875,
                0.9765625,
                0.9765625,
                0.984375,
                0.984375,
                0.9921875,
                0.9921875,
                1,
                1,
                1
            };



            #endregion


            var order = 3;

            (ctrlPts.Length/2 + order).Should().Be(knots.Length);

            var modeler = SwAddinBase.Active.Modeler;

            var surface = (Surface) modeler.CreatePlanarSurface(new double[] {0,0,0}, new double[] {0,0,1} );


            var props = new
            {
                dimension = 2,
                order,
                ctrlPtsLength = ctrlPts.Length,
                perioditicy =0
            };

            var bytes = new[]
            {
                BitConverter.GetBytes(props.dimension),
                BitConverter.GetBytes(props.order),
                BitConverter.GetBytes(props.ctrlPtsLength),
                BitConverter.GetBytes(props.perioditicy),
            }.SelectMany(p => p).ToArray();

            var d0 = BitConverter.ToDouble(bytes, 0);
            var d1 = BitConverter.ToDouble(bytes, 8);

            var propsDouble = new[] {d0, d1};
            var pCurve = (ICurve) modeler.CreatePCurve(surface, propsDouble, knots, ctrlPts);

            pCurve.Should().NotBeNull();


        }

        /// <summary>
        /// Demonstrate serializing a body to an iges stream. It's
        /// a total hack in the way it's done but as long as it works
        /// for the moment then I am happy.
        /// </summary>
        /// <returns></returns>
        [SolidworksFact]
        public void CanRoundTripIgesViaStreams ()
        {

            var disc0 = Modeler.CreateCirclularSheet
                (center: Vector3.Zero
                    ,
                    vNormal: Vector3.UnitZ
                    ,
                    radius: 2
                );

            var mStream = new MemoryStream();
            disc0.SaveAsIges(stream =>
            {
                stream.CopyTo(mStream);
                mStream.Position = 0;
            }, false);

            var loadedBody = BodyExtensions.LoadAsIges(mStream);

            loadedBody.Should().NotBeNull();
            var b0 = disc0.GetBodyBoxTs();
            var b1 = loadedBody.GetBodyBoxTs();

            b0.P0.X.Should().BeApproximately(b1.P0.X, 1e-5f);
            b0.P0.Y.Should().BeApproximately(b1.P0.Y, 1e-5f);
            b0.P0.Z.Should().BeApproximately(b1.P0.Z, 1e-5f);

            b0.P1.X.Should().BeApproximately(b1.P1.X, 1e-5f);
            b0.P1.Y.Should().BeApproximately(b1.P1.Y, 1e-5f);
            b0.P1.Z.Should().BeApproximately(b1.P1.Z, 1e-5f);


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