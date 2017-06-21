using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationOnInterceptedTypeAsyncTests : TestBase
    {
        public LogInformationOnInterceptedTypeAsyncTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog(level: LogEventLevel.Information)
                .WithAStopWatch()
                .WithADummyTypeRegistered();
        }

        [Fact]
        public async Task ThenOneInformationLogShouldBeLogged()
        {
            await WhenDummyIsResolvedAnd().DoStuffAsync();
            Log.Single().Level.Should().Be(LogEventLevel.Information);
        }

        [Fact]
        public async Task ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            await WhenDummyIsResolvedAnd().DoStuffAsync();
            Log.Single().Message.Should().Be("Method \"DoStuffAsync\" returned");
        }

        [Fact]
        public async Task ThenAnInformationWithExpectedPropertiesShouldBeLogged()
        {
            await WhenDummyIsResolvedAnd().DoStuffAsync();
            var properties = Log.Single().Properties;
            properties["SourceContext"].Should().Be("Unity.Interception.Serilog.Tests.Support.IDummy");
            properties["EventId"].Should().Be("DoStuffAsync");
            properties.ContainsKey("Arguments").Should().BeFalse();
            properties["Result"].Should().BeNull();
            properties["Duration"].Should().Be(2000.0);
            properties["LogType"].Should().Be("Trace");
        }
    }
}