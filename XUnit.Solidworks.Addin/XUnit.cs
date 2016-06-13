using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
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

    public static class Common
    {
        public static Guid CollectinId { get; } = Guid.NewGuid();
    }

    public class SolidworksFactDiscoverer : XUnitRemoteFactDiscoverer
    {
        public SolidworksFactDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink, TestSettings.Id, TestSettings.SolidworksPath, t => new ScheduledTestCase(t), Common.CollectinId)
        {
        }
    }

    public class SolidworksTheoryDiscoverer : XUnitRemoteTheoryDiscoverer
    {
        public SolidworksTheoryDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink, TestSettings.Id, TestSettings.SolidworksPath, t => new ScheduledTestCase(t), Common.CollectinId)
        {
        }
    }


    public class ScheduledTestCase : IXunitTestCase
    {
        private readonly IXunitTestCase _TestCase;

        public ScheduledTestCase(IXunitTestCase testCase)
        {
            _TestCase = testCase;
        }

        public Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var tcs = new TaskCompletionSource<RunSummary>();
            SwAddin.Scheduler.ScheduleAsync(async (scheduler, ct) =>
            {
                try
                {
                    var runSummary = await _TestCase.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);
                    tcs.SetResult(runSummary);
                }
                catch (OperationCanceledException)
                {
                    tcs.SetCanceled();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }

        public IMethodInfo Method => _TestCase.Method;

        public void Deserialize(IXunitSerializationInfo info) => _TestCase.Deserialize(info);

        public void Serialize(IXunitSerializationInfo info) => _TestCase.Serialize(info);

        public string DisplayName => _TestCase.DisplayName;

        public string SkipReason => _TestCase.SkipReason;

        public ISourceInformation SourceInformation
        {
            get { return _TestCase.SourceInformation; }
            set { _TestCase.SourceInformation = value; }
        }

        public ITestMethod TestMethod => _TestCase.TestMethod;

        public object[] TestMethodArguments => _TestCase.TestMethodArguments;

        public Dictionary<string, List<string>> Traits => _TestCase.Traits;

        public string UniqueID => _TestCase.UniqueID;
    }
}