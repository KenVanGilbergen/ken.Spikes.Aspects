using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace ken.Spikes.Aspects.MessageSink
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageSinkAttribute : ContextAttribute
    {
        public MessageSinkAttribute() : base("MessageSink")
        {
        }

        public override void GetPropertiesForNewContext(IConstructionCallMessage ccm)
        {
            ccm.ContextProperties.Add(new MessageSinkProperty());
        }
    }
}