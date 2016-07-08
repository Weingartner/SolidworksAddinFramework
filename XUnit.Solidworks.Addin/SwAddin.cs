using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
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
            yield return AddCommands();
            yield return StartTestService();
        }

        private IDisposable AddCommands()
        {
            const int mainCmdGroupId = 5;
            const int mainItemId1 = 0;
            const int mainItemId2 = 1;

            const string title = "Test execution";
            const string toolTip = "Test execution";


            int[] docTypes = { (int)swDocumentTypes_e.swDocPART };


            var cmdGroupErr = 0;
            var ignorePrevious = false;

            object registryIDs;
            //get the ID information stored in the registry
            var getDataResult = CommandManager.GetGroupDataFromRegistry(mainCmdGroupId, out registryIDs);

            int[] knownIDs = { mainItemId1, mainItemId2 };

            if (getDataResult)
            {
                if (!CompareIDs((int[])registryIDs, knownIDs)) //if the IDs don't match, reset the commandGroup
                {
                    ignorePrevious = true;
                }
            }

            var cmdGroup = CommandManager.CreateCommandGroup2(mainCmdGroupId, title, toolTip, "", -1, ignorePrevious,
                ref cmdGroupErr);
            cmdGroup.LargeIconList = GetBitMap("SwCSharpAddinMF.Icons.ToolbarLarge.bmp");
            cmdGroup.SmallIconList = GetBitMap("SwCSharpAddinMF.Icons.ToolbarSmall.bmp");
            cmdGroup.LargeMainIcon = GetBitMap("SwCSharpAddinMF.Icons.MainIconLarge.bmp");
            cmdGroup.SmallMainIcon = GetBitMap("SwCSharpAddinMF.Icons.MainIconSmall.bmp");

            var menuToolbarOption = (int)swCommandItemType_e.swToolbarItem | (int)swCommandItemType_e.swMenuItem;
            var cmdIndex0 = cmdGroup.AddCommandItem2(nameof(ContinueTestExecution), -1,
                "Continue execution of a currently paused test", "Continue test execution", 0, nameof(ContinueTestExecution),
                nameof(CanContinueTestExecution), mainItemId1, menuToolbarOption);

            cmdGroup.HasToolbar = true;
            cmdGroup.HasMenu = true;
            cmdGroup.Activate();


            foreach (var type in docTypes)
            {
                var cmdTab = CommandManager.GetCommandTab(type, title);

                // if tab exists, but we have ignored the registry info (or changed command group ID), re-create the tab.
                // Otherwise the ids won't matchup and the tab will be blank
                if (cmdTab != null & !getDataResult | ignorePrevious)
                {
                    CommandManager.RemoveCommandTab(cmdTab);
                    cmdTab = null;
                }

                //if cmdTab is null, must be first load (possibly after reset), add the commands to the tabs
                if (cmdTab != null) continue;

                cmdTab = CommandManager.AddCommandTab(type, title);

                var cmdBox = cmdTab.AddCommandTabBox();

                var cmdIDs = new[] { cmdIndex0 }
                    .Select(id => cmdGroup.CommandID[id])
                    .ToArray();

                var textType = cmdIDs
                    .Select(id => (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal)
                    .ToArray();

                cmdBox.AddCommands(cmdIDs, textType);
            }

            return Disposable.Create(() => CommandManager.RemoveCommandGroup(mainCmdGroupId));
        }

        public void ContinueTestExecution()
        {
            SolidWorksSpec.ContinueTestExecution();
        }

        public int CanContinueTestExecution()
        {
            const int deselectAndDisable = 0;
            const int deselectsAndEnable = 1;
            return SolidWorksSpec.CanContinueTestExecution ? deselectsAndEnable : deselectAndDisable;
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
    