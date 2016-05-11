using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using DModelViewEvents_Event = SolidworksAddinFramework.Events.DModelViewEvents_Event;

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
