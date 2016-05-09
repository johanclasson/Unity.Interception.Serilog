using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogWithoutReturnOrParameterTypesTests : TestBase
    {
        public LogWithoutReturnOrParameterTypesTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenAnInformationWithExpectedPropertiesShouldBeLogged()
        {
            var properties = Log.Single().Properties;
            properties.Keys.Should().NotContain("Arguments");
            properties.Keys.Should().NotContain("Result");
        }
    }
}