using System;
using System.Collections.Generic;
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

            var group = Page.CreateGroup(1, "Controls", new [] { swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded ,
                swAddGroupBoxOptions_e.swGroupBoxOptions_Visible});

            yield return CreateLabel(group, "Select test style", "Select test style");

            yield return CreateOptionGroup
                (group,
                 MacroFeature.Database,
                 p => p.Style,
                 config =>
                 {
                     config.CreateOption("Solid", MulitWireBodiesData.StyleEnum.Solid);
                     config.CreateOption("Wire", MulitWireBodiesData.StyleEnum.Wire);
                 });

        }

    }
}
