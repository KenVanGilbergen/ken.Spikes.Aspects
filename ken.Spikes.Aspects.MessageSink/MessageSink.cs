using System;
using System.Runtime.Remoting.Messaging;

namespace ken.Spikes.Aspects.MessageSink
{
    internal class MessageSink : IMessageSink
    {
        internal MessageSink(IMessageSink next) { m_next = next; }

        private IMessageSink m_next;
        private String _typeAndName;

        #region IMessageSink implementation
        public IMessageSink NextSink
        {
            get { return m_next; }
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            if (msg is IMethodMessage)
            {
                var call = msg as IMethodMessage;
                var type = Type.GetType(call.TypeName);
                if (type != null)
                {
                    //var key = call.TypeName + "." + call.MethodName;
                    //var cached = CachingConfiguration.SystemRuntimeCachingProvider.Get<object>(key);
                    //if (null == cached)
                    //{
                    IMessage returnMethod = m_next.SyncProcessMessage(msg);
                    if (!(returnMethod is IMethodReturnMessage)) return returnMethod;
                        
                    var retMsg = (IMethodReturnMessage)returnMethod;
                    System.Exception e = retMsg.Exception;
                    if (null == e)
                    {
                        if (retMsg.ReturnValue.GetType() != typeof (void))
                        {
                            var methodMessage = (IMethodCallMessage)msg;
                            var overrideReturnMethod = new ReturnMessage(
                                (int)retMsg.ReturnValue * 2, 
                                methodMessage.Args, 
                                methodMessage.ArgCount, 
                                methodMessage.LogicalCallContext, methodMessage);
                            return overrideReturnMethod;
                        }
                    }
                    return returnMethod;
                    
                    //}
                    //var methodMessage = (IMethodCallMessage)msg;
                    //var overrideReturnMethod = new ReturnMessage(cached, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
                    //return overrideReturnMethod;
                }
            }
            
            return m_next.SyncProcessMessage(msg);
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            throw new InvalidOperationException();
        }
        #endregion //IMessageSink implementation

        #region Helper methods
        private void Preprocess(IMessage msg)
        {
            // We only want to process method calls
            if (!(msg is IMethodMessage)) return;

            var call = msg as IMethodMessage;
            var type = Type.GetType(call.TypeName);
            if (type != null) _typeAndName = type.Name + "." + call.MethodName;
            Console.Write("PreProcessing: " + _typeAndName + "(");

            // Loop through the [in] parameters
            for (int i = 0; i < call.ArgCount; ++i)
            {
                if (i > 0) Console.Write(", ");
                Console.Write(call.GetArgName(i) + " = " + call.GetArg(i));
            }
            Console.WriteLine(")");
        }

        private void PostProcess(IMessage msg, IMessage msgReturn)
        {
            // We only want to process method return calls
            if (!(msg is IMethodMessage) ||
                !(msgReturn is IMethodReturnMessage)) return;

            var retMsg = (IMethodReturnMessage)msgReturn;
            Console.Write("PostProcessing: ");
            System.Exception e = retMsg.Exception;
            if (e != null)
            {
                Console.WriteLine("Exception was thrown: " + e);
                return;
            }

            // Loop through all the [out] parameters
            Console.Write(_typeAndName + "(");
            if (retMsg.OutArgCount > 0)
            {
                Console.Write("out parameters[");
                for (int i = 0; i < retMsg.OutArgCount; ++i)
                {
                    if (i > 0) Console.Write(", ");
                    Console.Write(retMsg.GetOutArgName(i) + " = " +
                                  retMsg.GetOutArg(i));
                }
                Console.Write("]");
            }
            if (retMsg.ReturnValue.GetType() != typeof(void))
                Console.Write(" returned [" + retMsg.ReturnValue + "]");

            Console.WriteLine(")\n");
        }
        #endregion Helpers
    }
}