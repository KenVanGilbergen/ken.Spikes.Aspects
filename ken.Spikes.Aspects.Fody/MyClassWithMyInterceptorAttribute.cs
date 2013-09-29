namespace ken.Spikes.Aspects.Fody
{
    public class MyClassWithMyInterceptorAttribute : IMyInterface
    {
        [MyInterceptor] 
        public int Calc(int x, int y)
        {
            return x - y;
        }
    }
}
