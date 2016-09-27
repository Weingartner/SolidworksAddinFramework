using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace WeinCadSW.MacroFeatures.CurveBender
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IPropertyManagerPage2Handler9))]
    class MultiWireBodiesPmp : MacroFeaturePropertyManagerPageBase<DemoMacroFeatures.MultiWireBodies.MultiWireBodies, MulitWireBodiesData>
    {
        private IPropertyManagerPageGroup _PageGroup;
        public int Group1Id { get; set; }

        private static IEnumerable<swPropertyManagerPageOptions_e> Options => new[]
                                                                              {
                                                                                  swPropertyManagerPageOptions_e
                                                                                      .swPropertyManagerOptions_OkayButton,
                                                                                  swPropertyManagerPageOptions_e
                                                                                      .swPropertyManagerOptions_CancelButton
                                                                              };

        public MultiWireBodiesPmp(DemoMacroFeatures.MultiWireBodies.MultiWireBodies feature)
            : base(Options, feature)
        {
        }

        protected override IEnumerable<IDisposable> AddControlsImpl()
        {

            yield break;

        }

    }
}
