namespace ken.Spikes.Aspects.PostSharp
{
    public class MyClassWithMyAspectAttribute : IMyInterface
    {
        [MyAspect] 
        public int Calc(int x, int y)
        {
            return x - y;
        }
    }
}
