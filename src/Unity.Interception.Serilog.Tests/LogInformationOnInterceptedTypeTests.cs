using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationOnInterceptedTypeTests : TestBase
    {
        public LogInformationOnInterceptedTypeTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().ReturnStuff(1, "b");
        }

        public void ThenOneInformationLogShouldBeLogged()
        {
            Log.Single().Level.Should().Be(LogEventLevel.Information);
        }

        [Fact]
        public void ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            Log.Single().Message.Should().Be("Method {Method} called with arguments {@Arguments} returned {@Result} after {Duration}");
        }

        [Fact]
        public void ThenAnInformationWithExpectedPropertiesShouldBeLogged()
        {
            var properties = Log.Single().Properties;
            properties["Method"].Should().Be("Unity.Interception.Serilog.Tests.Support.IDummy.ReturnStuff");
            properties["Arguments"].ShouldBeEquivalentTo(new Dictionary<string, object>()
            {
                ["a"] = "1",
                ["b"] = "b"
            });
            properties["Result"].Should().Be("1 b");
            properties["Duration"].Should().Be(TimeSpan.FromSeconds(2));
        }
    }
}