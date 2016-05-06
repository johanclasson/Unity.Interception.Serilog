using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationOnInterceptedInstanceTests : TestBase
    {

        public LogInformationOnInterceptedInstanceTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithADummyInstanceRegistered();
            WhenDummyIsResolvedAnd().ReturnStuff(1, "b");
        }

        [Fact]
        public void ThenAnInformationMessageShouldBeLogged()
        {
            Log.Count.Should().Be(1);
        }
    }
}