using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
