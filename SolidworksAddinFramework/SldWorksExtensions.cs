using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidworksAddinFramework.Events;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class SldWorksExtensions
    {
        public static IObservable<IModelDoc2> DocOpenObservable(this SldWorks swApp)
        {
            return swApp.DocumentLoadNotify2Observable()
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default)
                .Select(_ => swApp.GetDocuments().CastArray<IModelDoc2>())
                .Buffer(2, 1)
                .Select(pair => pair[1].Except(pair[0]).Single());
        }

        public static IDisposable DoWithOpenDoc(this SldWorks swApp, Func<IModelDoc2, IDisposable> action)
        {
            return swApp.DocOpenObservable()
                .Select(doc =>
                {
                    var disposable = action(doc);
                    return doc
                        .DestroyNotify2Observable()
                        .Select(_ => disposable);
                })
                .Switch()
                .Subscribe(disposable => disposable.Dispose());
        } 
    }
}
