using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ken.Spikes.Aspects.Snap
{
    public class MyClassWithInterceptorAttribute : IMyInterface
    {
        [MyInterceptorAttribute] 
        public int Calc(int x, int y)
        {
            return x - y;
        }
    }
}
