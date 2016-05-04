using System;
using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogMethodOnInterceptedTypeTests : TestBase
    {
        public LogMethodOnInterceptedTypeTests()
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
            Log["Information"]
                .Select(l => l.Message)
                .Should()
                .BeEquivalentTo(
                    "Method: {Method} called with arguments {@Arguments} returned {@Result} after {Duration}");
        }

        [Fact]
        public void ThenAnInformationWithExpectedArgumentsShouldBeLogged()
        {
            var parameters = Log["Information"].Select(l => l.Parameters).First();
            parameters.ShouldBeEquivalentTo(new object[]
            {
                "Unity.Interception.Serilog.Tests.Support.IDummy.ReturnStuff",
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