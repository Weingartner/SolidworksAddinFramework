using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices;
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
using System.Diagnostics;
using SolidworksAddinFramework;

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

        [SolidworksFact]
        public void DoWithOpenDocShouldCleanup()
        {
            var opened = 0;
            var closed = 0;
            using (SwApp.DoWithOpenDoc(doc =>
            {
                opened++;
                return Disposable.Create(() => closed++);
            }))
            {
                opened.Should().Be(0);
                closed.Should().Be(0);

                var doc = CreatePartDoc();

                opened.Should().Be(1);
                closed.Should().Be(0);

                var doc2 = CreatePartDoc();

                opened.Should().Be(2);
                closed.Should().Be(0);

                SwApp.CloseDoc(doc2.GetTitle());

                opened.Should().Be(2);
                closed.Should().Be(1);

                SwApp.CloseDoc(doc.GetTitle());

                opened.Should().Be(2);
                closed.Should().Be(2);
            }
        }

        private static void DisconnectEntity(IModelDoc2 doc, Func<object> getEntity)
        {
            // switch to new config
            var defaultConfig = doc.ConfigurationManager.ActiveConfiguration;
            var newConfig = doc.ConfigurationManager.AddConfiguration("TestConfig", "", "", 0, "Default", "");
            doc.ConfigurationManager.ActiveConfiguration.Should().Be(newConfig);

            // delete body
            doc.AddSelections(0, new[] { getEntity() });
            var deleteFeature = doc.FeatureManager.InsertDeleteBody2(false);
            doc.GetBodiesTs().Should().BeEmpty();

            // switch back to default config
            doc.ShowConfiguration2(defaultConfig.Name).Should().BeTrue();
            doc.ConfigurationManager.ActiveConfiguration.Should().Be(defaultConfig);
        }

        [SolidworksFact]
        public void EntityReferencesMightBeDisconnected()
        {
            CreatePartDoc(false, doc =>
            {
                var feature = SpecHelper.InsertDummyBody(doc);
                var body = doc.GetBodiesTs().Single();

                DisconnectEntity(doc, () => body);

                new Action(() => body.GetBodyBoxTs()).ShouldThrow<COMException>().Which.Message.Should().Contain("disconnected");

                return Disposable.Empty;
            });
        }

        [SolidworksFact]
        public void PersistentEntityReferencesShouldNotBeDisconnected()
        {
            CreatePartDoc(false, doc =>
            {
                var feature = SpecHelper.InsertDummyBody(doc);
                var body = doc.GetPersistentEntityReference(doc.GetBodiesTs().Single());

                DisconnectEntity(doc, body);

                new Action(() => body().GetBodyBoxTs()).ShouldNotThrow<COMException>();

                return Disposable.Empty;
            });
        }
    }
}
