using Xunit;

namespace ken.Spikes.Aspects.MessageSink
{
    /// <summary>
    /// http://www.codeproject.com/Articles/8414/The-simplest-AOP-scenario-in-C
    /// </summary>
    public class MessageSinkTests
    {
        [Fact]
        public void AnEmptyTestShouldAlwaysSucceed()
        {
        }

        [Fact]
        public void ClassShouldNotBeIntercepted()
        {
            var c = new MyClass();
            var result = c.Calc(6, 3);
            Assert.Equal(3, result);
        }

        [Fact]
        public void ClassShouldBeIntercepted()
        {
            var c = new MyClassContextBounded();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }

    }
}
