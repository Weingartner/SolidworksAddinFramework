using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using SolidWorks.Interop.sldworks;
using Unit = System.Reactive.Unit;

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
        public static IDisposable SubscribeDisposable<T>(this IObservable<T> o, Func<T, IDisposable> fn)
        {
            var d = new SerialDisposable();

            var s = o.Subscribe(v =>
            {
                    d.Disposable = Disposable.Empty;
                    d.Disposable = fn(v) ?? Disposable.Empty;
            });

            return new CompositeDisposable(s,d);

        }
        public static IDisposable SubscribeDisposableRender<T>(this IObservable<T> o, Func<T, IDisposable> fn, IModelDoc2 doc)
        {

            var d = new SerialDisposable();

            var s = o.Subscribe(v =>
            {
                using(OpenGlRenderer.DeferRedraw(doc))
                {
                    d.Disposable = Disposable.Empty;
                    d.Disposable = fn(v) ?? Disposable.Empty;
                }
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
        /// This behaves like the normal sample except that it will always generate
        /// an event when the sampler is triggered even if the input stream has not changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="sampled"></param>
        /// <param name="sampler"></param>
        /// <returns></returns>
        public static IObservable<T> SampleAlways<T, U>(this IObservable<T> sampled, IObservable<U> sampler)
        {
            return Observable.Create<T>(observer =>
            {
                var started = false;
                var value = default(T);
                var d0 = sampled.Subscribe(v =>
                {
                    value = v;
                    started = true;
                });
                var d1 = sampler.Subscribe(s =>
                {
                    if(started)
                        observer.OnNext(value);
                });
                return new CompositeDisposable(d0,d1);
            });
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
        public static IObservable<Unit> FromEvent(Action<Func<int>> add, Action<Func<int>> remove)
        {
            Func<Action<Unit>, Func<int>> conversion = action =>
            {
                return () =>
                {
                    action(Unit.Default);
                    return 0;
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }

        public static IObservable<Unit> FromEvent<T>(Action<Func<T>> add, Action<Func<T>> remove)
        {
            Func<Action<Unit>, Func<T>> conversion = action =>
            {
                return () =>
                {
                    action(Unit.Default);
                    return default(T);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }

        public static IObservable<TA> FromEvent<T,TA>(Action<Func<TA, T>> add, Action<Func<TA, T>> remove)
        {
            Func<Action<TA>, Func<TA, T>> conversion = action =>
            {
                return arg =>
                {
                    action(arg);
                    return default(T);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }

        /*
        public static IObservable<Tuple<T0,T1>> FromEvent<T0,T1,TA>(Action<Func<T0,T1,TA>> add, Action<Func<T0,T1,TA>> remove)
        {
            Func<Action<Tuple<T0,T1>>, Func<T0,T1, TA>> conversion = action =>
            {
                return (t0,t1)=>
                {
                    action(Tuple.Create(t0,t1));
                    return default(TA);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }

        public static IObservable<Tuple<T0,T1,T2>> FromEvent<T0,T1,T2,TA>(Action<Func<T0,T1,T2,TA>> add, Action<Func<T0,T1,T2,TA>> remove)
        {
            Func<Action<Tuple<T0,T1,T2>>, Func<T0,T1,T2, TA>> conversion = action =>
            {
                return (t0,t1,t2)=>
                {
                    action(Tuple.Create(t0,t1,t2));
                    return default(TA);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }

        public static IObservable<Tuple<T0,T1,T2, T3>> FromEvent<T0,T1,T2, T3,TA>(Action<Func<T0,T1,T2, T3,TA>> add, Action<Func<T0,T1,T2, T3,TA>> remove)
        {
            Func<Action<Tuple<T0,T1,T2, T3>>, Func<T0,T1,T2, T3, TA>> conversion = action =>
            {
                return (t0,t1,t2,t3)=>
                {
                    action(Tuple.Create(t0,t1,t2,t3));
                    return default(TA);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }
        public static IObservable<Tuple<T0,T1,T2, T3, T4>> FromEvent<T0,T1,T2, T3, T4,TA>(Action<Func<T0,T1,T2, T3, T4,TA>> add, Action<Func<T0,T1,T2, T3, T4,TA>> remove)
        {
            Func<Action<Tuple<T0,T1,T2, T3, T4>>, Func<T0,T1,T2, T3, T4, TA>> conversion = action =>
            {
                return (t0,t1,t2,t3,t4)=>
                {
                    action(Tuple.Create(t0,t1,t2,t3,t4));
                    return default(TA);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }
        public static IObservable<Tuple<T0,T1,T2, T3, T4, T5>> FromEvent<T0,T1,T2, T3, T4, T5,TA>(Action<Func<T0,T1,T2, T3, T4, T5,TA>> add, Action<Func<T0,T1,T2, T3, T4, T5,TA>> remove)
        {
            Func<Action<Tuple<T0,T1,T2, T3, T4, T5>>, Func<T0,T1,T2, T3, T4, T5, TA>> conversion = action =>
            {
                return (t0,t1,t2,t3,t4,t5)=>
                {
                    action(Tuple.Create(t0,t1,t2,t3,t4,t5));
                    return default(TA);
                };
            };
            return Observable.FromEvent(conversion, add, remove);
        }
        */

        public static IObservable<T> WhereIsSome<T>(this IObservable<Option<T>> q)
        {
            return q.SelectMany(o => o.MatchObservable(Observable.Return<T>, Observable.Empty<T>));
        }
    }
}