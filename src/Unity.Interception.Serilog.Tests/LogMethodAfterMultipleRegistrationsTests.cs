using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogMethodAfterMultipleRegistrationsTests : TestBase
    {
        public LogMethodAfterMultipleRegistrationsTests()
        {
            GivenThereExistsAContainer()
                .WithAnInformationLogger()
                .WithADummyTypeRegistered()
                //TODO: Fix multiple registration of types! .WithADummyTypeRegistered()
                .WithADummyInstanceRegistered()
                .WithADummyInstanceRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenAnInformationMessageShouldBeLogged()
        {
            Log["Information"].Count.Should().Be(1);
        }
    }
}