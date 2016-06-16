using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Threading;
using LanguageExt;
using ReactiveUI;
using Weingartner.Exceptional;
using Weingartner.Exceptional.Async;
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
        public static IObservableExceptional<T> StartWith<T>(this IObservableExceptional<T> o, T v)
        {
            return o.Observable.StartWith(Exceptional.Ok(v)).ToObservableExceptional();
        }

        public static IObservableExceptional<T> ToObservableExceptional<T>(this IObservable<Option<T>> o)
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
        public static IObservableExceptional<T> Select<T, U>(this IObservableExceptional<U> o, Func<U, Option<T>> sel)
        {
            return o
                .Select(v => sel(v).ToExceptional())
                .Observable
                .Select(v => v.SelectMany(e0 => e0))
                .ToObservableExceptional();
        }

        public static IObservableExceptional<T> Throttle<T>(this IObservableExceptional<T> o, TimeSpan t) =>
            o.Observable.Throttle(t).ToObservableExceptional();

        public static IObservableExceptional<T> Switch<T>(this IObservableExceptional<Task<T>> o) =>
            o.Observable
            .Select(te => te.Select(t => t.ToTaskOfExceptional().ToObservable().ToObservableExceptional()))
            .ToObservableExceptional()
            .Switch();


        public static IObservableExceptional<Task<Unit>> Switch(this IObservableExceptional<Task> o) =>
            o.Select
                (async t =>
                {
                    await t;
                    return Unit.Default;
                });

        public static IObservableExceptional<T> TakeWhile<T>(this IObservableExceptional<T> o, Func<T, bool> predicate)
        {
            return o.Observable.TakeWhile(v => v.HasException || predicate(v.Value)).ToObservableExceptional();
        }

        public static IObservableExceptional<T> Finally<T>(this IObservableExceptional<T> o, Action fn) =>
            o.Observable.Finally(fn).ToObservableExceptional();

        public static Task<IExceptional<T>> ToTask<T>(this IObservableExceptional<T> o, CancellationToken t) =>
            o.Observable.ToTask(t);

        public static IObservable<bool> IsOk<T>(this IObservableExceptional<T> o) =>
            o.Observable.Select(v => !v.HasException);
        public static IObservable<T> WhereIsOk<T>(this IObservableExceptional<T> o) =>
            o.Observable.Where(v => !v.HasException).Select(v=>v.Value);

        public static void Show(this Exception e) => new ExceptionReporting.ExceptionReporter().Show(e);


                    /// <Summary>
                    /// If any of the inputs are in error then the output will contain an aggregate
                    /// exception of the inputs. If the selector function throws then the output will
                    /// contain that exception.
                    /// </Summary>
        public static IObservableExceptional<IList<T>> CombineLatest<T>
            (this IEnumerable<IObservableExceptional<T>> t1)
        {
            return Observable
                .CombineLatest(t1.Select(t => t.Observable))
                .Select
                (es =>
                {
                    var r1 = es.FirstOrDefault(e => e.HasException);
                    return r1 == null
                        ? Exceptional.Ok(es.Select(e => e.Value).ToList())
                        : Exceptional.Fail<IList<T>>(r1.Exception);
                }).ToObservableExceptional();

        }

    }
}
