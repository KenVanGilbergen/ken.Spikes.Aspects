using LinFu.AOP.Interfaces;
using Xunit;

namespace ken.Spikes.Aspects.LinFu
{
    /// <summary>
    /// https://github.com/philiplaureano/LinFu
    /// http://www.codeproject.com/Articles/20884/Introducing-the-LinFu-Framework-Part-I-LinFu-Dynam
    /// </summary>
    public class LinfuTests
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
        // System.BadImageFormatException: Could not load file or assembly 'ken.Spikes.Aspects.LinFu.dll' or one of its dependencies. File is corrupt. (Exception from HRESULT: 0x8013110E)
        // System.BadImageFormatException: File is corrupt. (Exception from HRESULT: 0x8013110E)
        public void ClassShouldBeIntercepted()
        {
            var c = new MyClass();

            var modifiableType = c as IModifiableType;
            if (null != modifiableType) 
                modifiableType.MethodBodyReplacementProvider = new SimpleMethodReplacementProvider(new MyInterceptor());
            
            var result = c.Calc(6, 3);
            Assert.Equal(6, result);
        }
    }
}

// you also need a postbuild event, it will add this interface for you 
// <PropertyGroup>
//   <PostWeaveTaskLocation>$(MSBuildProjectDirectory)\$(OutputPath)..\packages\LinFu.Core.2.3.0.41559\lib\net35\LinFu.Core.dll</PostWeaveTaskLocation>
// </PropertyGroup>
// <UsingTask TaskName="PostWeaveTask" AssemblyFile="$(PostWeaveTaskLocation)" />
// <Target Name="AfterBuild">
//   <PostWeaveTask TargetFile="$(MSBuildProjectDirectory)\$(OutputPath)$(MSBuildProjectName).dll" InterceptAllExceptions="false" InterceptAllFields="false" InterceptAllNewInstances="false" InterceptAllMethodCalls="false" InterceptAllMethodBodies="true" />
// </Target>