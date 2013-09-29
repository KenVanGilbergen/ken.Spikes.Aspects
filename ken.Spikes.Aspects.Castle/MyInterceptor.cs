using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace ken.Spikes.Aspects.Castle
{
    public class MyInterceptor : IInterceptor
    {
        public void Before(IInvocation invocation)
        {
            Trace.WriteLine(String.Format("before: {0}",invocation.Method));
        }

        public void After(IInvocation invocation)
        {
            Trace.WriteLine(String.Format("after: {0}", invocation.Method));
        }

        public void Intercept(IInvocation invocation)
        {
            Before(invocation);
            invocation.Proceed();
            invocation.ReturnValue = (int)invocation.ReturnValue*2;
            After(invocation);
        }
    }
}
