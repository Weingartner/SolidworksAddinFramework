using System;
using System.Collections.Generic;

namespace SolidworksAddinFramework
{
    public static class ListExtensions
    {
        public static List<U> Transform<T, U>(this IReadOnlyList<T> list, int count, Action<T, Action<U>> yield)
        {
            var listu = new List<U>(count);
            count = list.Count;
            for (int i = 0; i < count; i++)
            {
                yield(list[i], listu.Add);
            }

            return listu;
        }
    }
}