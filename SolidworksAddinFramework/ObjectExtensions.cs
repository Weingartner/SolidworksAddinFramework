using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SolidworksAddinFramework
{
    public static class ObjectExtensions
    {
        public static void Using<T>(this T @this, Action<T> cleanup, Action<T> run)
        {
            using (Disposable.Create(() => cleanup(@this)))
                run(@this);
        }
        public static async Task Using<T>(this T @this, Action<T> cleanup, Func<T,Task> run)
        {
            using (Disposable.Create(() => cleanup(@this)))
                await run(@this);
        }
        public static R Using<T,R>(this T @this, Action<T> cleanup, Func<T,R> run)
        {
            using (Disposable.Create(() => cleanup(@this)))
                return run(@this);
        }
        public static Task<R> Using<T,R>(this T @this, Action<T> cleanup, Func<T,Task<R>> run)
        {
            using (Disposable.Create(() => cleanup(@this)))
                return run(@this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TR DirectCast<TR>(this object u)
        {
            return (TR) u ;
        }

        public static T[] CastArray<T>(this object o)
        {
            if (o == null || o is System.DBNull)
                return new T[0];
            {
                var objects = o as T[];
                if (objects != null)
                    return objects;
            }

            {
                var objects = o as IEnumerable;
                if (objects != null)
                {
                    var objectarray = objects.Cast<object>().ToArray();
                    return Array.ConvertAll(objectarray, q =>
                    {
                        if (q is T)
                            return (T) q;
                        return (T) Convert.ChangeType(q, typeof (T));
                        
                    });

                }
            }

            throw new InvalidCastException($"Cannot cast {o.GetType().Name} to {typeof(T).Name} ");
        }
    }
}
