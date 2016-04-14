using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Xunit;
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
        public Task CanSelect()
        {
            return CreatePartDoc(async doc =>
            {
                var modeller = (IModeler) SwApp.GetModeler();
                var box = modeller.CreateBox(0.1, 0.1, 0.1);
                var part = (PartDoc) doc;
                part.CreateFeatureFromBody3(box, false, 0);

                //var o = await doc.SelectionObservable((selectTypeE, mark) => true).FirstAsync();


                await doc.SelectionObservable((selectTypeE, mark) => true).Do(s=>MessageBox.Show("X")).FirstAsync();
                await Task.Delay(TimeSpan.FromDays(1));
            });
        }

        [Fact]
        public void MethodGroupConversionAreCached()
        {
                Func<int> fn = () => 10;

            var a = ((Func<int>) fn.Invoke);
            var b = (Func<int>) fn.Invoke;

            // This is expected
            Assert.Equal( true, object.ReferenceEquals(a,a));

            // This is not expected ( or I'm not convinced it will always be so )
            Assert.Equal(true, a == b);
            Assert.Equal(true, object.ReferenceEquals(a,b));
            
        }
    }
}
