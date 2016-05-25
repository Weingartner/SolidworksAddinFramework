using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LanguageExt;
using LanguageExt.SomeHelp;
using SolidworksAddinFramework.Wpf;
using SolidWorks.Interop.sldworks;
using static LanguageExt.Prelude;
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

        public static IObservable<T> Log<T>(this IObservable<T> o)
        {
            return o.Do(v => LogViewer.Log(v.ToString()));
        }

        public static void LoadUnloadHandler(this FrameworkElement control, Func<IEnumerable<IDisposable>> action)
        {
            LoadUnloadHandler(control, () => (IDisposable)new CompositeDisposable(action()));
        }

        public static void LoadUnloadHandler(this FrameworkElement control, Func<IDisposable> action)
        {
            var state = false;
            var cleanup = new SerialDisposable();
            Observable.Merge
                (Observable.Return(control.IsLoaded)
                , control.Events().Loaded.Select(x => true)
                , control.Events().Unloaded.Select(x => false)
                )
                .Subscribe(isLoadEvent =>
                {
                    if (!state)
                    {
                        // unloaded state
                        if (isLoadEvent)
                        {
                            state = true;
                            cleanup.Disposable = new CompositeDisposable(action());
                        }
                    }
                    else
                    {
                        // loaded state
                        if (!isLoadEvent)
                        {
                            state = false;
                            cleanup.Disposable = Disposable.Empty;
                        }
                    }

                });
        }

        public static void LoadUnloadHandler<T>(this IObservable<T> @this, FrameworkElement control, Action<T> action)
        {
            LoadUnloadHandler(control, () => @this.Subscribe(action));
        }

        /// <summary>
        /// Fluent version of connect that adds the disposable to a composite disposable
        /// and then returns the original observable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IObservable<T> Connect<T>(this IConnectableObservable<T> observable, CompositeDisposable d)
        {
            var d0 = observable.Connect();
            d.Add(d0);
            return observable;
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
        /// Allows cancellation of a call to `selector` when the input observable produces a new value.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="o"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IObservable<Unit> SelectAsync<TIn>(this IObservable<TIn> o, Func<TIn, CancellationToken, Task> selector)
        {
            Func<TIn,CancellationToken,Task<Unit>> wrapper =
                async (t, token) =>
                {
                    await selector(t, token);
                    return Unit.Default;
                };

            return o.SelectAsync(wrapper);
        }

        /// <summary>
        /// <para>Observe on UIDispatcherScheduler.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// If a new value arrives before the selector is finished calculated the old one then it is canceled.
        /// </summary>
        public static IObservable<U> ObserveOnSolidworksThread<T,U>(this IObservable<T> source, Func<T,CancellationToken,U> selector )
        {
            return source
                .StartWith(default(T))
                .Select(s => new {s, cts = new CancellationTokenSource()})
                .Buffer(2, 1).Where(b => b.Count == 2)
                .Select(b =>
                {
                    b[0].cts.Cancel();
                    return b[1];
                })
                .ObserveOn(UiDispatcherScheduler.Default)
                .Select(b => { 
                        try
                        {
                            return Optional(selector(b.s, b.cts.Token));
                        }
                        catch (OperationCanceledException e)
                        {
                            return None;
                        }
                    })
                .WhereIsSome();
        }
        public static IObservable<Task> ObserveOnSolidworksThread<T>(this IObservable<T> source, Func<T,CancellationToken,Task> selector )
        {
            return source
                .StartWith(default(T))
                .Select(s => new {s, cts = new CancellationTokenSource()})
                .Buffer(2, 1).Where(b => b.Count == 2)
                .Select(b =>
                {
                    b[0].cts.Cancel();
                    return b[1];
                })
                .ObserveOn(UiDispatcherScheduler.Default)
                .Select(async b =>
                {
                    try
                    {
                        await selector(b.s, b.cts.Token);
                    }
                    catch (OperationCanceledException e)
                    {
                        Console.WriteLine("Operation cancelled");
                    }
                });
        }

        public static IObservable<T> ObserveOnSolidworksThread<T>(this IObservable<T> source) =>
            source
            .ObserveOn(UiDispatcherScheduler.Default);

        /// <summary>
        /// <para>Subscribe on UIDispatcherScheduler.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// </summary>
        public static IObservable<T> SubscribeOnSolidworksThread<T>(this IObservable<T> source) =>
            source.SubscribeOn(UiDispatcherScheduler.Default);

        public static IObservable<Unit> Switch(this IObservable<Task> o)
        {
            return Observable.Switch(o.Select(async t =>
            {
                await t;
                return Unit.Default;
            }));
        }

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

        public static IEnumerable<T> WhereIsSome<T>(this IEnumerable<Option<T>> q)
        {
            return q.SelectMany(o => o.Match(EnumerableEx.Return, Enumerable.Empty<T>));
        }
    }
}