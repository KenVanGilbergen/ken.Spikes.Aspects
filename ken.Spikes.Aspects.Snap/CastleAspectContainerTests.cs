using System;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Snap;
using Snap.CastleWindsor;
using Xunit;

namespace ken.Spikes.Aspects.Snap
{
    /// <summary>
    /// https://github.com/TylerBrinks/Snap/wiki
    /// </summary>
    public class CastleAspectContainerTests
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
            var container = new WindsorContainer();

            SnapConfiguration.For(new CastleAspectContainer(container.Kernel)).Configure(_ =>
            {
                //c.IncludeNamespace("ConsoleApplication1");
                _.Bind<MyInterceptor>().To<MyInterceptorAttribute>();
            });

            container.Register(Component.For<IMyInterface>().ImplementedBy<MyClassWithInterceptorAttribute>());

            var c = container.Resolve<IMyInterface>();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
