using System;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public abstract class ReactiveObjectPropertyManagerPage<T> : PropertyManagerPageBase
        where T : ReactiveUI.ReactiveObject
    {
        protected T Data { get; }
        private readonly T _Original;


        protected ReactiveObjectPropertyManagerPage
            (string name
                , IEnumerable<swPropertyManagerPageOptions_e> optionsE
                , ISldWorks swApp
                , IModelDoc2 modelDoc
                , T data) : base(name, optionsE, swApp, modelDoc)
        {
            _Original = data;
            Data = Json.JsonClone(_Original);
        }

        protected override IDisposable PushSelections()
        {
            return ModelDoc.PushSelections(Data);
        }

        protected override void OnClose(swPropertyManagerPageCloseReasons_e reason)
        {
            switch (reason)
            {
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay:
                    using (Data.DelayChangeNotifications())
                        Json.JsonCopyTo(Data, _Original);
                    break;
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
            }
        }
    }
}