using System;
using System.Reactive.Disposables;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class ModelViewManagerExtensions
    {
        public static IDisposable CreateSectionView(this IModelViewManager modelViewManager, Action<SectionViewData> config)
        {
            var data = modelViewManager.CreateSectionViewData();
            config(data);
            if (!modelViewManager.CreateSectionView(data))
            {
                throw new Exception("Error while creating section view.");
            }
            // TODO `modelViewManager.RemoveSectionView` always returns `false` and doesn't remove the section view
            // In 2011 this seems to have worked (see https://forum.solidworks.com/thread/47641)
            return Disposable.Create(() => modelViewManager.RemoveSectionView());
        }
    }
}