using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SolidworksAddinFramework
{
    public static class EnumerableExtensions
    {

        /// <summary>
        /// This assumes that the inner enumerables always overlap with each other but
        /// we wish to eliminate the overlap. For example [ [ 1 2 3 4] [3 4 5 6] [5 6 7 8] ]
        /// has an overlap width of 2. The output result should be [ 1 2 3 4 5 6 7 8 ]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveOverlap<T>(this IEnumerable<IEnumerable<T>> enumerable, int overlap)
        {
            var i = 0;
            foreach (var inner in enumerable)
            {
                var j = 0;
                foreach (var item in inner)
                {
                    if (i < overlap && j == 0 || j != 0)
                        yield return item;

                    j++;
                }
                i++;
                
            }
        }

        public static IEnumerable<List<T>> BufferTillChanged<T,U>(this IEnumerable<T> source, Func<T,U> selector )
        {
            List<T> buffer = new List<T>();
            Option<U> pattern = None; 
            foreach (var item in source)
            {
                var key = selector(item);
                if (pattern != key)
                {
                    if (buffer.Count > 0)
                        yield return buffer;
                    buffer = new List<T>();
                }
                buffer.Add(item);
                pattern = Some(key);
            }
            if(buffer.Count>0)
                yield return buffer;
        }

        /// <summary>
        /// Index of minium elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IList<int> IndexOfMinBy<T, U>(this IEnumerable<T> @this, Func<T, U> fn)
        {
            return @this.Select((v, i) => new { v, i }).MinBy(p => fn(p.v)).Select(p => p.i).ToList();
        }

        /// <summary>
        /// Index of maximum elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IList<int> IndexOfMaxBy<T, U>(this IEnumerable<T> @this, Func<T, U> fn)
        {
            return @this.Select((v, i) => new { v, i }).MaxBy(p => fn(p.v)).Select(p => p.i).ToList();
        }

    }
}
