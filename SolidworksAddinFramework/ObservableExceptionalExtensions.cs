﻿using System;
using System.Reactive.Linq;
using LanguageExt;
using SolidworksAddinFramework.Wpf;
using Weingartner.Exceptional;
using Weingartner.Exceptional.Reactive;

namespace SolidworksAddinFramework
{
    public class NoneException : Exception
    {
        public NoneException() : base("None")
        {
        }
    }

    public static class ObservableExceptionalExtensions
    {
        public static IObservableExceptional<T> ToObservableExceptionalFlatten<T>(this IObservable<Option<T>> o)
        {
            return o.Select(ToExceptional).ToObservableExceptional();
        }

        private static IExceptional<T> ToExceptional<T>(this Option<T> vo)
        {
            return vo.Match(Exceptional.Ok, () => Exceptional.Fail<T>(new NoneException()));
        }

        /// <summary>
        /// Converts None to a NoneException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="o"></param>
        /// <param name="sel"></param>
        /// <returns></returns>
        public static IObservableExceptional<T> SelectOptionalFlatten<T, U>(this IObservableExceptional<U> o, Func<U, Option<T>> sel)
        {
            return o
                .Select(v => sel(v).ToExceptional())
                .Observable
                .Select(v => v.SelectMany(e0 => e0))
                .ToObservableExceptional();
        }
        public static void Show(this Exception e)
        {
            // Exceptions viewer must be shown on STA thread.
            LogViewer.Invoke(()=>
            {
                var exceptionReporter = new ExceptionReporting.ExceptionReporter();
                exceptionReporter.Show(e);
            });
        }
    }
}
