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
                .Select(args => swApp
                    .GetDocuments()
                    .CastArray<IModelDoc2>()
                    .Single(doc => doc.GetTitle() == args.docTitle && doc.GetPathName() == args.docPath)
                );
        }

        public static IDisposable DoWithOpenDoc(this SldWorks swApp, Func<IModelDoc2, IDisposable> action)
        {
            return swApp.DocOpenObservable()
                .SelectMany(doc =>
                {
                    var disposable = action(doc);
                    return doc
                        .DestroyNotify2Observable()
                        .FirstAsync()
                        .Select(_ => disposable);
                })
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
        public static ModelDoc2 OpenInvisibleReadOnly(this ISldWorks sldWorks, string toolFile, bool visible = false, swDocumentTypes_e type = swDocumentTypes_e.swDocPART)
        {
            try
            {
                if(!visible)
                    sldWorks.DocumentVisible(false, (int)type);
                var spec = (IDocumentSpecification)sldWorks.GetOpenDocSpec(toolFile);
                if(!visible)
                {
                    spec.Silent = true;
                    spec.ReadOnly = true;
                }
                var doc = SwAddinBase.Active.SwApp.OpenDoc7(spec);

                doc.Visible = visible;
                return doc;
            }
            finally
            {
                if(!visible)
                    sldWorks.DocumentVisible
                        (true,
                            (int)
                                type);

            }
        }
    }
}
