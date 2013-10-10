using System;
using System.Diagnostics;
using LinFu.AOP.Interfaces;

namespace ken.Spikes.Aspects.LinFu
{
    public class MyInterceptor : IInterceptor
    {
        public object Intercept(IInvocationInfo info)
        {
            var methodName = info.TargetMethod.Name;
            Trace.WriteLine(String.Format("before: {0}", methodName));
            var result = info.TargetMethod.Invoke(info.Target, info.Arguments);
            result = (int)result * 2;
            Trace.WriteLine(String.Format("after: {0}", methodName));
            return result;
        }
    }
}
