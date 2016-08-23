using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt;
using Weingartner.Exceptional;
using static LanguageExt.Prelude;

namespace SolidworksAddinFramework
{
    public static class OptionExtensions
    {


        /// <summary>
        /// Get the value from an option. If it is none 
        /// then a null reference exception will be raised.
        /// Don't use this in production please.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns></returns>
        public static T __Value__<T>(this Option<T> option)
        {
            return option.Match
                (v => v ,
                    () =>
                    {
                        throw new NullReferenceException();
                    });
        }

        public static Option<ImmutableList<T>> Sequence<T>(this IEnumerable<Option<T>> p)
        {
            return p.Fold(
                Prelude.Optional(ImmutableList<T>.Empty),
                (state, itemOpt) =>
                    from item in itemOpt
                    from list in state
                    select list.Add(item));
        }

        /// <summary>
        /// Invokes the action if it is there.
        /// </summary>
        /// <param name="a"></param>
        public static void Invoke(this Option<Action> a)
        {
            a.IfSome(fn => fn());
        }

        /// <summary>
        /// Fluent version Optional
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Option<T> ToOption<T>(this T obj)
        {
            return Optional(obj);
        }

        public static Option<T> ToOption<T>(this IExceptional<T> obj)
        {
            return obj.Match(Some, _ => None);
        }
    }
}