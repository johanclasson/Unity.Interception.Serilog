using System;
using System.Linq;
using FluentAssertions;
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
                .WithAnInformationLogger()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            Action a = () => WhenDummyIsResolvedAnd().ThrowException();
            a.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ThenAnErrorWithExpectedMessageShouldBeLogged()
        {
            Log["Information"].Count.Should().Be(0);
            Log["Error"]
                .Select(l => l.Message)
                .Should()
                .BeEquivalentTo(ExpectedMessage);
        }

        [Fact]
        public void ThenAnErrorWithExpectedArgumentsShouldBeLogged()
        {
            var parameters = Log["Error"].Select(l => l.Parameters).First();
            parameters.ShouldBeEquivalentTo(new object[]
            {
                "Unity.Interception.Serilog.Tests.Support.IDummy.ThrowException",
                TimeSpan.FromSeconds(2)
            });
        }

        [Fact]
        public void ThenAnErrorWithExpectedExceptionShouldBeLogged()
        {
            var exception = Log["Error"].Select(l => l.Exception).First();
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Be("Something bad happened");
        }
    }
}