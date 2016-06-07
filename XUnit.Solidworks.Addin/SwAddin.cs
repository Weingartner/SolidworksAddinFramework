using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorksTools;
using XUnitRemote.Remote;
using XUnitRemote.Remote.Service.TestService;
using MessageBox = System.Windows.MessageBox;


namespace XUnit.Solidworks.Addin
{
    /// <summary>
    /// Summary description for XUnit.Solidworks.Addin.
    /// </summary>
    [Guid("7AAE59CE-53BD-4550-B231-5FF4BAE5E7E9"), ComVisible(true)]
    [SwAddin(
        Description = "XUnit.Solidworks.Addin description",
        Title = "XUnit.Solidworks.Addin",
        LoadAtStartup = true
        )]
    public class SwAddin : SwAddinBase
    {
        public static IScheduler Scheduler { get; set; }

        protected override IEnumerable<IDisposable> Setup()
        {
            yield return StartTestService();
        }

        private IDisposable StartTestService()
        {
            try
            {
                var data = new Dictionary<string, object> { { nameof(ISldWorks), SwApp } };
                return XUnitService.StartWithCustomRunner<ScheduledRunner>(new TestServiceConfiguration(TestSettings.Id, data));
            }
            catch (TestServiceException e)
            {
                MessageBox.Show(e.ToString());
                return Disposable.Empty;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return Disposable.Empty;
            }
        }
    }

    internal class ScheduledRunner : ITestRunner
    {
        private readonly ThreadMarshaller _Marshaller = new ThreadMarshaller();
        private readonly ITestRunner _Runner;

        public ScheduledRunner(ITestRunner runner)
        {
            _Runner = runner;
        }

        public Task RunTest(string assemblyPath, string typeName, string methodName)
        {
            return _Marshaller.Marshall(() =>
            {
                SwAddin.Scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
                return _Runner.RunTest(assemblyPath, typeName, methodName);
            });
        }
    }

    public class ThreadMarshaller : Control
    {
        private bool _MbCallMarshalled;

        public ThreadMarshaller()
        {
            _MbCallMarshalled = false;

            // Force the Windows HWND to be created, so this Control becomes associated with this thread:
            // - if this step is omitted Control::InvokeRequired will always return false,
            //   as Window creation is done lazily.
            Debug.Assert(IsHandleCreated == false);
            // ReSharper disable once UnusedVariable
            var pHandle = Handle;
            Debug.Assert(IsHandleCreated == true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // We never want to show this control, so leave empty.

            base.OnPaint(pe);
        }

        public bool CallMarshalled
        {
            get
            {
                lock (this)
                {
                    return _MbCallMarshalled;
                }
            }

            set
            {
                lock (this)
                {
                    _MbCallMarshalled = value;
                }
            }
        }

        public void Marshall(Action method)
        {
            Marshall(() =>
            {
                method();
                return 0;
            });
        }

        public T Marshall<T>(Func<T> method)
        {
            Debug.Assert(IsHandleCreated);

            if (method == null) return default(T);
            if (!InvokeRequired) return (T)method.DynamicInvoke();
            try
            {
                CallMarshalled = true;
                return (T)Invoke(method);
            }
            finally
            {
                CallMarshalled = false;
            }
        }
    }
}
    