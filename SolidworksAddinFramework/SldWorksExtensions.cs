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
            var d = new CompositeDisposable();
            swApp.DocOpenObservable()
                .SelectDisposable(d, action, (doc, disposable) => doc
                    .DestroyNotify2Observable()
                    .FirstAsync()
                    .Select(_ => disposable)
                )
                .SelectMany(p => p)
                .Subscribe(disposable => disposable.Dispose())
                .DisposeWith(d);
            return d;
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
                if (!visible)
                    sldWorks.DocumentVisible(false, (int)type);
                var spec = (IDocumentSpecification)sldWorks.GetOpenDocSpec(toolFile);
                if (!visible)
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
                if (!visible)
                    sldWorks.DocumentVisible
                        (true,
                            (int)
                                type);

            }
        }

        /// <summary>
        /// Tries to load an iges file invisibly. Throws an exception if it doesn't work.
        /// </summary>
        /// <param name="sldWorks"></param>
        /// <param name="igesFile"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public static ModelDoc2 LoadIgesInvisible(this ISldWorks sldWorks, string igesFile, bool visible = false)
        {
            var swDocPart = (int)swDocumentTypes_e.swDocPART;

            try
            {
                if (!visible)
                    sldWorks.DocumentVisible(false, swDocPart);

                ImportIgesData swImportData =
                    (ImportIgesData)SwAddinBase.Active.SwApp.GetImportFileData(igesFile);

                int err = 0;
                var newDoc = SwAddinBase.Active.SwApp.LoadFile4(igesFile, "r", swImportData, ref err);
                if (err != 0)
                    throw new Exception(@"Unable to load file {igesFile");

                return newDoc;
            }
            finally
            {
                if (!visible)
                    sldWorks.DocumentVisible
                        (true,
                            swDocPart);

            }
        }

        public static ModelDoc2 CreateHiddenDocument
            (this ISldWorks sldWorks, bool hidden = true, swDocumentTypes_e type = swDocumentTypes_e.swDocPART)
        {
            try
            {

                if (hidden)
                    sldWorks.DocumentVisible(false, (int)type);

                var partTemplateName = sldWorks.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
                var doc = (ModelDoc2)sldWorks.NewDocument(partTemplateName, (int)swDwgPaperSizes_e.swDwgPaperA4size, 1, 1);
                doc.Visible = false;

                /*
                ModelView myModelView = null;
                myModelView = ((ModelView)(doc.ActiveView));
                myModelView.FrameLeft = 0;
                myModelView.FrameTop = 0;
                myModelView = ((ModelView)(doc.ActiveView));

                myModelView.FrameState = ((int)(swWindowState_e.swWindowMinimized));
                */
                return doc;
            }
            finally
            {
                if (hidden)
                    sldWorks.DocumentVisible
                        (true,
                            (int)
                                type);

            }

        }
    }
}
