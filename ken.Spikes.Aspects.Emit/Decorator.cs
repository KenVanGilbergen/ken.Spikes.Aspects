using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ken.Spikes.Aspects.Emit
{
    /// <summary>
    /// Now all that remains is mapping the decorators on types. 
    /// This isn’t too hard… simply cache the proxy’s that are generated and return a new proxy after generation.
    /// </summary>
    public class Decorator
    {
        private Decorator() { }

        private class DecoratorPair
        {
            public Type DecoratorType;
            public Type InterfaceType;

            public override bool Equals(object obj)
            {
                var pair = (DecoratorPair)obj;
                return pair.DecoratorType.Equals(DecoratorType) && pair.InterfaceType.Equals(InterfaceType);
            }

            public override int GetHashCode()
            {
                return DecoratorType.GetHashCode() * 7577 + InterfaceType.GetHashCode();
            }
        }

        private static Dictionary<DecoratorPair, Type> decorators = new Dictionary<DecoratorPair, Type>();
        private static object decoratorObject = new object();

        public static T CreateDecorator<T, U>(T target, U decorator)
        {
            var pair = new DecoratorPair()
            {
                DecoratorType = typeof(U),
                InterfaceType = typeof(T)
            };
            Type t;
            lock (decoratorObject)
            {
                if (!decorators.TryGetValue(pair, out t))
                {
                    var generator = new DecoratorProxyGenerator();
                    t = generator.CreateDecorator(typeof(T), typeof(U));
                    decorators.Add(pair, t);
                }
            }

            return (T)Activator.CreateInstance(t, target, decorator);
        }
    }	
}
