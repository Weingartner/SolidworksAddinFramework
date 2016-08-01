using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
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

        internal class Group<TKey,TValue> : IGrouping<TKey, TValue>
        {
            private readonly ImmutableList<TValue> Values;

            public Group(TKey key, ImmutableList<TValue> values)
            {
                Values = values;
                Key = key;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public TKey Key { get; }
        }

        public static IEnumerable<IGrouping<TKey, TValue>> GroupAdjacent<TValue, TKey>(this IEnumerable<TValue> e,
            Func<TValue, TKey> keySelector)
        {
            var list = new List<TValue>();
            var currentKey = Option<TKey>.None; 
            foreach (var value in e)
            {
                var key = keySelector(value);
                if (key != currentKey)
                {
                    if (!currentKey.IsNone)
                        yield return new Group<TKey, TValue>(currentKey.__Value__(), list.ToImmutableList());
                    list = new List<TValue>();
                    currentKey = key;
                }
                list.Add(value);
            }
            if (!currentKey.IsNone)
                yield return new Group<TKey, TValue>(currentKey.__Value__(), list.ToImmutableList());
            
        } 

        /// <summary>
        /// Switch the dimensions in IEnumerable of IEnumerable. Assumes
        /// that all the sub IEnumerables have the same dimension
        /// http://stackoverflow.com/a/10555037/158285
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Transpose<T>( this IEnumerable<IEnumerable<T>> @this)
        {
            var enumerators = @this.Select(t => t.GetEnumerator());
            do
            {
                enumerators = enumerators.Where(e => e.MoveNext());
                var r = enumerators.Select(e => e.Current);
                if (!r.Any())
                    break;
                yield return r;
            } while (true);
        }

        public static IEnumerable<KeyValuePair<U,List<T>>> BufferTillChanged<T,U>(this IEnumerable<T> source, Func<T,U> selector )
        {
            List<T> buffer = new List<T>();
            Option<U> pattern = None; 
            foreach (var item in source)
            {
                var key = selector(item);
                if (pattern != key)
                {
                    if (buffer.Count > 0)
                        yield return new KeyValuePair<U, List<T>>(pattern.__Value__(), buffer);
                    buffer = new List<T>();
                }
                buffer.Add(item);
                pattern = Some(key);
            }
            if(buffer.Count>0)
                yield return new KeyValuePair<U, List<T>>(pattern.__Value__(),buffer);
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

        public static IEnumerable<T> EndWith<T>(this IEnumerable<T> e, T item) =>
            e.Concat(new[] {item});

        public static IEnumerable<IList<T>> Paired<T>(this IEnumerable<T> e)
        {
            return  e
                .Buffer(2, 1)
                .Where(p => p.Count == 2);
        }

        /// <summary>
        /// Return the cumlative application of the function
        /// on the input sequence starting with a seed
        /// value. Note the seed value will be returned
        /// as the first element of th output sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="This"></param>
        /// <param name="initial"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IEnumerable<T> CummulativeAggregate<T, U>
            (this IEnumerable<U> This
            , T initial
            , Func<T, U, T> fn
            )
        {
            yield return initial;
            foreach (var item in This)
            {
                initial = fn(initial, item);
                yield return initial;
            }
        }


    }
}
