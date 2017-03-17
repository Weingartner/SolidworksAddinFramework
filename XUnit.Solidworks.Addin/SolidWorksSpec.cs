using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace XUnit.Solidworks.Addin
{
    public abstract class SolidWorksSpec
    {
        protected static SldWorks SwApp => SwAddinBase.Active.SwApp;
        protected static IModeler Modeler => (IModeler)SwApp.GetModeler();
        protected static IMathUtility MathUtility => (IMathUtility)SwApp.GetMathUtility();

        /// <summary>
        /// Create a part using the standard template
        /// </summary>
        /// <returns></returns>
        protected IModelDoc2 CreatePartDoc()
        {
            var partTemplateName = SwApp.GetUserPreferenceStringValue((int) swUserPreferenceStringValue_e.swDefaultTemplatePart);
            return (IModelDoc2)SwApp.NewDocument(partTemplateName, 0, 0, 0);
        }

        protected IModelDoc2 CreatePartDoc(string path)
        {
            Path.IsPathRooted(path).Should().BeTrue("SW working directory can switch");

            var type = (int) swDocumentTypes_e.swDocPART;
            int options = (int) swOpenDocOptions_e.swOpenDocOptions_LoadModel;
            var configuration = "";
            int errors = 0;
            int warnings = 0;
            return SwApp.OpenDoc6(path, type, options, configuration, ref errors, ref warnings);
        }

        /// <summary>
        /// Create a part doc which will be deleted after the action even if the test fails
        /// </summary>
        /// <param name="action"></param>
        protected void CreatePartDoc(Action<IModelDoc2> action)
        {
            CreatePartDoc().Using(SwApp, action);
        }
        protected Task CreatePartDoc(Func<IModelDoc2,Task> action)
        {
            return CreatePartDoc().Using(SwApp, action);
        }

        /// <summary>
        /// Create a part doc which will be deleted if keep is false. This is mainly useful
        /// just for debugging so that the document is kept open after a test is run so you
        /// can eyeball the results.
        /// </summary>
        /// <param name="action"></param>
        protected void CreatePartDoc(Func<IModelDoc2, IDisposable> action)
        {
            var doc = CreatePartDoc();
            Debug.Assert(doc!=null);
            doc.Using(SwApp, m => action(m).Dispose());
        }
        protected void CreatePartDocWithTitle(string title, Func<IModelDoc2, IDisposable> action)
        {
            var doc = CreatePartDoc();
            doc.SetTitle2(title);
            doc.Using(SwApp, m => action(m).Dispose());
        }
        protected async Task CreatePartDoc(Func<IModelDoc2, Task<IDisposable>> action)
        {
            await CreatePartDoc().Using(SwApp, async m => (await action(m)).Dispose());
        }

        protected void CreatePartDoc(string path, Func<IModelDoc2, IDisposable> action)
        {
            CreatePartDoc(path).Using(SwApp, m => action(m).Dispose());
        }
        protected async Task CreatePartDoc(string path, Func<IModelDoc2, Task<IDisposable>> action)
        {
            await CreatePartDoc(path).Using(SwApp, async m => (await action(m)).Dispose());
        }
        protected void CreatePartDoc(Action<IModelDoc2, Action<IDisposable>> action)
        {
            CreatePartDoc(doc =>
            {
                var comp = new CompositeDisposable();
                Action<IDisposable> yielder = d => comp.Add(d);
                action(doc, yielder);
                return comp;

            });
        }
        protected Task CreatePartDoc(Func<IModelDoc2, Action<IDisposable>,Task> action)
        {
            return CreatePartDoc(async doc =>
             {
                 var comp = new CompositeDisposable();
                 Action<IDisposable> yielder = d => comp.Add(d);
                 await action(doc, yielder);
                 return comp;

             });
        }

        private static readonly ISubject<Unit> Subject = new Subject<Unit>();
        public static bool CanContinueTestExecution { get; private set; }
        public static void ContinueTestExecution()
        {
            Subject.OnNext(Unit.Default);
        }

        public static async Task PauseTestExecution(bool pause=true)
        {
#if DEBUG
            if (!pause)
                return;
            CanContinueTestExecution = true;
            using (Disposable.Create(() => CanContinueTestExecution = false))
            {
                await Subject.FirstAsync();
            }
#else
            await Task.CompletedTask;
#endif
        }
    }
}
