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
        public void ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            Log.Single().Message.Should().Be("Method {Method} ran for {Duration}");
        }
    }
}