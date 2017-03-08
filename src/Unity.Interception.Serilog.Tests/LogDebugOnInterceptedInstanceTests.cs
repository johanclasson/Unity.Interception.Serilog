using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogDebugOnInterceptedInstanceTests : TestBase
    {

        public LogDebugOnInterceptedInstanceTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog() //Default level is Debug
                .WithADummyInstanceRegistered();
            WhenDummyIsResolvedAnd().ReturnStuff(1, "b");
        }

        [Fact]
        public void ThenADebugMessageShouldBeLogged()
        {
            var entry = Log.Single();
            entry.Level.Should().Be(LogEventLevel.Debug);
        }
    }
}