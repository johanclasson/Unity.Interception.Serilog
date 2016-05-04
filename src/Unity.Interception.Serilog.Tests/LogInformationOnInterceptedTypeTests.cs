using System;
using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogInformationOnInterceptedTypeTests : TestBase
    {
        private const string ExpectedMessage = "Method {Method} called with arguments {@Arguments} returned {@Result} after {Duration}";
        private const string ExpectedMethod = "Unity.Interception.Serilog.Tests.Support.IDummy.ReturnStuff";

        public LogInformationOnInterceptedTypeTests()
        {
            GivenThereExistsAContainer()
                .WithAnInformationLogger()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().ReturnStuff(1, "b");
        }

        [Fact]
        public void ThenAnInformationWithExpectedMessageShouldBeLogged()
        {
            Log["Error"].Count.Should().Be(0);
            Log["Information"]
                .Select(l => l.Message)
                .Should()
                .BeEquivalentTo(ExpectedMessage);
        }

        [Fact]
        public void ThenAnInformationWithExpectedArgumentsShouldBeLogged()
        {
            var parameters = Log["Information"].Select(l => l.Parameters).First();
            parameters.ShouldBeEquivalentTo(new object[]
            {
                ExpectedMethod,
                new object[]
                {
                    new {Name = "a", Value = "1"},
                    new {Name = "b", Value = "b"}
                },
                "1 b",
                TimeSpan.FromSeconds(2)
            });
        }
    }
}