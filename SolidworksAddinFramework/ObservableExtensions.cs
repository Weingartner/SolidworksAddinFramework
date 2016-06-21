using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LanguageExt;
using LanguageExt.SomeHelp;
using SolidworksAddinFramework.Reflection;
using SolidworksAddinFramework.Wpf;
using SolidWorks.Interop.sldworks;
using Weingartner.Exceptional.Reactive;
using Unit = System.Reactive.Unit;

namespace SolidworksAddinFramework
{
    public static class ObservableExtensions
    {
        #region SubscribeDisposable
        /// <summary>
        /// Subscribes to the observable sequence and manages the disposables with a serial disposable. That
        /// is before the function is called again the previous disposable is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IDisposable SubscribeDisposable<T>(this IObservableExceptional<T> o, Func<T, IDisposable> fn, Action<Exception> errHandler)
        {
            var d = new SerialDisposable();

            var s = o.Subscribe(v =>
            {
                    d.Disposable = Disposable.Empty;
                    d.Disposable = fn(v) ?? Disposable.Empty;
            }, onError:errHandler);

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
        public static IDisposable SubscribeDisposable<T>(this IObservableExceptional<T> o, Func<T, IEnumerable<IDisposable>> fn, Action<Exception> errHandler)
        {
            return SubscribeDisposable(o, v =>(IDisposable) new CompositeDisposable(fn(v)),errHandler);
        }


        /// <summary>
        /// Subscribes to the observable sequence and manages the disposables with a serial disposable. That
        /// is before the function is called again the previous disposable is disposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="disposableYielder"></param>
        /// <returns></returns>
        public static IDisposable SubscribeDisposable<T>(this IObservableExceptional<T> o, Action<T, Action<IDisposable>> disposableYielder, Action<Exception> errHandler)
        {
            Func<T, IDisposable> fn2 = v =>
            {
                var c = new CompositeDisposable();
                disposableYielder(v, c.Add);
                return c;
            };
            return SubscribeDisposable(o, fn2, errHandler);
        }
        #endregion

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

        /// <summary>
        /// Using a selector bind the property to the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="obs"></param>
        /// <param name="target"></param>
        /// <param name="selector"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IDisposable AssignTo<T, S>(this IObservableExceptional<T> obs, S target, Expression<Func<S, T>> selector, Action<Exception> handler)
        {
            var proxy = selector.GetProxy(target);
            return obs.Subscribe(u => proxy.Value = u, onError: handler);
        }

        public static IDisposable AssignTo<T, S>(this IObservable<T> obs, S target, Expression<Func<S, T>> selector)
        {
            var proxy = selector.GetProxy(target);
            return obs.Subscribe(u => proxy.Value = u);
        }

        public static IObservable<bool> Not(this IObservable<bool> obs) => obs.Select(b => !b);
        public static IObservable<bool> IsSome<T>(this IObservable<Option<T>> obs) => obs.Select(b => b.IsSome);

        public static IObservable<Option<T>> IfTrue<T>(this IObservable<bool> obs, T t) => obs.Select(v => v.IfTrue(t));
        public static IObservable<Option<T>> IfTrue<T>(this IObservable<bool> obs, Func<T> t) => obs.Select(v => v.IfTrue(t));


        public static IDisposable AssignTo<T,S,U>(this IObservable<U> obs, Func<U, T> fn, S target, Expression<Func<S, T>> selector)
        {
            var proxy = selector.GetProxy(target);
            return obs.Subscribe(u => proxy.Value = fn(u));
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
        public static IConnectableObservableExceptional<T> Connect<T>(this IConnectableObservableExceptional<T> observable, CompositeDisposable d)
        {
            var d0 = observable.Connect();
            d.Add(d0);
            return observable;
        }

        public static IDisposable SubscribeDisposableRender<T>(this IObservableExceptional<T> o, Func<T, IDisposable> fn, IModelDoc2 doc, Action<Exception> errHandler)
        {


            var d = new SerialDisposable();

            var s = o
                .Subscribe(onNext:v =>
            {
                using(OpenGlRenderer.DeferRedraw(doc))
                {
                    try
                    {
                        d.Disposable = Disposable.Empty;
                        d.Disposable = fn(v) ?? Disposable.Empty;
                    }
                    catch (Exception e)
                    {
                        d.Disposable = Disposable.Empty;
                        errHandler(e);
                    }
                }
            },
            onError: e =>
            {
                d.Disposable = Disposable.Empty;
                errHandler(e);
            });

            return new CompositeDisposable(s,d);

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
        public static IObservableExceptional<T> SampleAlways<T, U>(this IObservableExceptional<T> sampled, IObservable<U> sampler)
        {
            return ObservableExceptional.Create<T>(observer =>
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

        public static IObservable<T> CatchAndRetry<T>(this IObservable<T> @this, Action<Exception> handler)
        {
            return @this.Catch<T, Exception>(e =>
            {
                handler(e);
                return @this.CatchAndRetry(handler);
            });
        }


        /// <summary>
        /// <para>Observe on UIDispatcherScheduler.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// If a new value arrives before the selector is finished calculated the old one then it is canceled.
        /// 
        /// This will also raise a message box if an exception is thrown downstream.
        /// </summary>
        public static IObservableExceptional<U> ObserveOnSolidworksThread<T,U>(this IObservableExceptional<T> source, Func<T,CancellationToken,U> selector )
        {
            return source
                .StartWith(default(T))
                .Select(s => new {s, cts = new CancellationTokenSource()})
                .Buffer(2, 1).Where(b => b.Count == 2)
                .Select
                (b =>
                {
                    b[0].cts.Cancel();
                    return b[1];
                })
                .ObserveOn(UiDispatcherScheduler.Default)
                .Select(b => selector(b.s, b.cts.Token));
        }
        public static IObservableExceptional<Task> ObserveOnSolidworksThread<T>(this IObservableExceptional<T> source, Func<T,CancellationToken,Task> selector )
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
                    catch (OperationCanceledException )
                    {
                        Console.WriteLine("Operation cancelled");
                    }
                });
        }


        public static IDisposable SubscribeAndReportExceptions<T>(this IObservableExceptional<T> @this)
            => @this.Subscribe(onNext:v=> {}, onError: e => e.Show());

        public static IObservableExceptional<T> ObserveOnSolidworksThread<T>(this IObservableExceptional<T> source) =>
            source
            .ObserveOn(UiDispatcherScheduler.Default);

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