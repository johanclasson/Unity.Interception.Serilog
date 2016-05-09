using System;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationFromExpectedExceptionsTests : TestBase
    {
        public LogInformationFromExpectedExceptionsTests()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog(typeof (InvalidOperationException))
                .WithADummyTypeRegistered();
            Action a = () => WhenDummyIsResolvedAnd().ThrowException();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            var entry = Log.Single();
            entry.Level.Should().Be(LogEventLevel.Information);
            entry.Message.Should().Be("Method Unity.Interception.Serilog.Tests.Support.IDummy.ThrowException failed");
        }
    }
}