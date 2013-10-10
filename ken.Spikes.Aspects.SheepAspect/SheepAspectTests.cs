using Xunit;

namespace ken.Spikes.Aspects.SheepAspect
{
    /// <summary>
    /// http://sheepaspect.codeplex.com
    /// </summary>
    public class SheepAspectTests
    {
        [Fact]
        public void AnEmptyTestShouldAlwaysSucceed()
        {
        }

        [Fact]
        public void ClassShouldAlwaysBeIntercepted()
        {
            var c = new MyClass();
            var result = c.Calc(6, 3);
            Assert.NotEqual(3, result);
        }

        [Fact]
        public void ClassShouldBeIntercepted()
        {
            var c = new MyClass();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
