using Xunit;

namespace ken.Spikes.Aspects.Fody
{
    /// <summary>
    /// https://github.com/Fody
    /// </summary>
    public class FodyTests
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

        //http://channel9.msdn.com/Forums/TechOff/259443-Using-SystemReflection-to-obtain-parameter-values-dynamically
        //People are going to keep telling you reflection will work when it won't, so here is the function that's actually capable of getting argument values. It's not likely to work reliably with optimization enabled (for example, there might not even be a stack frame when inlining is on) and getting a debugger installed so you can call that function won't be nearly as simple as you were hoping.
        //http://msdn.microsoft.com/en-us/library/ms231057.aspx
        //I probably just need another Fody plugin or else use https://github.com/Dresel/MethodCache to correct the decorator implementation?
        [Fact(Skip="Doesn't seem practical to change params and return value using MethodBase in MyInterceptorAttribute")]
        public void ClassShouldBeIntercepted()
        {
            var c = new  MyClassWithMyInterceptorAttribute();
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}
