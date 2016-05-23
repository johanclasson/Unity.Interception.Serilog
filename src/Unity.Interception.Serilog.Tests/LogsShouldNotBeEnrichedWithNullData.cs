using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogsShouldNotBeEnrichedWithNullData : TestBase
    {
        public LogsShouldNotBeEnrichedWithNullData()
        {
            GivenThereExistsAContainer()
                .WithFakeCommonProperties()
                .WithConfiguredSerilog()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenLogsShouldNotHaveManagedThreadIdProperty()
        {
            Log.Single().Properties.ContainsKey("ManagedThreadId").Should().BeFalse();
        }

        [Fact]
        public void ThenLogsShouldNotHaveMachineNameProperty()
        {
            Log.Single().Properties.ContainsKey("MachineName").Should().BeFalse();
        }

        [Fact]
        public void ThenLogsShouldNotHaveProcessIdProperty()
        {
            Log.Single().Properties.ContainsKey("ProcessId").Should().BeFalse();
        }

        [Fact]
        public void ThenLogsShouldNotHaveProcessNameProperty()
        {
            Log.Single().Properties.ContainsKey("ProcessName").Should().BeFalse();
        }

        [Fact]
        public void ThenLogsShouldNotHaveThreadNameProperty()
        {
            Log.Single().Properties.ContainsKey("ThreadName").Should().BeFalse();
        }

        [Fact]
        public void ThenLogsShouldNotHaveAppDomainNameProperty()
        {
            Log.Single().Properties.ContainsKey("AppDomainName").Should().BeFalse();
        }

        [Fact]
        public void ThenNoErrorsShouldBeWrittenToSelfLog()
        {
            SelfLog.Should().BeEmpty();
        }
    }
}