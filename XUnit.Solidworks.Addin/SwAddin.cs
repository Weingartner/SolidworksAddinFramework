//#define UseXUnitAppDomain

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using XUnitRemote;
using XUnitRemote.Remoting.Service;


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
        public static string SwdataKey { get; } = "SwData";

        #region Local Variables

        public const int MainCmdGroupId = 5;
        public const int MainItemId1 = 0;
        public const int MainItemId2 = 1;
        public const int MainItemId3 = 2;

        #endregion

        #region UI Methods

        public override void AddCommandMgr()
        {

            int[] docTypes = {(int) swDocumentTypes_e.swDocPART};

            var cmdGroupErr = 0;
            var ignorePrevious = false;

            object registryIDs;
            //get the ID information stored in the registry
            var getDataResult = CommandManager.GetGroupDataFromRegistry(MainCmdGroupId, out registryIDs);

            int[] knownIDs = {MainItemId1, MainItemId2};

            if (getDataResult)
            {
                if (!CompareIDs((int[]) registryIDs, knownIDs)) //if the IDs don't match, reset the commandGroup
                {
                    ignorePrevious = true;
                }
            }

            //    SpinWait.SpinUntil(() => { return RemoteDebugger.IsDebuggerAttached(); });

            try
            {


#if UseXUnitAppDomain 
                string path = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                var dir = Path.GetDirectoryName(path);
                var domaininfo = AppDomain.CurrentDomain.SetupInformation;
                domaininfo.ApplicationBase = dir;
                AppDomain domain = AppDomain.CreateDomain("XUnitDomain", AppDomain.CurrentDomain.Evidence,
                    domaininfo);
                domain.SetData(SwdataKey, SwApp);
                    domain.DoCallBack(Callback);
#else
                MessageBox.Show("Starting SW");
                AppDomain.CurrentDomain.SetData(SwdataKey, SwApp);
                Callback(SwApp);
#endif
                //MessageBox.Show("Unloading app domain");
                //AppDomain.Unload(domain);
                //GC.Collect();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        private static void Callback(ISldWorks sldWorks = null)
        {
            SolidWorksSpec.SwApp = sldWorks;
            var marshaller = new ThreadMarshaller();
            XUnitService.Start(SolidworksFactDiscoverer.XUnitId, f =>
            {
                return marshaller.Marshall(f);
            });
        }



        public override void RemoveCommandMgr()
        {
            CommandManager.RemoveCommandGroup(MainCmdGroupId);
        }

#endregion

#region UI Callbacks

#endregion

    }


    public  class ThreadMarshaller : Control
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
            if (!InvokeRequired) return (T) method.DynamicInvoke();
            try
            {
                CallMarshalled = true;
                return (T) Invoke(method);
            }
            finally
            {
                CallMarshalled = false;
            }
        }
    }



}
    