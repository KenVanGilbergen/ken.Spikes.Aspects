using System;
using AopAlliance.Intercept;

namespace ken.Spikes.Aspects.Spring
{
    public class MyAspect : IMethodInterceptor
    {
        public object Invoke(IMethodInvocation invocation)
        {
            Console.WriteLine("Before: {0}", invocation.Method.Name);
            var returnValue = (int)invocation.Proceed() * 2;
            Console.WriteLine("After: {0}", invocation.Method.Name);
            return returnValue;
        }
    }
}
