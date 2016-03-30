using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class MathTransformExtensions
    {
        public static void ExtractTransform(this IMathTransform transform, out DenseMatrix rotation, out DenseVector translation)
        {
            double scale;
            ExtractTransform(transform, out rotation, out translation, out scale);
        }

        public static void ExtractTransform(this IMathTransform transform, out DenseMatrix rotation, out DenseVector translation, out double scale)
        {
            var transformArray = transform.ArrayData.CastArray<double>();
            rotation = new DenseMatrix(3, 3, transformArray.Take(9).ToArray());
            translation = new DenseVector(transformArray.Skip(9).Take(3).ToArray());
            scale = transformArray[12];
        }

    }
}