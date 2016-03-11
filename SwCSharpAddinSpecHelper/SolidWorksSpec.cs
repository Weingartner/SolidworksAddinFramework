using System;
using SolidWorks.Interop.sldworks;
using Xunit;

namespace SwCSharpAddinSpecHelper
{
    [Collection("SwPool")]
    public abstract class SolidWorksSpec : IDisposable
    {
        private readonly SwPoolFixture _Pool;
        protected SldWorks App;

        protected SolidWorksSpec(SwPoolFixture pool)
        {
            _Pool = pool;
            App = _Pool.Acquire();
        }

        public void Dispose()
        {
            _Pool.Release(App);
        }
    }

    public class SwPoolFixture : IDisposable
    {
        private readonly Pool<SldWorks> _Instances = new Pool<SldWorks>(() => new SldWorks { Visible = false });

        public SldWorks Acquire()
        {
            return _Instances.Acquire();
        }

        public void Release(SldWorks instance)
        {
            // TODO check if instance is healthy?
            _Instances.Release(instance);
        }

        public void Dispose()
        {
            _Instances.ForEach(i => i.ExitApp());
        }
    }
}
