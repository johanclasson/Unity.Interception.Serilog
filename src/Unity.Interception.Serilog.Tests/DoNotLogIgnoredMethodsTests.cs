using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class DoNotLogIgnoredMethodsTests : TestBase
    {
        public DoNotLogIgnoredMethodsTests()
        {
            GivenThereExistsAContainer()
                .WithAnInformationLogger()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoSecretStuff();
        }

        [Fact]
        public void ThenNoInformationMessageShouldBeLogged()
        {
            Log["Information"].Count.Should().Be(0);
        }
    }
}