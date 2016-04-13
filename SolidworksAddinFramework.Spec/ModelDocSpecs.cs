using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using XUnit;
using XUnit.Solidworks.Addin;

namespace SolidworksAddinFramework.Spec
{
    public class ModelDocSpecs : SolidWorksSpec
    {
        /// <summary>
        /// Interactive test example. 
        /// </summary>
        /// <returns></returns>
        [SolidworksFact]
        public async Task CanSelect()
        {
            await CreatePartDoc(async doc =>
            {
                var modeller = (IModeler) SwApp.GetModeler();
                var box = modeller.CreateBox(0.1, 0.1, 0.1);
                var part = (PartDoc) doc;
                part.CreateFeatureFromBody3(box, false, 0);

                var o = await doc.SelectionObservable((selectTypeE, mark) => true).FirstAsync().Timeout(TimeSpan.FromSeconds(5));

                o.Should().NotBeNull();
            });
        }
    }
}
