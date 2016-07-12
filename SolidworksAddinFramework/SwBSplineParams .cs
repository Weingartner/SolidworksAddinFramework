using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using MathNet.Numerics;
using SolidworksAddinFramework.OpenGl;
using SolidworksAddinFramework.Wpf;
using SolidWorks.Interop.sldworks;
using static  LanguageExt.Prelude;

namespace SolidworksAddinFramework
{
    public static class Packing
    {
        public static Tuple<int, int> DoubleToInteger(this double v)
        {
            var bytes = v.Bytes();
            var i0 = BitConverter.ToInt32(bytes, 0);
            var i1 = BitConverter.ToInt32(bytes, 4);
            return Tuple(i0, i1);
        }
        public static Tuple<short, short, short, short> DoubleToShort(this double v)
        {
            var bytes = v.Bytes();
            var i0 = BitConverter.ToInt16(bytes, 0);
            var i1 = BitConverter.ToInt16(bytes, 2);
            var i2 = BitConverter.ToInt16(bytes, 4);
            var i3 = BitConverter.ToInt16(bytes, 6);
            return Tuple(i0, i1,i2,i3);
        }

        public static int DivideByAndRoundUp(this int i0, int i1) => (i0 - 1)/i1 + 1;

        public static IEnumerable<int> DoubleToInteger(this IEnumerable<double> values) =>
            values.SelectMany(v => v.DoubleToInteger().Map((i0, i1) => new[] {i0, i1}));

        public static IEnumerable<short> DoubleToShort(this IEnumerable<double> values) =>
            values.SelectMany(v => v.DoubleToShort().Map((i0, i1,i2,i3) => new[] {i0, i1,i2,i3}));

        public static byte[] Bytes(this double v) => BitConverter.GetBytes(v);
        public static ArraySegment<T> ToArraySegment<T>(this T[] o, int start, int step)=>new ArraySegment<T>(o,start, step);

        public static double ToDouble(this short[] props)
        {
            Debug.Assert(props.Length==4);
            return BitConverter.ToDouble(props.SelectMany(BitConverter.GetBytes).ToArray(),0);
        }
        public static double[] ToDouble(this int[] props)
        {
            Debug.Assert(props.Length%2==0);
            return props.SelectMany(BitConverter.GetBytes)
                .Buffer(8, 8)
                .Select(b => BitConverter.ToDouble(b.ToArray(), 0))
                .ToArray();
        }
    }


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

