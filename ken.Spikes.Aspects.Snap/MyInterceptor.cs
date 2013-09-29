using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Snap;

namespace ken.Spikes.Aspects.Snap
{
    public class MyInterceptor : MethodInterceptor
    {
        public override void BeforeInvocation()
        {
            Console.WriteLine("this is executed before your method");
            base.BeforeInvocation();
        }

        public override void InterceptMethod(IInvocation invocation, MethodBase method, Attribute attribute)
        {
            invocation.Proceed(); // the underlying method call
            invocation.ReturnValue = (int)invocation.ReturnValue * 2;
        }

        public override void AfterInvocation()
        {
            Console.WriteLine("this is executed after your method");
            base.AfterInvocation();
        }
    }
}
