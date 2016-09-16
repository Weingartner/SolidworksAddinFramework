using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class ModelViewManagerExtensions
    {
        private static readonly ISubject<SectionViewData> _CreateSectionViewObservable = new Subject<SectionViewData>();

        public static IObservable<SectionViewData> CreateSectionViewObservable => _CreateSectionViewObservable.AsObservable();

        public static IDisposable CreateSectionView(this IModelViewManager modelViewManager, Action<SectionViewData> config)
        {
            var data = modelViewManager.CreateSectionViewData();
            config(data);
            _CreateSectionViewObservable.OnNext(data);
            if (!modelViewManager.CreateSectionView(data))
            {
                throw new Exception("Error while creating section view.");
            }
            // TODO `modelViewManager.RemoveSectionView` returns `false` and doesn't remove the section view
            // when `SectionViewData::GraphicsOnlySection` is `true`
            return Disposable.Create(() => modelViewManager.RemoveSectionView());
        }
    }
}