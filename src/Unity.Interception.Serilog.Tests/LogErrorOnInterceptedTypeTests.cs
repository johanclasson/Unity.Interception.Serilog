using System;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogErrorOnInterceptedTypeTests : TestBase
    {
        private const string ExpectedMessage = "Method {Method} failed after {Duration}";

        public LogErrorOnInterceptedTypeTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            Action a = () => WhenDummyIsResolvedAnd().ThrowException();
            a.ShouldThrow<InvalidOperationException>();
        }

        public void ThenOneErrorLogShouldBeLogged()
        {
            Log.Single().Level.Should().Be(LogEventLevel.Error);
        }

        [Fact]
        public void ThenAnErrorWithExpectedMessageShouldBeLogged()
        {
            Log.Single().Message.Should().Be(ExpectedMessage);
        }

        [Fact]
        public void ThenAnErrorWithExpectedPropertiesShouldBeLogged()
        {
            var properties = Log.Single().Properties;
            properties["Method"].Should().Be("Unity.Interception.Serilog.Tests.Support.IDummy.ThrowException");
            properties["Duration"].Should().Be(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void ThenAnErrorWithExpectedExceptionShouldBeLogged()
        {
            var exception = Log.Single().Exception;
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Be("Something bad happened");
        }
    }
}