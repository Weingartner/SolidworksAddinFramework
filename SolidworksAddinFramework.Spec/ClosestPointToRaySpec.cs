using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using SolidworksAddinFramework.Geometry;
using Xunit;
using XUnit.Solidworks.Addin;

namespace SolidworksAddinFramework.Spec
{
    public class ClosestPointToRaySpec : SolidWorksSpec
    {
        [SolidworksFact]
        public void CanFindTheClosestPointOnCurveToRay()
        {
            CreatePartDoc(false, modelDoc =>
            {
                var ray = new PointDirection3(new Vector3(1,0,0),Vector3.UnitZ);
                var mod = SwAddinBase.Active.Modeler;
                return new CompositeDisposable();
            };
        }

    }
}
