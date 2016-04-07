using System;
using Xunit.Abstractions;
using Xunit.Sdk;
using XUnitRemote.Local;
using FactAttribute = Xunit.FactAttribute;

namespace XUnit.Solidworks.Addin
{

    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("XUnit.Solidworks.Addin.SolidworksFactDiscoverer", "XUnit.Solidworks.Addin")]
    public class SolidworksFactAttribute : FactAttribute { }

    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("XUnit.Solidworks.Addin.SolidworksTheoryDiscoverer", "XUnit.Solidworks.Addin")]
    public class SolidworksTheoryAttribute : FactAttribute { }

    internal static class TestSettings
    {
        public const string Id = "SolidWorksAddin";
        public const string SolidworksPath = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\sldworks.exe";
    }

    public class SolidworksFactDiscoverer : XUnitRemoteFactDiscoverer
    {
        public SolidworksFactDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink, TestSettings.Id, TestSettings.SolidworksPath)
        {
        }
    }

    public class SolidworksTheoryDiscoverer : XUnitRemoteTheoryDiscoverer
    {
        public SolidworksTheoryDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink, TestSettings.Id, TestSettings.SolidworksPath)
        {
        }
    }
}