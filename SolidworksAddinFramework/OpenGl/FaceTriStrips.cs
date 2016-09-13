using System;
using System.Diagnostics;
using System.Linq;

namespace SolidworksAddinFramework.OpenGl
{
    public static class FaceTriStrips
    {
        public static double[][][] Unpack(double[] packedData)
        {
            if (packedData == null || packedData.Length == 0)
                return new double[0][][];

            var numStrips = (int)ToUnsignedInt32(packedData, 0);
            var vertexPerStrip = Enumerable.Range(1, numStrips)
                .Select(i => ToUnsignedInt32(packedData, i))
                .ToArray();
            var bufferIdex = vertexPerStrip
                .Scan(
                    Tuple.Create(0U, 0U),
                    (a, next) => Tuple.Create(a.Item1 + a.Item2, next) )
                .ToList();

            var r = 
                bufferIdex
                    .Select(idx =>
                        Enumerable.Range((int) idx.Item1, (int) idx.Item2)
                            .Select(i => 1 + vertexPerStrip.Length + i*3)
                            .Select(i => new[] { packedData[i], packedData[i + 1], packedData[i + 2]}).ToArray()).ToArray();

            Debug.Assert(r.Length==numStrips);

            return r;
        }

        private static uint ToUnsignedInt32(double[] data, int i)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(data[i]), 0);
        }
    }
}