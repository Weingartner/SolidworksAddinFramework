using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;

namespace SolidworksAddinFramework
{
    public static class DisposableExtensions
    {
        public static IDisposable ToCompositeDisposable(this IEnumerable<IDisposable> d)
        {
            return new CompositeDisposable(d);
        }
        public static IDisposable Concat(this IDisposable d, IDisposable d1)
        {
            return new CompositeDisposable(d,d1);
        }
        public static IDisposable Concat(this IDisposable d, params IDisposable [] d1)
        {
            return new CompositeDisposable
                (new[]
                {
                    d
                }.Concat(d1));
        }


        public static T DisposeWith<T>(this T disposable, CompositeDisposable container)
            where T : IDisposable
        {
            if(disposable!=null)
                container.Add(disposable);
            return disposable;
        }
        public static void DisposeEnumerableWith<T>(this IEnumerable<T> disposable, CompositeDisposable container)
            where T : IDisposable
        {
            disposable?.ForEach(d=>container.Add(d));
        }
        public static void DisposeEnumerable<T>(this IEnumerable<T> disposable)
            where T : IDisposable
        {
            disposable?.ForEach(d=>d.Dispose());
        }

        public static IDisposable DisposeOn(this IDisposable d, IScheduler s )=>
            new ScheduledDisposable(s,d);

        public static IDisposable DisposeWith(this IDisposable d, CancellationToken token) => token.Register(d.Dispose);




        public static Func< IDisposable> CreateAndDisposeOn(this Func<IDisposable> fn, IScheduler s)
        {
            return () =>
            {
                var d = new SerialDisposable();
                s.Schedule(() => d.Disposable = fn());
                return d.DisposeOn(s);
            };

        }
        public static Func<T0, IDisposable> CreateAndDisposeOn<T0>(this Func<T0, IDisposable> fn, IScheduler s)
        {
            return t0 => CreateAndDisposeOn(()=>fn(t0),s)();
        }

        public static Func<T0,T1, IDisposable> CreateAndDisposeOn<T0,T1>(this Func<T0,T1, IDisposable> fn, IScheduler s)
        {
            return (t0,t1) => CreateAndDisposeOn(()=>fn(t0,t1),s)();

        }

    }
}
