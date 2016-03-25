using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework
{
    public static class SwProxy
    {
        public static readonly ProxyGenerator Generator = new ProxyGenerator();
        public static readonly ProxyGenerationOptions GenerationOptions = new ProxyGenerationOptions(new ProxyGenerationHook()) { Selector = new InterceptorSelector() };

    public static ISldWorks Proxy(this ISldWorks app)
        {
            return (ISldWorks)Generator.CreateInterfaceProxyWithTargetInterface(typeof(SldWorks), app, GenerationOptions, new SwProxyInterceptor());
        }
    }

    public class SwInterceptor<T> : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            invocation.ReturnValue = SwProxy.Generator.CreateInterfaceProxyWithTargetInterface(typeof(T), invocation.ReturnValue, SwProxy.GenerationOptions, new SwProxyInterceptor());
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

    public class ProxyGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            var fullName = $"{type.FullName}::{methodInfo.Name}";
            return fullName != $"{typeof (ISldWorks).FullName}::{nameof(ISldWorks.GetDocuments)}";
        }
    }

    public class InterceptorSelector : IInterceptorSelector
    {
        private readonly IDictionary<string, IInterceptor> _MethodToInterceptor = new Dictionary<string, IInterceptor>
        {
            { $"{typeof (ISldWorks).FullName}::{nameof(ISldWorks.GetMathUtility)}", new SwInterceptor<IMathUtility>() }
        };

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var fullName = $"{method.DeclaringType?.FullName}::{method.Name}";
            IInterceptor interceptor;
            if (_MethodToInterceptor.TryGetValue(fullName, out interceptor))
            {
                return interceptors.Concat(new[] { interceptor }).ToArray();
            }
            return interceptors;
        }
    }

    public class SwException : Exception
    {
        public SwException(string message): base(message)
        {
        }
    }
}
