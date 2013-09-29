using System;
using Xunit;

namespace ken.Spikes.Aspects.Emit
{
    /// <summary>
    /// Original code from Stefan at Atlasteit
    /// http://atlasteit.wordpress.com/2012/06/29/advanced-c-programming-2-decorator-proxy-and-aop/
    /// </summary>
    public class EmitTests
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
        public void ClassShouldBeInterceptedUsingExample()
        {
            var c = new  MyClass();
            var decorator = new DecoratorExampleProxy(c, new MyDecorator());

            var result = decorator.Calc(6, 3);
            Assert.Equal(6, result);
        }

        [Fact]
        public void ClassShouldBeInterceptedUsingGenerator()
        {
            var c = new MyClass();
            var decorator = Decorator.CreateDecorator<IMyInterface, MyDecorator>(c, new MyDecorator());
            
            var result = decorator.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
