using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
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
            return SubscribeDisposable(o, v =>(IDisposable) new CompositeDisposable(fn(v)));
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

        /// <summary>
        /// Allows cancellation of a call to `selector` when the input observable produces a new value.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="o"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
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

            /// <summary>
            /// <para>Observe on UIDispatcherScheduler.</para>
            /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
            /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
            /// </summary>
            public static IObservable<T> ObserveOnUiDispatcher<T>(this IObservable<T> source) =>
                source.ObserveOn(UiDispatcherScheduler.Default);

            /// <summary>
            /// <para>Subscribe on UIDispatcherScheduler.</para>
            /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
            /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
            /// </summary>
            public static IObservable<T> SubscribeOnUiDispatcher<T>(this IObservable<T> source) =>
                source.SubscribeOn(UiDispatcherScheduler.Default);

        /// <summary>
        /// A helper for attaching observables to solidworks events with delegates that have <![CDATA[Func<T>]]> type signitures.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type you want to use</typeparam>
        /// <param name="add"></param>
        /// <param name="remove"></param>
        /// <param name="delegateCreator">This should be able to create the delegate from an action</param>
        /// <returns></returns>
        public static IObservable<Unit> FromEvent<TDelegate>(Action<TDelegate> add, Action<TDelegate> remove, Func<Func<int>, TDelegate> delegateCreator )
        {
            Debug.Assert(typeof(Delegate).IsAssignableFrom(typeof(TDelegate)));

            return Observable.Create<Unit>(observer =>
            {

                Func<int> callback =  () =>
                {
                    observer.OnNext(Unit.Default);
                    return 0;
                };

                var setup = add;
                var teardown = remove;

                var convertedCallback = delegateCreator(callback);
                setup(convertedCallback);
                return Disposable.Create(() => teardown(convertedCallback)) ;

            });

        }
    }
}