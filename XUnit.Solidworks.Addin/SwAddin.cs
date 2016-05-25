// Keep this commented out. I have trouble loading DLL's when I 
// create isolated app domains. It requires more work
//#define UseXUnitAppDomain

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SolidworksAddinFramework;
using SolidWorks.Interop.sldworks;
using SolidWorksTools;
using XUnitRemote.Local;


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
        #region UI Methods

        protected override IEnumerable<IDisposable> Setup()
        {
            try
            {
#if UseXUnitAppDomain
                var swDataKey = "SwData";
                var domain = CloneDomain("XUnitDomain");
                domain.SetData(swDataKey, SwApp);
                domain.DoCallBack(() => Callback((ISldWorks)AppDomain.CurrentDomain.GetData(swDataKey)));
                return new[] { Disposable.Create(() => AppDomain.Unload(domain)) };
#else
                Callback(SwApp);
#endif
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return new IDisposable[0];
        }


        static Assembly LoadFromSolidworks(object sender, ResolveEventArgs args, string baseDirectory)
        {
            baseDirectory = @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS";
            string assemblyPath = System.IO.Path.Combine(baseDirectory, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            var assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private static AppDomain CloneDomain(string name)
        {
            string path = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            var dir = System.IO.Path.GetDirectoryName(path);
            var domaininfo = AppDomain.CurrentDomain.SetupInformation;
            domaininfo.ApplicationBase = dir;
            var domain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, domaininfo);

            domain.AssemblyResolve += (o, args) => LoadFromSolidworks(o, args, AppDomain.CurrentDomain.BaseDirectory);


            return domain;
        }

        private static void Callback(ISldWorks sldWorks )
        {
            var marshaller = new ThreadMarshaller();
            var data = new Dictionary<string, object> { {nameof(ISldWorks), sldWorks}};
            XUnitService.Start(TestSettings.Id,false, f => marshaller.Marshall(f), data);
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
    