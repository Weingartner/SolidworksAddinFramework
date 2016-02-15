using System.Windows.Forms;
using SwCSharpAddinMF.SWAddin;

namespace SwCSharpAddinMF
{
    public class PMPHandler : PmpHandlerBase
    {
        public UserPmPage PmpPage { get; }

        public PMPHandler(UserPmPage addin)
        {
            PmpPage = addin;
        }

        //Implement these methods from the interface
        override public void AfterClose()
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.
            int IndentSize;
            IndentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(IndentSize);
        }

        override public void OnClose(int reason)
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.
            int IndentSize;
            IndentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(IndentSize);
        }
    }
}
