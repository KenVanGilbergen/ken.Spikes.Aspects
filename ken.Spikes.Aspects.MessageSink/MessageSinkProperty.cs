using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace ken.Spikes.Aspects.MessageSink
{
    public class MessageSinkProperty : IContextProperty, IContributeObjectSink
    {
        #region IContributeObjectSink implementation
        public IMessageSink GetObjectSink(MarshalByRefObject o, IMessageSink next)
        {
            return new MessageSink(next);
        }
        #endregion // IContributeObjectSink implementation

        #region IContextProperty implementation
        public string Name
        {
            get
            {
                return "CallMessageSinkProperty";
            }
        }
        public void Freeze(Context newContext)
        {
        }
        public bool IsNewContextOK(Context newCtx)
        {
            return true;
        }
        #endregion //IContextProperty implementation
    }
}