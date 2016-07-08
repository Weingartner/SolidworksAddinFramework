using System;
using System.Reactive.Disposables;
using DemoMacroFeatures;
using FluentAssertions;
using XUnit.Solidworks.Addin;

namespace SolidworksAddinFramework.Spec
{
    public class MacroFeatureDataSpec : SolidWorksSpec
    {
        [SolidworksFact]
        public void ShouldBeAbleToStore2MegabytesOfData()
        {
            CreatePartDoc(doc =>
            {
                var feature = new DummyMacroFeature();
                feature.Insert(SwApp, doc, new DummyMacroFeatureData {Data = RandomString(1024 * 1024 * 50)});
                feature.SwFeature.Should().NotBeNull();

                var feature2 = new DummyMacroFeature();
                feature2.Edit(SwApp, doc, feature.SwFeature);
                feature2.Database.Data.Should().Be(feature.Database.Data);
                return Disposable.Empty;
            });
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
