using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ken.Spikes.Aspects.Emit
{
    /// <summary>
    /// It’s important to notice that I added some Boolean return values where you wouldn’t expect them. 
    /// These are for adding ‘retry’ support, which is useful for things like exception filtering and failovers. 
    /// Also notice the context object that I pass along. 
    /// This object is for anything you would like to have during scope of the method invocation.
    /// 
    /// We also want to be able to change the parameters and return types in our decorators. 
    /// In other words, you are allowed to change ‘pars’ during method invocation. 
    /// This is especially useful in the ‘Before’ phase.
    /// </summary>
    public class MyDecorator
    {
        [DecoratorAttribute(DecoratorUsage.Before)]
        public void CallBefore(string methodName, object[] pars, ref object context)
        {
            Console.WriteLine("Before calling {0}", methodName);
        }

        //[DecoratorAttribute(DecoratorUsage.Success)]
        [DecoratorAttribute(DecoratorUsage.Success, "Calc")]
        public bool CallSuccess(string methodName, object[] pars, ref object result, ref object context)
        {
            Console.WriteLine("Success calling {0}", methodName);

            result = (int)result * 2;

            return false; //retry
        }

        [DecoratorAttribute(DecoratorUsage.OnException)]
        public bool CallException(string methodName, object[] pars, Exception exception, ref object context)
        {
            Console.WriteLine("Exception calling {0}", methodName);
            return false;
        }

        [DecoratorAttribute(DecoratorUsage.After)]
        public void CallAfter(string methodName, object[] pars, ref object context)
        {
            Console.WriteLine("After calling {0}", methodName);
        }
    }
}
