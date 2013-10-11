using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Xunit;

namespace ken.Spikes.Aspects.Unity
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff660891
    /// </summary>
    public class UnityTests
    {
        [Fact]
        public void AnEmptyTestShouldAlwaysSucceed()
        {
        }

        [Fact]
        public void ClassShouldNonBeIntercepted()
        {
            var c = new MyClass();
            var result = c.Calc(6, 3);
            Assert.Equal(3, result);
        }

        [Fact]
        public void ClassShouldBeIntercepted()
        {
            var container = new UnityContainer();
            container
                .AddNewExtension<Interception>()
                .RegisterType<IMyInterface, MyClass>(
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<MyAspect>()
                );

            var c = container.Resolve<IMyInterface>();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
