using System;
using System.Collections.Generic;
using System.Linq;

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

        public static List<T> RotateToHead<T>(this IList<T> list, Func<T, bool> selector)
        {
            var q = new Queue<T>(list);
            while (!selector(q.Head()))
            {
                var v = q.Dequeue();
                q.Enqueue(v);
            }

            return q.ToList();
        }

        public static List<T> Rotate<T>(this IList<T> list, int n)
        {
            var q = new Queue<T>(list);
            for (int i = 0; i < n; i++)
            {
                var v = q.Dequeue();
                q.Enqueue(v);
                
            }
            return q.ToList();
        }
    }
}