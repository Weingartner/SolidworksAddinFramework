using System;
using System.Collections.Generic;
using System.Xaml.Schema;
using SolidWorks.Interop.sldworks;

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
        public double XMid => (XMin + XMax)/2; 

        public double YMin => Math.Min(P0[1], P1[1]);
        public double YMax => Math.Max(P0[1], P1[1]);
        public double YMid => (YMin + YMax)/2; 

        public double ZMin => Math.Min(P0[2], P1[2]);
        public double ZMax => Math.Max(P0[2], P1[2]);
        public double ZMid => (ZMin + ZMax)/2; 

        public double[] XRange => new[] {XMin, XMax};
        public double[] YRange => new[] {YMin, YMax};
        public double[] ZRange => new[] {ZMin, ZMax};

        public double Dx => XMax - XMin;
        public double Dy => YMax - YMin;
        public double Dz => ZMax - ZMin;

        public bool Inside(double[] p)
        {
            var x = p[0];
            var y = p[1];
            var z = p[2];
            return (Between(x, XMin, XMax)&&Between(y,YMin,YMax)&& Between(z, ZMin, ZMax));
        }

        private bool Between(double v, double lower, double upper)
        {
            return v >= lower && v < upper;
        }

        public IBody2 ToSolid()
        {
            var modeler = SwAddinBase.Active.Modeler;
            var math = SwAddinBase.Active.Math;
            var center = new double[] {XMid, YMid, ZMin};
            var box = modeler.CreateBox(center, math.ZAxisArray(), this.Dx, this.Dy, this.Dz);
            return box;
        }

        public static TwoPointRange FromVertices(IEnumerable<double[]>  vertices)
        {
            double xmin = Double.MaxValue;
            double ymin = Double.MaxValue;
            double zmin = Double.MaxValue;

            double xmax = Double.MinValue;
            double ymax = Double.MinValue;
            double zmax = Double.MinValue;

            foreach (var vertex in vertices)
            {
                xmin = Math.Min(xmin, vertex[0]);
                xmax = Math.Max(xmax, vertex[0]);

                ymin = Math.Min(ymin, vertex[1]);
                ymax = Math.Max(ymax, vertex[1]);

                zmin = Math.Min(zmin, vertex[2]);
                zmax = Math.Max(zmax, vertex[2]);
            }

            return new TwoPointRange(new[] {xmin,ymin,zmin}, new[] {xmax, ymax, zmax});

        }

        public TwoPointRange Scale(double s)
        {
            s = (s - 1)/2;
            var sx = Dx*s;
            var sy = Dy*s;
            var sz = Dz*s;
            return new TwoPointRange(new[] {XMin-sx,YMin-sy,ZMin-sz}, new[] {XMax+sx, YMax+sy, ZMax+sz});
        }
    }
}