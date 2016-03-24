using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class SwProxy
    {
        public static ISldWorks Proxy(this ISldWorks app)
        {
            var generator = new ProxyGenerator();
            return (ISldWorks)generator.CreateInterfaceProxyWithTargetInterface(typeof(ISldWorks), new[] { typeof(SldWorks) }, app, ProxyGenerationOptions.Default, new SwProxyInterceptor());
        }
    }

    public class SwProxyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.Method.ReturnType.FullName == "System.Object" && invocation.ReturnValue == null)
            {
                throw new SwException($"{invocation.Method.DeclaringType?.FullName}::{invocation.Method.Name} returned <null> which might indicate an error.");
            }
        }
    }

    public class SwException : Exception
    {
        public SwException(string message): base(message)
        {
        }
    }
}
