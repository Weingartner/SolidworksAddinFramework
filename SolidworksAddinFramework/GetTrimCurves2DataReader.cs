using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class GetTrimCurves2DataReader
    {
        double[] _Data;
        private int idx;

        public GetTrimCurves2DataReader(double[] data)
        {
            _Data = data;
        }
        public IEnumerable<double> Read(int n)
        {
            for (var i = 0; i < n; i++)
            {
                yield return _Data[idx++];
            }
        }


        /// <summary>
        /// Read n integers from the double array. If
        /// n is odd then the next even is thrown away
        /// and consumed.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public IEnumerable<int> ReadIntegers(int n)
        {
            return Read(n.DivideByAndRoundUp(2)).DoubleToInteger().Take(n);
        }
        /// <summary>
        /// Read n * b integers and return them in group sizes of b
        /// </summary>
        /// <param name="bufferSize"></param>
        /// <param name="numberOfBuffers"></param>
        /// <returns></returns>
        public IEnumerable<int[]> ReadBufferedIntegers(int bufferSize, int numberOfBuffers)
        {
            return ReadIntegers(numberOfBuffers*bufferSize)
                .Buffer(bufferSize, bufferSize)
                .Select(q=>q.ToArray());
        }

        /// <summary>
        /// Read n integers from the double array. If
        /// n%4 is not zero then the remaining int16 values are
        /// thrown away.
        /// and consumed.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public IEnumerable<short> ReadShort(int n)
        {
            return Read(n.DivideByAndRoundUp(4)).DoubleToShort().Take(n);
            
        }


        public double ReadDouble()
        {
            foreach (var d in Read(1))
            {
                return d;
            }
            throw new IndexOutOfRangeException();
        }

    }
}