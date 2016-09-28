using System;
using FsCheck;
using static LanguageExt.Prelude;

namespace WeinCadSW.Spec.FsCheck
{
    /// <summary>
    /// Different float generators
    /// </summary>
    public static class GenFloat
    {

        private static double fraction(int a, int b, int c) =>a + (double) b/ c;

        public static Gen<double> Normal =>
                Arb.Default.DoNotSizeInt32()
                    .Generator
                    .Three()
                    .Where(v=>v.Item3.Item!=0)
                    .Select(t=>fraction(t.Item1.Item, t.Item2.Item, t.Item3.Item));

        private static Gen<double> _Range(double lower, double upper) =>
            Normal
            .Select(f => lower + (upper - lower)*Math.Abs(f)%1.0);

        public static Gen<double> Range(double lower, double upper) =>
            Gen.Frequency
                (Tuple(1, Gen.Constant(lower)),
                 Tuple(1, Gen.Constant(lower + double.Epsilon)),
                 Tuple(1, Gen.Constant(upper - double.Epsilon)),
                 Tuple(1, Gen.Constant(upper)),
                 Tuple(20, GenFloat._Range(lower, upper))
                );
    }
}