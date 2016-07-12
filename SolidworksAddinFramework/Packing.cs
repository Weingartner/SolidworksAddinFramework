using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LanguageExt;

namespace SolidworksAddinFramework
{
    public static class Packing
    {
        public static Tuple<int, int> DoubleToInteger(this double v)
        {
            var bytes = v.Bytes();
            var i0 = BitConverter.ToInt32(bytes, 0);
            var i1 = BitConverter.ToInt32(bytes, 4);
            return Prelude.Tuple(i0, i1);
        }
        public static Tuple<short, short, short, short> DoubleToShort(this double v)
        {
            var bytes = v.Bytes();
            var i0 = BitConverter.ToInt16(bytes, 0);
            var i1 = BitConverter.ToInt16(bytes, 2);
            var i2 = BitConverter.ToInt16(bytes, 4);
            var i3 = BitConverter.ToInt16(bytes, 6);
            return Prelude.Tuple(i0, i1,i2,i3);
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
}