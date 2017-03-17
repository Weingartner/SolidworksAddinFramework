using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class FeatureMgrViewObservables
    {
        public static IObservable<bool> VisibilityObservable(this FeatMgrView @this)
        {
            return Observable.Create<bool>
            (observer =>
            {
                SolidWorks.Interop.sldworks.DFeatMgrViewEvents_ActivateNotifyEventHandler activateCallback =
                    (ref object view) => {
                        observer.OnNext(true);
                        return default(System.Int32);
                    };

                @this.ActivateNotify += activateCallback;

                SolidWorks.Interop.sldworks.DFeatMgrViewEvents_DeactivateNotifyEventHandler deactivateCallback =
                    (ref object view) => {
                        observer.OnNext(false);
                        return default(System.Int32);
                    };

                @this.DeactivateNotify += deactivateCallback;

                return Disposable.Create(() =>
                {
                    @this.ActivateNotify -= activateCallback;
                    @this.DeactivateNotify -= deactivateCallback;
                });

            }
            );
        }
    }
}
