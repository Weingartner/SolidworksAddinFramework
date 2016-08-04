using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Weingartner.Numerics
{
    public static class Sequences
    {
        public static IEnumerable<double> Arange(int start, int count, Func<double, double> selector)
        {
            return Enumerable.Range(start, count).Select(v => selector(v));
        }

        public static IEnumerable<double> Power(IEnumerable<double> exponents, double baseValue = 10.0d)
        {
            return exponents.Select(v => Math.Pow(baseValue, v));
        }

        public static IEnumerable<Vector3> LinSpace(Vector3 start, Vector3 stop, int num, bool endpoint = true)
        {
            return LinSpace(0, 1, num, endpoint).Select(i => Vector3.Lerp(start, stop, (float) i));
        } 

        public static List<List<double>> LinSpace(List<double> start, List<double> stop, int num, bool endpoint = true)
        {
            if (start.Count == 0)
                return new List<List<double>>();

            if (start.Count != stop.Count)
                throw new ArgumentException("start and stop must have the same length");

            var lists = Enumerable
                .Zip(start, stop, (a, b) => LinSpace(a, b, num, endpoint).ToList())
                .ToList();

            var result = new List<List<double>>();

            for (int i = 0; i < num; i++)
            {
                result.Add(lists.Select(l => l[i]).ToList());
            }

            return result;

        }

        /// <summary>
        /// Given the desired distance between generated points return
        /// the change in angle. Can be used by ArcSpace with an
        /// angle stepSize.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="stepSize"></param>
        /// <returns></returns>
        public static double ArcResolution(double radius, double stepSize)
        {
            return stepSize / radius;
        }


        public static IEnumerable<double> LinSpace(double start, double desiredStop, double stepSize, bool endpoint = true)
        {
            var num = Math.Abs(desiredStop - start) / stepSize;
            int sign = desiredStop < start ? -1 : 1;
            var actualStop = sign * ((int)num) * stepSize + start;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (actualStop == desiredStop)
            {
                if (endpoint)
                {
                    return LinSpace(start, actualStop, (int)num + 1);
                }
                return LinSpace(start, actualStop, (int)num, false);
            }
            var inum = (int)num;
            if (inum == 0) inum = 1;

            if (endpoint)
            {
                if ((int)num == 0)
                    return new List<double>
                    {
                        start,
                        desiredStop
                    };
                return LinSpace(start, actualStop, inum + 1).Concat(new [] { desiredStop});
            }
            return LinSpace(start, actualStop, inum, false);
        }

        public static IEnumerable<double> LinSpace(double start, double desiredStop, int num, bool endpoint = true)
        {
            var result = new List<double>();
            if (num <= 0)
            {
                return result;
            }

            if (endpoint)
            {
                if (num == 1)
                {
                    return new List<double> { start };
                }

                var step = (desiredStop - start) / (num - 1.0d);
                result = Arange(0, num, v => (v * step) + start).ToList();
            }
            else
            {
                var step = (desiredStop - start) / num;
                result = Arange(0, num, v => (v * step) + start).ToList();
            }

            return result;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/915745/thoughts-on-foreach-with-enumerable-range-vs-traditional-for-loop
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<int> To(this int from, int to)
        {
            if (from < to)
            {
                while (from <= to)
                {
                    yield return from++;
                }
            }
            else
            {
                while (from >= to)
                {
                    yield return from--;
                }
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/915745/thoughts-on-foreach-with-enumerable-range-vs-traditional-for-loop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int step)
        {
            if (step == 0)
            {
                throw new ArgumentOutOfRangeException("step", "Param cannot be zero.");
            }

            return source.Where((x, i) => (i % step) == 0);
        }

        public static IEnumerable<double> Sample
            (Func<double, double> fn, double start, double desiredStop, int num, bool endpoint = true)
        {
            return LinSpace(start, desiredStop, num, endpoint).Select(fn);
        }

        public static IEnumerable<double> GammaSpace
            (double start
            , double stop
            , int num
            , bool endpoint = true
            , double gamma = 2.0d)
        {
            var g = (stop - start);
            return LinSpace(0, 1, num, endpoint)
                .Select(n => Math.Pow(n, gamma) * g + start);
        }

        public static int CountDigits
       (BigInteger number) => ((int)BigInteger.Log10(number)) + 1;

        private static readonly BigInteger[] BigPowers10
           = Enumerable.Range(0, 100)
                     .Select(v => BigInteger.Pow(10, v))
                     .ToArray();

        private static readonly BigInteger BigUnitMask = new BigInteger(~(uint)0);

        public static decimal RoundToSignificantDigits
        (this decimal num,
         short n)
        {
            if (num == 0)
                return 0;
            
            var bits = decimal.GetBits(num);
            var u0 = unchecked((uint)bits[0]);
            var u1 = unchecked((uint)bits[1]);
            var u2 = unchecked((uint)bits[2]);

            var i = new BigInteger(u0)
                    + (new BigInteger(u1) << 32)
                    + (new BigInteger(u2) << 64);

            var d = CountDigits(i);

            var delta = d - n;
            if (delta < 0)
                return num;

            var scale = BigPowers10[delta];
            var div = i / scale;
            var rem = i % scale;
            var up = rem > scale / 2;
            if (up)
                div += 1;
            var shifted = div * scale;

            bits[0] = unchecked((int)(uint)(shifted & BigUnitMask));
            bits[1] = unchecked((int)(uint)(shifted >> 32 & BigUnitMask));
            bits[2] = unchecked((int)(uint)(shifted >> 64 & BigUnitMask));

            return new decimal(bits);
        }

        public static double Blend(this double v0, double v1, double blend, bool checkRange=true)
        {
            if(checkRange)
            {
                Debug.Assert(blend>=0);
                Debug.Assert(blend<=1);
            }
            return v0 + blend*(v1-v0);
        }

    }
}