using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationOnInterceptedTypeAsyncWithResultTests : TestBase
    {
        public LogInformationOnInterceptedTypeAsyncWithResultTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog(level: LogEventLevel.Information)
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().ReturnStuffAsync(1, "b").Wait();
        }

        [Fact]
        public void ThenOneInformationLogShouldBeLogged()
        {
            Log.Single().Level.Should().Be(LogEventLevel.Information);
        }

        [Fact]
        public void ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            Log.Single().Message.Should().Be("Method \"ReturnStuffAsync\" returned");
        }

        [Fact]
        public void ThenAnInformationWithExpectedPropertiesShouldBeLogged()
        {
            var properties = Log.Single().Properties;
            properties["SourceContext"].Should().Be("Unity.Interception.Serilog.Tests.Support.IDummy");
            properties["EventId"].Should().Be("ReturnStuffAsync");
            properties["Arguments"].ShouldBeEquivalentTo(new Dictionary<string, object>
            {
                ["a"] = "1",
                ["b"] = "b"
            });
            properties["Result"].Should().Be("1 b");
            properties["Duration"].Should().Be(2000.0);
            properties["LogType"].Should().Be("Trace");
        }
    }
}