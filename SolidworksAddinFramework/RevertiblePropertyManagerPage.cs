using System.Collections.Generic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// This page works with live Data. Changes made in the
    /// UI are immediately reflected in the object passed
    /// in. If OnCancel is called then the live data is reverte
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RevertiblePropertyManagerPage<T> : ReactiveObjectPropertyManagerPageBase<T>
        where T : ReactiveUI.ReactiveObject
    {
        private T _Original;

        protected RevertiblePropertyManagerPage(
            string name,
            IEnumerable<swPropertyManagerPageOptions_e> optionsE,
            IModelDoc2 modelDoc,
            T data) : base(name, optionsE, modelDoc, data)
        {
        }

        protected override void OnShow()
        {
            CloneData();
        }

        protected override void OnCancel()
        {
            using (Data.DelayChangeNotifications())
            {
                Json.Copy(_Original, Data);
            }
        }

        protected override void OnCommit()
        {
            CloneData();
        }

        private void CloneData()
        {
            _Original = Json.Clone(Data);
        }
    }
}