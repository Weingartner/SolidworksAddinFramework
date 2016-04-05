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

            //var task = Task.Run(() =>
            //{
                try
                {
                    string path = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                    var dir = Path.GetDirectoryName(path);
                    var domaininfo = AppDomain.CurrentDomain.SetupInformation;
                    domaininfo.ApplicationBase = dir;

                    AppDomain domain = AppDomain.CreateDomain("XUnitDomain", AppDomain.CurrentDomain.Evidence,
                        domaininfo);

                    domain.SetData(SwdataKey, SwApp);
                    domain.DoCallBack(Callback);
                    //MessageBox.Show("Unloading app domain");
                    //AppDomain.Unload(domain);
                    //GC.Collect();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            //}
                //);
        }


        private static void Callback()
        {
            XUnitService.Start(SolidworksFactDiscoverer.XUnitId);
        }



        public override void RemoveCommandMgr()
        {
            CommandManager.RemoveCommandGroup(MainCmdGroupId);
        }

        #endregion

        #region UI Callbacks

        #endregion

    }



}
    