            var packedData = swFace.GetTrimCurves2(true, false).CastArray<double>();
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
                    return new {info, knots};
                })
                .ToList();

            var trimCurves = spCurveInfos2
                .Select
                (info =>
                {
                    var ctrlPoints = reader
                    .Read(info.info.numCtrlPoints*info.info.dimension)
                    .Buffer(info.info.dimension, info.info.dimension)
                    .Select(ToRationalVector3)
                    .ToList();

                    return new {info.info.isPeriodic, info.knots, info.info.order, ctrlPoints};

                })
                .ToArray();


            var loops = curvesPerLoopLookup
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
            var indexFlags = reader.ReadDouble().DoubleToInteger().Map((nSurface, index) => new {nSurface, index});

            var ctrlPointsArray = new Vector4[uvNumCtrlPoints.u, uvNumCtrlPoints.v];
            for (int u = 0; u < uvNumCtrlPoints.u; u++)
            {
                for (int v = 0; v < uvNumCtrlPoints.v; v++)
                {
                    ctrlPointsArray[u, v] = surfaceCtrlPoints[v*uvNumCtrlPoints.u + u];
                }
            }

            var surface = new BSplineSurface(ctrlPointsArray,uvOrder.u, uvOrder.v,uKnots, vKnots);
            var loops2 = loops
                .Select
                (loop => loop.Select(c => new BSpline2D(c.ctrlPoints.ToArray(), c.knots.ToArray(), c.order)).ToList())
                .ToList();

            return new BSplineFace(surface, loops2);

        }

        public IBody2 ToSheetBody()
        {
            var surface = Surface.ToSurface();
            var loops =
                TrimLoops.SelectMany(loop => loop.Select(c => c.ToPCurve(surface)).EndWith(null)).SkipLast(1).ToArray();
            return (IBody2) surface.CreateTrimmedSheet4(loops, true);
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


    public class BSpline<T> : IEquatable<BSpline<T>>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Control points are stored as (X,Y,Z) being the
        /// true location of the control point and (W) being
        /// the weight. Some frameworks store (X*W,Y*W,Z*W).
        /// This is not done here. We use the unscaled location
        /// variables. You may have to scale up and down when
        /// coverting to other systems such as eyeshot and 
        /// solidworks.
        /// 
        /// Eyeshot always stores as (X*W,Y*W,Z*W,W) in
        /// it's Point4D class. Solidworks varies depending
        /// on if the spline is periodic, closed and if 
        /// it's a tuesday or not. Grrrr. So be careful.
        /// </summary>
        public T[] ControlPoints { get; }

        public bool IsPeriodic { get; }

        public int Order { get; }

        public double[] KnotVectorU { get; }

        public BSpline([NotNull] T[] controlPoints, [NotNull] double[] knotVectorU, int order)
        {

            if (controlPoints == null) throw new ArgumentNullException(nameof(controlPoints));
            if (knotVectorU == null) throw new ArgumentNullException(nameof(knotVectorU));

            ControlPoints = controlPoints;
            Order = order;
            KnotVectorU = knotVectorU;
        }


        #region equality
        public bool Equals(BSpline<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ControlPoints.SequenceEqual(other.ControlPoints)
                   && KnotVectorU.SequenceEqual(other.KnotVectorU)

                   && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BSpline<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlPoints.Cast<Vector3>().GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ KnotVectorU.GetHashCode(v=>v.GetHashCode());
                hashCode = (hashCode*397) ^ Order;
                return hashCode;
            }
        }

        public static bool operator ==(BSpline<T> left, BSpline<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BSpline<T> left, BSpline<T> right)
        {
            return !Equals(left, right);
        }
        #endregion
        protected double[] PropsDouble
        {
            get
            {
                int dimension = 3;
                int order = (short) Order;
                int numCtrlPoints = (short) ControlPoints.Length;
                int periodicity = (short) (IsPeriodic ? 1 : 0);
                var propsDouble = new[] {dimension, order, numCtrlPoints, periodicity}.ToDouble();
                return propsDouble;
            }
        }

    }
    public class BSpline3D : BSpline<Vector4>
    {
        public BSpline3D([NotNull] Vector4[] controlPoints, [NotNull] double[] knotVectorU, int order) : base(controlPoints, knotVectorU, order)
        {
        }
        public ICurve ToCurve()
        {
            var propsDouble = PropsDouble;
            var knots = KnotVectorU;
            var ctrlPtCoords = ControlPoints.SelectMany(p => p.ToDoubles()).ToArray();
            return (ICurve) SwAddinBase.Active.Modeler.CreateBsplineCurve( propsDouble, knots, ctrlPtCoords);
        }

        public double[] ToDoubles(Vector4 t)
        {
            return t.ToDoubles();
        }
    }

    /// <summary>
    /// Control points are (X,Y,W) the W param is mapped to the Z parameter of the Vector3
    /// </summary>
    public class BSpline2D : BSpline<Vector3>
    {
        public BSpline2D([NotNull] Vector3[] controlPoints, [NotNull] double[] knotVectorU, int order) : base(controlPoints, knotVectorU, order)
        {
        }

        /// <summary>
        /// Creates a PCurve, a 2D curve parameterized in UV over
        /// the surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public ICurve ToPCurve(ISurface surface)
        {
            var propsDouble = PropsDouble;
            var knots = KnotVectorU;
            var ctrlPtCoords = ControlPoints.SelectMany(p => p.ToDoubles()).ToArray();
            return (ICurve) SwAddinBase.Active.Modeler.CreatePCurve(surface, propsDouble, knots, ctrlPtCoords);
        }


        public double[] ToDoubles(Vector3 t)
        {
            return t.ToDoubles();
        }
    }

    public static class SwBSplineParamsExtensions
    {
        public static BSpline3D GetBSplineParams(this ICurve swCurve, bool isClosed)
        {
            return swCurve
                .GetBCurveParams5
                    ( WantCubicIn: false
                    , WantNRational: false
                    , ForceNonPeriodic: false
                    , IsClosed: isClosed
                    )
                .SwBSplineParams(isClosed);
        }

        public static BSpline3D SwBSplineParams(this SplineParamData swSurfParameterisation, bool isClosed)
        {
            object ctrlPts;
            var canGetCtrlPts = swSurfParameterisation.GetControlPoints(out ctrlPts);
            Debug.Assert(canGetCtrlPts);
            var ctrlPtArray = ctrlPts.CastArray<double>();

            object knots;
            var canGetKnots = swSurfParameterisation.GetKnotPoints(out knots);
            Debug.Assert(canGetKnots);
            var knotArray = knots.CastArray<double>();

            var dimension = swSurfParameterisation.Dimension;
            var order = swSurfParameterisation.Order;
            var degree = order - 1;

            var isPeriodic = swSurfParameterisation.Periodic == 1;

            var controlPoints4D = ctrlPtArray
                .Buffer(dimension, dimension)
                .Where(p => p.Count == dimension)
                //http://help.solidworks.com/2016/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.isplineparamdata~igetcontrolpoints.html
                .Select
                (p =>
                {
                    var x = 0.0;
                    var y = 0.0;
                    var z = 0.0;
                    var w = 1.0;
                    if (dimension >= 2)
                    {
                        x = p[0];
                        y = p[1];
                    }
                    if (dimension >= 3)
                    {
                        z = p[2];
                    }
                    if (dimension == 4)
                    {
                        w = p[3];
                    }

                    return new Vector4((float) (x*w), (float) (y*w), (float) (z*w), (float) w);
                })
                .ToArray();

            if(controlPoints4D.Any(c=>c.W!=1.0))
                LogViewer.Log($"Got a rational curve, periodic = {isPeriodic}, isClosed = {isClosed}");

            if (isPeriodic)
                ConvertToNonPeriodic(ref controlPoints4D, ref knotArray, degree);

            return new BSpline3D(controlPoints4D, knotArray, order);
        }


        /// <summary>
        /// We want to convert the periodic formulation into a non periodic formulation. To do
        /// this we follow the instructions from Guilia at DevDept.
        ///
        /// the things that need to be done to get the correct curve in Eyeshot are:
        /// - add one last control point equal to the first to close the curve
        /// - multiply the(x, y, z) coordinates of each control point by its w coordinate
        /// - the degree p of your curve is given by the multiplicity of the last knot in the periodic knot vector, therefore you need to increase by 1 the multiplicity of the last knot, and by p the multiplicity of the first knot.
        /// </summary>
        /// <param name="controlPoints4D"></param>
        /// <param name="knotArray"></param>
        /// <param name="degree"></param>
        private static void ConvertToNonPeriodic(ref Vector4[] controlPoints4D, ref double[] knotArray, int degree)
        {

            controlPoints4D = controlPoints4D.EndWith(controlPoints4D.First()).ToArray();

            var last = knotArray.Last();
            var first = knotArray.First();
            knotArray = Enumerable.Repeat(first, degree).Concat(knotArray).EndWith(last).ToArray();

            // We need to do some special handling to make it non periodic
        }
    }
}