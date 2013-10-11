using Spring.Context.Support;
using Xunit;

namespace ken.Spikes.Aspects.Spring
{
    /// <summary>
    /// http://springframework.net/
    /// </summary>
    public class SpringTests
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
            var context = ContextRegistry.GetContext();
            var c = (IMyInterface) context.GetObject("MyInterceptedClass");
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
