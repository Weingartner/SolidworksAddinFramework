using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Accord;

namespace SolidworksAddinFramework
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Enumerate a 2D matrix defined as T[row,column] by traversing
        /// each column completey before the next column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> EnumerateColumnWise<T>(this T[,] array)
        {
            for (int v = 0; v < array.GetLength(1); v++)
            {
                for (int u = 0; u < array.GetLength(0); u++)
                {
                    yield return array[u, v];

                }
            }
        }

        /// <summary>
        /// Reshape a 1D array into a 2D array. The array is filled columnwise
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="uCount"></param>
        /// <param name="vCount"></param>
        /// <returns></returns>
        public static T[,] Reshape<T>(this IReadOnlyList<T> array , int uCount, int vCount)
        {
            Debug.Assert(array.Count == uCount*vCount);

            var array2D = new T[uCount, vCount];
            for (var u = 0; u < uCount; u++)
            {
                for (var v = 0; v < vCount; v++)
                {
                    array2D[u, v] = array[v*uCount + u];
                }
            }
            return array2D;
        }

        /// <summary>
        /// Enumerate a 2D matrix defined as T[row,column] by traversing
        /// each row completely before the next row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> EnumerateRowWise<T>(this T[,] array)
        {
            for (int u = 0; u < array.GetLength(0); u++)
            {
                for (int v = 0; v < array.GetLength(1); v++)
                {
                    yield return array[u, v];

                }
            }
        }

        public static U[,] Select<T, U>(this T[,] ts, Func<T, U> sel)
        {
            var t0 = ts.GetLength(0);
            var t1 = ts.GetLength(1);
            var us = new U[t0, t1];
            for (int t0i = 0; t0i < t0; t0i++)
            {
                for (int t1i = 0; t1i < t1; t1i++)
                {
                    us[t0i, t1i] = sel(ts[t0i, t1i]);
                }
            }
            return us;
        }
        public static U[,] Zip<T0,T1, U>(this T0[,] t0s, T1[,] t1s, Func<T0,T1, U> sel)
        {
            var t00 = t0s.GetLength(0);
            var t01 = t0s.GetLength(1);

            var t10 = t1s.GetLength(0);
            var t11 = t1s.GetLength(1);

            if(t00!=t10)
                throw new DimensionMismatchException(nameof(t0s) );
            if(t01!=t11)
                throw new DimensionMismatchException(nameof(t0s) );

            var t0 = t00;
            var t1 = t01;

            var us = new U[t0, t1];
            for (var t0i = 0; t0i < t0; t0i++)
            {
                for (var t1i = 0; t1i < t1; t1i++)
                {
                    us[t0i, t1i] = sel(t0s[t0i, t1i], t1s[t0i, t1i]);
                }
            }
            return us;
        }

        public static void Foreach<T>(this T[,] ts, Action<T> action)
        {
            ts.Cast<T>().ForEach(action);
        }

        public static void Fill<T>(this T[,] ts, T value)
        {
            for (int u = 0; u < ts.GetLength(0); u++)
            {
                for (int v = 0; v < ts.GetLength(1); v++)
                {
                    ts[u, v] = value;
                }
            }
        }
    }
}
