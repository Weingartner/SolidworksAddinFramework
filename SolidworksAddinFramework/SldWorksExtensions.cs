using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidworksAddinFramework.Events;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

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

        public static IDisposable DoWithOpenDoc(this SldWorks swApp, Action<IModelDoc2, Action<IDisposable>> action)
        {
            Func<IModelDoc2, IDisposable> func = doc =>
            {
                var disposables = new CompositeDisposable();
                action(doc, disposables.Add);
                return disposables;
            };
            return swApp.DoWithOpenDoc(func);
        }

        /// <summary>
        /// Open a document invisibly. It will not be shown to the user but you will be
        /// able to interact with it through the API as if it is loaded.
        /// </summary>
        /// <param name="sldWorks"></param>
        /// <param name="toolFile"></param>
        /// <returns></returns>
        public static ModelDoc2 OpenInvisibleReadOnly(this ISldWorks sldWorks, string toolFile, swDocumentTypes_e type = swDocumentTypes_e.swDocPART)
        {
            try
            {
                sldWorks.DocumentVisible(false, (int)type);
                var spec = (IDocumentSpecification)sldWorks.GetOpenDocSpec(toolFile);
                spec.Silent = true;
                spec.ReadOnly = true;
                var doc = SwAddinBase.Active.SwApp.OpenDoc7(spec);
                doc.Visible = false;
                return doc;
            }
            finally
            {
                sldWorks.DocumentVisible
                    (true,
                        (int)
                            type);

            }
        }
    }
}
