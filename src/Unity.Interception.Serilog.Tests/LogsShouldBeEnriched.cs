using System.Linq;
using FluentAssertions;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogsShouldBeEnriched : TestBase
    {
        public LogsShouldBeEnriched()
        {
            GivenThereExistsAContainer()
                .WithFakeCommonProperties(managedThreadId: 1, machineName: "m1", processId: 2, processName: "p1", threadName: "t1", appDomainName: "a1")
                .WithConfiguredSerilog()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenLogsShouldHaveManagedThreadIdProperty()
        {
            Log.Single().Properties["ManagedThreadId"].Should().Be(1);
        }
        [Fact]
        public void ThenLogsShouldHaveMachineNameProperty()
        {
            Log.Single().Properties["MachineName"].Should().Be("m1");
        }
        [Fact]
        public void ThenLogsShouldHaveProcessIdProperty()
        {
            Log.Single().Properties["ProcessId"].Should().Be(2);
        }
        [Fact]
        public void ThenLogsShouldHaveProcessNameProperty()
        {
            Log.Single().Properties["ProcessName"].Should().Be("p1");
        }
        [Fact]
        public void ThenLogsShouldHaveThreadNameProperty()
        {
            Log.Single().Properties["ThreadName"].Should().Be("t1");
        }
        [Fact]
        public void ThenLogsShouldHaveAppDomainNameProperty()
        {
            Log.Single().Properties["AppDomainName"].Should().Be("a1");
        }
    }
}