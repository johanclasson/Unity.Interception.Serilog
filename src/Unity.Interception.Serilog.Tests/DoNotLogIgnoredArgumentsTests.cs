using System;
using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class DoNotLogIgnoredArgumentsTests : TestBase
    {
        public DoNotLogIgnoredArgumentsTests()
        {
            GivenThereExistsAContainer()
                .WithAnInformationLogger()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuffWithSecretParameter("username1", "abc123");
        }

        [Fact]
        public void ThenAnInformationWithExpectedArgumentsShouldBeLogged()
        {
            var parameters = Log["Information"].Select(l => l.Parameters).First();
            parameters.ShouldBeEquivalentTo(new object[]
            {
                "Unity.Interception.Serilog.Tests.Support.IDummy.DoStuffWithSecretParameter",
                new object[]
                {
                    new {Name = "username", Value = "username1"},
                    new {Name = "password", Value = "[hidden]"}
                },
                TimeSpan.FromSeconds(2)
            });
        }
    }
}