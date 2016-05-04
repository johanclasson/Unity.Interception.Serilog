using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogMethodOnInterceptedInstanceTests : TestBase
    {

        public LogMethodOnInterceptedInstanceTests()
        {
            GivenThereExistsAnContainer()
                .WithAnInformationLogger()
                .WithAStopWatch()
                .WithADummyInstanceRegistered();
            WhenDummyIsResolvedAnd().ReturnStuff(1, "b");
        }

        [Fact]
        public void ThenAnInformationMessageShouldBeLogged()
        {
            Log["Information"].Count.Should().Be(1);
        }
    }
}