using System;

namespace SolidworksAddinFramework
{
    public class TwoPointRange
    {
        public double[] P0 { get; }

        public double[] P1 { get; }

        public double[] Center => new double[]
        {
            (P0[0]+P1[0])/2,
            (P0[1]+P1[1])/2,
            (P0[2]+P1[2])/2
        };

        public TwoPointRange(double[] p0, double[] p1)
        {
            P0 = p0;
            P1 = p1;
        }

        public double XMin => Math.Min(P0[0], P1[0]);
        public double XMax => Math.Max(P0[0], P1[0]);

        public double YMin => Math.Min(P0[1], P1[1]);
        public double YMax => Math.Max(P0[1], P1[1]);
        public double ZMin => Math.Min(P0[2], P1[2]);
        public double ZMax => Math.Max(P0[2], P1[2]);

        public double[] XRange => new[] {XMin, XMax};
        public double[] YRange => new[] {YMin, YMax};
        public double[] ZRange => new[] {ZMin, ZMax};
    }
}