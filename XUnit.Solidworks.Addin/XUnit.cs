using System;
using System.Net;
using System.Windows.Forms;
using Xunit.Abstractions;
using Xunit.Sdk;
using XUnit.Solidworks.Addin;
using XUnitRemote;
using FactAttribute = Xunit.FactAttribute;

namespace XUnit.Solidworks.Addin
{

    /// <summary>
    /// This is the custom attribute you add to tests you wish to run under the control of SampleProcess.
    /// For example your unit test will look like
    /// <![CDATA[
    /// [SampleProcessFact]
    /// public void TestShouldWork(){
    ///    Assert.Equal("SampleProcess",Process.GetCurrentProcess().ProcessName)
    ///    Assert.IsTrue(1==2);
    /// }
    /// ]]>
    /// and it will be executed within "SampleProcess" not the visual studio process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("XUnit.Solidworks.Addin.SolidworksFactDiscoverer", "XUnit.Solidworks.Addin")]
    public class SolidworksFactAttribute : FactAttribute { }

    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("XUnit.Solidworks.Addin.SolidworksTheoryDiscoverer", "XUnit.Solidworks.Addin")]
    public class SolidworksTheoryAttribute : FactAttribute { }


    /// <summary>
    /// This is the xunit fact discoverer that xunit uses to replace the standard xunit runner
    /// with our runner. Anything that is tagged with the above attribute will use this discoverer. 
    /// </summary>
    public class SolidworksFactDiscoverer : XUnitRemoteFactDiscovererBase
    {
        protected override string Id { get; } = XUnitId;

        /// <summary>
        /// You can set this to specify the location of solidworks exe. Should be version 2016+
        /// </summary>
        public static string SolidworksPath = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\sldworks.exe";

        protected override string ExePath { get; } = SolidworksPath;

        public SolidworksFactDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public static string XUnitId = "SolidWorksAddin";
    }

    public class SolidworksTheoryDiscoverer : XUnitRemoteTheoryDiscovererBase
    {
        protected override string Id { get; } = XUnitId;

        /// <summary>
        /// You can set this to specify the location of solidworks exe. Should be version 2016+
        /// </summary>
        public static string SolidworksPath = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\sldworks.exe";

        protected override string ExePath { get; } = SolidworksPath;

        public SolidworksTheoryDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        public static string XUnitId = "SolidWorksAddin";
    }
}