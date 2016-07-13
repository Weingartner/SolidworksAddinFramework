using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Geometry
{
    public class BSplineFace
    {
        public BSplineSurface Surface { get; }
        public IReadOnlyList<IReadOnlyList<BSpline2D>> TrimLoops { get; }

        public BSplineFace
            TransformSurfaceControlPoints(Func<Vector4[,], Vector4[,]> xformSurface)
        {
            var surface = Surface.WithCtrlPts(xformSurface);

            return new BSplineFace(surface, TrimLoops);
        }


        public BSplineFace(BSplineSurface surface, IReadOnlyList<IReadOnlyList<BSpline2D>> trimLoops)
        {
            Surface = surface;
            TrimLoops = trimLoops;
        }

        /// <summary>
        /// Create a BSplineFace from the TrimCurve data.
        /// http://help.solidworks.com/2015/English/api/sldworksapi/SOLIDWORKS.Interop.sldworks~SOLIDWORKS.Interop.sldworks.IFace2~GetTrimCurves2.html
        /// </summary>
        /// <param name="swFace"></param>
        /// <returns></returns>
        public static BSplineFace Create(IFace2 swFace )
        {
            var start = 0;

            var packedData = swFace.GetTrimCurves2(WantCubic: true, WantNRational: false).CastArray<double>();
            var reader = new GetTrimCurves2DataReader(packedData);

            // Packed Double 1
            var packedDouble1 = reader.ReadDouble().DoubleToInteger();
            int numLoops = packedDouble1.Item1;
            int numSPCurves = packedDouble1.Item2;

            // PackeDouble 2
            var curvesPerLoopLookup = reader.ReadIntegers(numLoops).ToList();

            // PackedDouble 3
            var spCurveInfos = reader
                .ReadBufferedIntegers(4, numSPCurves)
                .Select(b => new {order = b[0], isPeriodic = b[1]== 1, dimension=b[2], isRational=b[2]==3, numCtrlPoints = b[3]})
                .ToList();

            var spCurveInfos2 = spCurveInfos
                .Select
                (info =>
                {
                    var knots = reader.Read(info.order + info.numCtrlPoints).ToList();
                    return new {info.order,info.isPeriodic, info.dimension, info.isRational, info.numCtrlPoints, knots};
                })
                .ToList();

            var trimCurves = spCurveInfos2
                .Select
                (info =>
                {
                    var ctrlPoints = reader
                        .Read(info.numCtrlPoints*info.dimension)
                        .Buffer(info.dimension, info.dimension)
                        .Select(ToRationalVector3)
                        .ToList();

                    return new BSpline2D(ctrlPoints.ToArray(), info.knots.ToArray(), info.order, info.isPeriodic);

                })
                .ToArray();


            var bLoops = curvesPerLoopLookup
                .Scan(new {start = 0, step = 0}, (acc, count) => new {start = acc.start + acc.step, step = count})
                .Skip(1)
                .Select(o => trimCurves.ToArraySegment(o.start, o.step).ToArray())
                .ToArray();

            // packed double 4
            var surfaceDimension = reader.ReadDouble().DoubleToInteger().Item1;

            // packed double 5
            var uvOrder = reader.ReadDouble().DoubleToInteger().Map((u, v) => new {u, v});

            // packed double 6
            var uvNumCtrlPoints = reader.ReadDouble().DoubleToInteger().Map((u, v) => new {u, v});

            // packed double 7
            var uvIsPeriodic = reader.ReadDouble().DoubleToInteger().Map((u, v) => new {u, v});

            // surfaceKnotValuesU
            var uKnots = reader.Read(uvOrder.u + uvNumCtrlPoints.u).ToArray();
            
            // surfaceKnotValuesV
            var vKnots = reader.Read(uvOrder.v + uvNumCtrlPoints.v).ToArray();

            // surfaceCtrlPoinCoords
            var surfaceCtrlPoints = reader.Read(surfaceDimension*uvNumCtrlPoints.u*uvNumCtrlPoints.v)
                .Buffer(surfaceDimension, surfaceDimension)
                .Select(ToRationalVector4)
                .ToList();

            // packed doubles 8 
            // TODO handle the case for multiple surfaces
            var indexFlags = reader.ReadDouble().DoubleToInteger().Map((nSurface, index) => new {nSurface, index});

            var ctrlPointsArray = surfaceCtrlPoints.Reshape(uvNumCtrlPoints.u, uvNumCtrlPoints.v);

            var bSurface = new BSplineSurface(ctrlPointsArray,uvOrder.u, uvOrder.v,uKnots, vKnots);

            return new BSplineFace(bSurface, bLoops);

        }

        public IBody2 ToSheetBody()
        {
            var surface = Surface.ToSurface();
            var loops =
                TrimLoops.SelectMany(loop => loop.Select(c => c.ToPCurve(surface)).EndWith(null)).SkipLast(1).ToArray();
            var trimmedSheet4 = (IBody2) surface.CreateTrimmedSheet4(loops, true);
            Debug.Assert(trimmedSheet4!=null);
            return trimmedSheet4;
        }
        private static Vector3 ToRationalVector3(IList<double> data)
        {
            return new Vector3
                ((float) data[0], (float) data[1], (float) (data.Count == 3 ? data[2] : 1));
        }
        private static Vector4 ToRationalVector4(IList<double> data)
        {
            return new Vector4
                ((float) data[0], (float) data[1], (float)data[2], (float) (data.Count == 4 ? data[3] : 1));
        }


    }
}