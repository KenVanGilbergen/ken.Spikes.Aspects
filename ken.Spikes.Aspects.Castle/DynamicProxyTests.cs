using System;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Xunit;

namespace ken.Spikes.Aspects.Castle
{
    /// <summary>
    /// http://kozmic.net/dynamic-proxy-tutorial/
    /// </summary>
    public class DynamicProxyTests
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
        public void ClassShouldBeInterceptedUsingProxyGenerator()
        {
            var generator = new ProxyGenerator();
            var proxy = (IMyInterface) generator.CreateClassProxy(
                typeof(MyClass), 
                new Type[] {typeof (IMyInterface)}, new MyInterceptor());

            var result = proxy.Calc(6, 3);
            Assert.Equal(6, result);
        }

        [Fact]
        public void ClassShouldBeInterceptedUsingContainer()
        {
            var container = new WindsorContainer();
            container
                .Register(Component.For<MyInterceptor>())
                .Register(Component.For<IMyInterface>().ImplementedBy<MyClass>().Interceptors<MyInterceptor>());

            var c = container.Resolve<IMyInterface>();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
