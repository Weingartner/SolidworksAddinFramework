using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using Weingartner.Exceptional.Reactive;

namespace SolidworksAddinFramework
{
    public static class ObservableExtensionsSW
    {
        public static IObservableExceptional<T> ObserveOnSolidworksThread<T>(this IObservableExceptional<T> source) =>
            source
            .ObserveOn(SolidworksSchedular.Default);

        public static IObservable<T> ObserveOnSolidworksThread<T>(this IObservable<T> source) =>
            source
            .ObserveOn(SolidworksSchedular.Default);

        public static IDisposable SubscribeDisposableRender<T>(this IObservableExceptional<T> o, Func<T, IDisposable> fn, IModelDoc2 doc, Action<Exception> errHandler)
        {


            var d = new SerialDisposable();

            var s = o
                .ObserveOnSolidworksThread()
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
    }
}
