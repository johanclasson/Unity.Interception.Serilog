using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationAfterMultipleRegistrationsTests : TestBase
    {
        public LogInformationAfterMultipleRegistrationsTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithADummyTypeRegistered()
                //TODO: Fix multiple registration of types! .WithADummyTypeRegistered()
                .WithADummyInstanceRegistered()
                .WithADummyInstanceRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenAnInformationMessageShouldBeLogged()
        {
            Log.Count.Should().Be(1);
        }
    }
}