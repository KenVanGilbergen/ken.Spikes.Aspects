using System;
using PostSharp.Aspects;

namespace ken.Spikes.Aspects.PostSharp
{
    [Serializable]
    public class MyAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Console.WriteLine("OnEntry");
        }
        
        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine("OnSuccess");
            args.ReturnValue = (int)args.ReturnValue * 2;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Console.WriteLine("OnExit");
        }
    }
}
