using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogWithoutReturnOrParameterTypesdTests : TestBase
    {
        public LogWithoutReturnOrParameterTypesdTests()
        {
            GivenThereExistsAnContainer()
                .WithAnInformationLogger()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenAnInformationMessageShouldBeLogged()
        {
            Log["Information"]
                .Select(l => l.Message)
                .Should()
                .BeEquivalentTo("Method: {Method} ran for {Duration}");
        }
    }
}