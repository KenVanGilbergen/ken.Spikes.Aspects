using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ken.Spikes.Aspects.Emit
{
    /// <summary>
    /// As for the proxy, we want our code to generate something like this.
    /// </summary>
    public class DecoratorExampleProxy : IMyInterface
    {
        public DecoratorExampleProxy(IMyInterface target, MyDecorator decorator)
        {
            this.target = target;
            this.decorator = decorator;
        } 
 
        private IMyInterface target;
        private MyDecorator decorator; 
 
        public int Calc(int x, int y)
        {
            object context = null;
            object[] par = new object[] { x, y }; 
 
        retry:
            decorator.CallBefore("Calc", par, ref context);
            try
						
            {
                //int obj = target.Calc((int)par[0], (int)par[1]); //adjusted
                object obj = target.Calc((int)par[0], (int)par[1]);
                //if (decorator.CallSuccess("Calc", par, obj, ref context)) //adjusted

                if (decorator.CallSuccess("Calc", par, ref obj, ref context))
                {
                    goto retry;
                }

                //return obj; //adjusted
                return (int)obj;
            }
            catch (Exception ex)
            {
                if (decorator.CallException("Calc", par, ex, ref context))
                {
                    goto retry;
                }
                throw;
            }
            finally
						
            {
                decorator.CallAfter("Calc", par, ref context);
            }
        }
    }
}
