using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using SolidWorksTools;
using SolidWorksTools.File;

namespace SolidworksAddinFramework
{

    public interface ISolidworksAddin
    {
        ISldWorks SwApp { set; get; }
        IModeler Modeler { get; }
        MathUtility Math { get; }
    }


    [ComVisible(true)]
    public abstract class SwAddinBase : ISwAddin, ISolidworksAddin
    {
        public int AddinId { get; private set; }
        protected BitmapHandler Bmp;
        private Hashtable _OpenDocs = new Hashtable();
        private SldWorks _SwEventPtr;

        public ICommandManager CommandManager { get; private set; }

        public Hashtable OpenDocs => _OpenDocs;

        #region ISolidworksAddin
        public ISldWorks SwApp { set; get; }

        public MathUtility Math => (MathUtility) SwApp.GetMathUtility();

        public IModeler Modeler => (IModeler) SwApp.GetModeler();
        #endregion

        public static ISolidworksAddin Active { get; set; }

        [ComRegisterFunction]
        public static void RegisterFunction(Type t)
        {
            #region Get Custom Attribute: SwAddinAttribute
            var sWattr = t
                .GetCustomAttributes(false).OfType<SwAddinAttribute>()
                .FirstOrDefault();

            #endregion

            try
            {
                var hklm = Registry.LocalMachine;
                var hkcu = Registry.CurrentUser;

                var keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID + "}";
                var addinkey = CreateSubKey(hklm, keyname);
                addinkey.SetValue(null, 0);

                addinkey.SetValue("Description", sWattr.Description);
                addinkey.SetValue("Title", sWattr.Title);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID + "}";
                addinkey = CreateSubKey(hkcu,keyname);
                addinkey.SetValue(null, Convert.ToInt32(sWattr.LoadAtStartup), RegistryValueKind.DWord);
                //MessageBox.Show("Registered pluging with GUID.\n\"" + t.GUID + "\"");
            }
            catch (NullReferenceException nl)
            {
                Console.WriteLine("There was a problem registering this dll: SWattr is null. \n\"" + nl.Message + "\"");
                MessageBox.Show("There was a problem registering this dll: SWattr is null.\n\"" + nl.Message + "\"");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                MessageBox.Show("There was a problem registering the function: \n\"" + e.Message + "\"");
            }
        }

        private static RegistryKey CreateSubKey(RegistryKey hklm, string keyname)
        {
            Console.WriteLine($"Registering '{keyname}' to '{hklm.Name}' ");
            return hklm.CreateSubKey(keyname);
        }

        [ComUnregisterFunction]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                var hklm = Registry.LocalMachine;
                var hkcu = Registry.CurrentUser;

