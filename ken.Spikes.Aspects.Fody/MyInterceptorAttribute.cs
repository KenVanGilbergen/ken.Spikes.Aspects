using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ken.Spikes.Aspects.Fody
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class MyInterceptorAttribute : Attribute, IMethodDecorator
    {
        public void OnEntry(MethodBase method)
        {
            Console.WriteLine("OnEntry");
        }

        public void OnExit(MethodBase method)
        {
            Console.WriteLine("OnExit");

            var mi = (MethodInfo) method;
            var p = method.GetParameters();
            var b = method.GetMethodBody();
            var st = new StackTrace();
            StackFrame sf = st.GetFrame(1); //previous method call
	    }

        public void OnException(MethodBase method, Exception exception)
        {
            Console.WriteLine("OnException");
        }
    }
}
