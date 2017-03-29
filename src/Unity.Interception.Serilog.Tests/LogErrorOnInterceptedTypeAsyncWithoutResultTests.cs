using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogErrorOnInterceptedTypeAsyncWithoutResultTests : TestBase
    {
        public LogErrorOnInterceptedTypeAsyncWithoutResultTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            Func<Task> a = async () => await WhenDummyIsResolvedAnd().ThrowExceptionWithoutResultAsync();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ThenOneErrorLogShouldBeLogged()
        {
            Log.Single().Level.Should().Be(LogEventLevel.Error);
        }

        [Fact]
        public void ThenAnErrorWithExpectedMessageShouldBeLogged()
        {
            Log.Single().Message.Should().Be("Method \"ThrowExceptionWithoutResultAsync\" failed");
        }

        [Fact]
        public void ThenAnErrorWithExpectedPropertiesShouldBeLogged()
        {
            var properties = Log.Single().Properties;
            properties["SourceContext"].Should().Be("Unity.Interception.Serilog.Tests.Support.IDummy");
            properties["EventId"].Should().Be("ThrowExceptionWithoutResultAsync");
            properties["Duration"].Should().Be(2000.0);
            properties["ExceptionType"].Should().Be("System.InvalidOperationException");
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