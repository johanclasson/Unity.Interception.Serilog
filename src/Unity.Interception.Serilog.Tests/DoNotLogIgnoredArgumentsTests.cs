using System;
using System.Collections.Generic;
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
                .WithConfiguredSerilog()
                .WithAStopWatch()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuffWithSecretParameter("username1", "abc123");
        }

        [Fact]
        public void ThenAnInformationWithExpectedPropertiesShouldBeLogged()
        {
            var properties = Log.Single().Properties;
            properties["SourceContext"].Should().Be("Unity.Interception.Serilog.Tests.Support.IDummy");
            properties["EventId"].Should().Be("DoStuffWithSecretParameter");
            properties["Arguments"].ShouldBeEquivalentTo(new Dictionary<string, object>()
            {
                ["username"] = "username1",
                ["password"] = "[hidden]"
            });
            properties["Duration"].Should().Be(2000.0);
        }
    }
}