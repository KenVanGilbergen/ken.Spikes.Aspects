using System;

namespace ken.Spikes.Aspects.Emit
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class DecoratorAttribute : Attribute
    {
        public DecoratorAttribute(DecoratorUsage usage)
        {
            this.Usage = usage;
            this.MethodName = null;
        }

        public DecoratorAttribute(DecoratorUsage usage, string methodName)
        {
            this.Usage = usage;
            this.MethodName = methodName;
        }

        public DecoratorUsage Usage { get; private set; }
        public string MethodName { get; private set; }
    }
}
