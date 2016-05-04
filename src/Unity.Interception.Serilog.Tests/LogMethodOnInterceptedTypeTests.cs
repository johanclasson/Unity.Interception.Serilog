using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogMethodOnInterceptedTypeTests : TestBase
    {

        public LogMethodOnInterceptedTypeTests()
        {
            GivenThereExistsAnContainer()
                .WithAnInformationLogger()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().ReturnStuff(1, "b");
        }

        [Fact]
        public void ThenAnInformationMessageShouldBeLogged()
        {
            Log["Information"]
                .Select(l => l.Message)
                .Should()
                .BeEquivalentTo("Method: {Method} called with arguments {@Arguments} returned {@Result} after {Duration}");
        }
    }
}