                var keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID + "}";
                hklm.DeleteSubKey(keyname);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID + "}";
                hkcu.DeleteSubKey(keyname);
            }
            catch (NullReferenceException nl)
            {
                Console.WriteLine("There was a problem unregistering this dll: " + nl.Message);
                MessageBox.Show("There was a problem unregistering this dll: \n\"" + nl.Message + "\"");
            }
            catch (Exception e)
            {
                Console.WriteLine("There was a problem unregistering this dll: " + e.Message);
                MessageBox.Show("There was a problem unregistering this dll: \n\"" + e.Message + "\"");
            }
        }

        public bool ConnectToSW(object thisSw, int cookie)
        {
            SwApp = (ISldWorks)thisSw;
            AddinId = cookie;

            Active = this;

            //Setup callbacks
            SwApp.SetAddinCallbackInfo2(0, this, AddinId);

            #region Setup the Command Manager
            Connect();
            #endregion

            #region Setup the Event Handlers
            _SwEventPtr = (SldWorks)SwApp;
            _OpenDocs = new Hashtable();
            AttachEventHandlers();
            #endregion


            return true;
        }
        public void Connect()
        {
            Bmp = new BitmapHandler();
            CommandManager = SwApp.GetCommandManager(AddinId);
            AddCommandMgr();
        }

        public void Disconnect()
        {
           RemoveCommandMgr();
           Bmp.Dispose();
        }

        private BitmapHandler BitmapHandler = new BitmapHandler();

        /// <summary>
        /// Get a bitmap resource string from bitmap name located relative
        /// to the assembly the current type is in.
        /// </summary>
        /// <example>
        /// <![CDATA[cmdGroup.LargeIconList = this.GetBitMap("MyProject.ToolbarLarge.bmp")]]>
        /// </example>
        /// <param name="name"></param>
        /// <param name="t">The type to use to look up the assembly the bitmap is relative to. If null
        /// uses the type of 'this'</param>
        /// <returns></returns>
        public string GetBitMap(string name, Type t = null)
        {
            var assembly = Assembly.GetAssembly(t ?? GetType());
            return Bmp.CreateFileFromResourceBitmap(name, assembly);
        }

        /*
        public void AddTab(swDocumentTypes_e docType, string title)
        {
            
                var cmdTab = CommandManager.GetCommandTab(type, title);

                if (cmdTab != null & !getDataResult | ignorePrevious)//if tab exists, but we have ignored the registry info (or changed command group ID), re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
                {
                    CommandManager.RemoveCommandTab(cmdTab);
                    cmdTab = null;
                }

                //if cmdTab is null, must be first load (possibly after reset), add the commands to the tabs
            if (cmdTab == null)
            {
            }
        }
        */

        public abstract void AddCommandMgr();
        public abstract void RemoveCommandMgr();

        public bool DisconnectFromSW()
        {
            Disconnect();
            DetachEventHandlers();

            Marshal.ReleaseComObject(CommandManager);
            CommandManager = null;
            Marshal.ReleaseComObject(SwApp);
            SwApp = null;
            //The addin _must_ call GC.Collect() here in order to retrieve all managed code pointers 
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }

        public bool CompareIDs(int[] storedIDs, int[] addinIDs)
        {
            var storedList = new HashSet<int>(storedIDs);
            var addinList = new HashSet<int>(addinIDs);
            return storedList.SetEquals(addinList);
        }

        public bool AttachEventHandlers()
        {
            AttachSwEvents();
            //Listen for events on all currently open docs
            AttachEventsToAllDocuments();
            return true;
        }


        private bool AttachSwEvents()
        {
            try
            {
                _SwEventPtr.ActiveDocChangeNotify += OnDocChange;
                _SwEventPtr.DocumentLoadNotify2 += OnDocLoad;
                _SwEventPtr.FileNewNotify2 += OnFileNew;
                _SwEventPtr.ActiveModelDocChangeNotify += OnModelChange;
                _SwEventPtr.FileOpenPostNotify += FileOpenPostNotify;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool DetachSwEvents()
        {
            try
            {
                _SwEventPtr.ActiveDocChangeNotify -= OnDocChange;
                _SwEventPtr.DocumentLoadNotify2 -= OnDocLoad;
                _SwEventPtr.FileNewNotify2 -= OnFileNew;
                _SwEventPtr.ActiveModelDocChangeNotify -= OnModelChange;
                _SwEventPtr.FileOpenPostNotify -= FileOpenPostNotify;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        public void AttachEventsToAllDocuments()
        {
            var modDoc = (ModelDoc2)SwApp.GetFirstDocument();
            while (modDoc != null)
            {
                if (!_OpenDocs.Contains(modDoc))
                {
                    AttachModelDocEventHandler(modDoc);
                }
                modDoc = (ModelDoc2)modDoc.GetNext();
            }
        }

        public bool AttachModelDocEventHandler(ModelDoc2 modDoc)
        {
            if (modDoc == null)
                return false;

            DocumentEventHandler docHandler = null;

            if (!_OpenDocs.Contains(modDoc))
            {
                switch (modDoc.GetType())
                {
                    case (int)swDocumentTypes_e.swDocPART:
                    {
                        docHandler = new PartEventHandler(modDoc, this);
                        break;
                    }
                    case (int)swDocumentTypes_e.swDocASSEMBLY:
                    {
                        docHandler = new AssemblyEventHandler(modDoc, this);
                        break;
                    }
                    case (int)swDocumentTypes_e.swDocDRAWING:
                    {
                        docHandler = new DrawingEventHandler(modDoc, this);
                        break;
                    }
                    default:
                    {
                        return false; //Unsupported document type
                    }
                }
                docHandler.AttachEventHandlers();
                _OpenDocs.Add(modDoc, docHandler);
            }
            return true;
        }

        public bool DetachModelEventHandler(ModelDoc2 modDoc)
        {
            DocumentEventHandler docHandler;
            docHandler = (DocumentEventHandler)_OpenDocs[modDoc];
            _OpenDocs.Remove(modDoc);
            modDoc = null;
            docHandler = null;
            return true;
        }

        public bool DetachEventHandlers()
        {
            DetachSwEvents();

            //Close events on all currently open docs
            DocumentEventHandler docHandler;
            var numKeys = _OpenDocs.Count;
            var keys = new object[numKeys];

            //Remove all document event handlers
            _OpenDocs.Keys.CopyTo(keys, 0);
            foreach (ModelDoc2 key in keys)
            {
                docHandler = (DocumentEventHandler)_OpenDocs[key];
                docHandler.DetachEventHandlers(); //This also removes the pair from the hash
                docHandler = null;
            }
            return true;
        }

        protected virtual int OnDocChange()
        {

            return 1;
        }

        protected virtual int OnDocLoad(string docTitle, string docPath)
        {
            return 0;
        }

        protected virtual int FileOpenPostNotify(string fileName)
        {
            AttachEventsToAllDocuments();
            return 0;
        }

        protected virtual int OnFileNew(object newDoc, int docType, string templateName)
        {
            AttachEventsToAllDocuments();
            return 0;
        }

        protected virtual int OnModelChange()
        {
            return 0;
        }
    }
}