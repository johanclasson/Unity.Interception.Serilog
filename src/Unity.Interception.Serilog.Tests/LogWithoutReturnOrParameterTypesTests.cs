using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogWithoutReturnOrParameterTypesTests : TestBase
    {
        private const string ExpectedMessage = "Method {Method} ran for {Duration}";

        public LogWithoutReturnOrParameterTypesTests()
        {
            GivenThereExistsAContainer()
                .WithAnInformationLogger()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            Log["Information"]
                .Select(l => l.Message)
                .Should()
                .BeEquivalentTo(ExpectedMessage);
        }
    }
}