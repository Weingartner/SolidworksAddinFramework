using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace SolidworksAddinFramework
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Subscribes to the observable sequence and manages the disposables with a serial disposable. That
        /// is before the function is called again the previous disposable is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IDisposable SubscribeDisposable<T>(this IObservable<T> o, Func<T, IDisposable> fn )
        {
            var d = new SerialDisposable();

            var s = o.Subscribe(v =>
            {
                d.Disposable = Disposable.Empty;
                d.Disposable = fn(v);
            });

            return new CompositeDisposable(s,d);

        }

        /// <summary>
        /// Subscribes to the observable sequence and manages the disposables with a serial disposable. That
        /// is before the function is called again the previous disposable is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IDisposable SubscribeDisposable<T>(this IObservable<T> o, Func<T, IEnumerable<IDisposable>> fn)
        {
            return SubscribeDisposable<T>(o, v =>(IDisposable) new CompositeDisposable(fn(v)));
        }

        /// <summary>
        /// Subscribes to the observable sequence and manages the disposables with a serial disposable. That
        /// is before the function is called again the previous disposable is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="disposableYielder"></param>
        /// <returns></returns>
        public static IDisposable SubscribeDisposable<T>(this IObservable<T> o, Action<T, Action<IDisposable>> disposableYielder)
        {
            Func<T, IDisposable> fn2 = v =>
            {
                var c = new CompositeDisposable();
                disposableYielder(v, c.Add);
                return c;
            };
            return SubscribeDisposable(o, fn2);
        }

        public static IObservable<TOut> SelectAsync<TIn, TOut>(this IObservable<TIn> o, Func<TIn, CancellationToken, Task<TOut>> selector)
        {
            return o
                .Select(x =>
                    Observable.DeferAsync(async ct =>
                    {
                        try
                        {
                            var result = await selector(x, ct);
                            return Observable.Return(result);
                        }
                        catch (OperationCanceledException)
                        {
                            return Observable.Empty<TOut>();
                        }
                    })
                )
                .Switch();
        }
    }
}