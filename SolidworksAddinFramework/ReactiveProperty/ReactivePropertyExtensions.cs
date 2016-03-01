using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Reactive.Bindings;

namespace SolidworksAddinFramework.ReactiveProperty
{
    public static class ReactivePropertyExtensions
    {
        /// <summary>
        /// Generates an observable which starts with the initial value of the reactive property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IObservable<T> WhenAnyValue<T>(this ReactiveProperty<T> @this)
        {
            return @this.StartWith(@this.Value);
        } 
    }
}
