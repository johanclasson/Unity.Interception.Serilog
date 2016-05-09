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
                .WithConfiguredSerilog(ignoredMethods: new[] {new MethodIdentifier(typeof (IDummy), nameof(IDummy.DoStuff))})
                .WithADummyTypeRegistered();
            //Method which is ignored by type and name
            WhenDummyIsResolvedAnd().DoStuff();
            //Method which is ignored by attribute
            WhenDummyIsResolvedAnd().DoSecretStuff();
        }

        [Fact]
        public void ThenNoInformationMessageShouldBeLogged()
        {
            Log.Count.Should().Be(0);
        }
    }
}