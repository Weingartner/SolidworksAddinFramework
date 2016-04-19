using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using XUnitRemote.Local;

namespace XUnit.Solidworks.Addin
{
    public abstract class SolidWorksSpec
    {
        private static CompositeDisposable _keptStuff = new CompositeDisposable();
        protected static ISldWorks SwApp => (ISldWorks)XUnitService.Data[nameof(ISldWorks)];

        /// <summary>
        /// Create a part using the standard template
        /// </summary>
        /// <returns></returns>
        protected IModelDoc2 CreatePartDoc()
        {
            var partTemplateName = SwApp.GetUserPreferenceStringValue((int) swUserPreferenceStringValue_e.swDefaultTemplatePart);
            return (IModelDoc2)SwApp.NewDocument(partTemplateName, 0, 0, 0);
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
        /// <param name="keep"></param>
        /// <param name="action"></param>
        protected void CreatePartDoc(bool keep, Func<IModelDoc2, IDisposable> action)
        {
            if (keep)
            {
                var doc =CreatePartDoc();
                _keptStuff.Add(action(doc));
            }
            else
            {
                CreatePartDoc().Using(SwApp, m => action(m).Dispose());
            }
        }
        protected void CreatePartDoc(bool keep, Action<IModelDoc2, Action<IDisposable>> action)
        {
            CreatePartDoc(keep, doc =>
            {
                var comp = new CompositeDisposable();
                Action<IDisposable> yielder = d => comp.Add(d);
                action(doc, yielder);
                return comp;

            });
        }
    }
}
