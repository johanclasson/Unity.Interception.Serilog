using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;
using SerilogLog=Serilog.Log;

namespace Unity.Interception.Serilog.StaticTests
{
    public class StaticLoggerTests : TestBase
    {
        public StaticLoggerTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog();
        }

        [Fact]
        public void ThenStaticLoggerShouldBeSet()
        {
            SerilogLog.Logger.Should().Be(Logger);
        }
    }
}