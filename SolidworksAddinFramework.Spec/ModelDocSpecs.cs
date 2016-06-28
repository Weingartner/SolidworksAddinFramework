using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using XUnit;
using XUnit.Solidworks.Addin;
using SolidworksAddinFramework.Events;

namespace SolidworksAddinFramework.Spec
{
    public class ModelDocSpecs : SolidWorksSpec
    {
        /// <summary>
        /// Interactive test example. 
        /// </summary>
        /// <returns></returns>
        [SolidworksFact]
        public void CanSelect()
        {
            CreatePartDoc(doc =>
            {
                var modeller = (IModeler) SwApp.GetModeler();
                var box = modeller.CreateBox(0.1, 0.1, 0.1);
                var part = (PartDoc) doc;
                part.CreateFeatureFromBody3(box, false, 0);

                new Action(() =>
                {
                        doc.SelectionObservable((selectTypeE, mark) => true)
                            .FirstAsync()
                            .Timeout(TimeSpan.FromSeconds(5))
                            .Wait();
                }).ShouldThrow<TimeoutException>("Because you didn't select anything in time");

            });
        }

        [SolidworksFact]
        public void GetConfigurationFromID()
        {
            CreatePartDoc(doc =>
            {
                var config = doc.ConfigurationManager.AddConfiguration("test", "", "", (int)swConfigurationOptions2_e.swConfigOption_DontActivate, "", "");
                var output = doc.GetConfigurationFromID(config.GetID());
                var result = output.Match(
                    Some: v => v.Name == "test",
                    None: () => false
                    );
                result.Should().BeTrue();
            });
        }

        [SolidworksFact]
        public void GetConfigurationFromIDNullTest()
        {
            CreatePartDoc(doc =>
            {
                var config = doc.ConfigurationManager.AddConfiguration("test", "", "", (int)swConfigurationOptions2_e.swConfigOption_DontActivate, "", "");
                var result = doc.GetConfigurationFromID(27);
                result.IsNone.Should().BeTrue();
            });
        }
    }
}
