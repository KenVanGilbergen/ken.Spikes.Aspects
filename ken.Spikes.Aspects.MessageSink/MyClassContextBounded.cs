using System;

namespace ken.Spikes.Aspects.MessageSink
{
    [MessageSink]
    public class MyClassContextBounded : ContextBoundObject, IMyInterface
    {
        public int Calc(int x, int y)
        {
            return x - y;
        }
    }
}
