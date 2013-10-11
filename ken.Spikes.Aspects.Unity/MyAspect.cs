using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace ken.Spikes.Aspects.Unity
{
    public class MyAspect : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            Console.WriteLine("Before: {0}", input.MethodBase.Name);
            var returnMethod = getNext().Invoke(input, getNext);
            returnMethod.ReturnValue = (int)returnMethod.ReturnValue*2; 
            Console.WriteLine("After: {0}", input.MethodBase.Name);
            return returnMethod;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute { get { return true; } }
    }
}
