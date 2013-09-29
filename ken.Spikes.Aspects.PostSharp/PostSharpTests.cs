using Xunit;

namespace ken.Spikes.Aspects.PostSharp
{
    /// <summary>
    /// http://www.postsharp.net/
    /// </summary>
    public class PostSharpTests
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
            var c = new MyClassWithMyAspectAttribute();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
