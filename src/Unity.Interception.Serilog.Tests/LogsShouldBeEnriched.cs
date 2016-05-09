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
        public void ThenAllLogsShouldHaveManagedThreadIdProperty()
        {
            Log.Single().Properties["ManagedThreadId"].Should().Be(1);
        }
        [Fact]
        public void ThenAllLogsShouldHaveMachineNameProperty()
        {
            Log.Single().Properties["MachineName"].Should().Be("m1");
        }
        [Fact]
        public void ThenAllLogsShouldHaveProcessIdProperty()
        {
            Log.Single().Properties["ProcessId"].Should().Be(2);
        }
        [Fact]
        public void ThenAllLogsShouldHaveProcessNameProperty()
        {
            Log.Single().Properties["ProcessName"].Should().Be("p1");
        }
        [Fact]
        public void ThenAllLogsShouldHaveThreadNameProperty()
        {
            Log.Single().Properties["ThreadName"].Should().Be("t1");
        }
        [Fact]
        public void ThenAllLogsShouldHaveAppDomainNameProperty()
        {
            Log.Single().Properties["AppDomainName"].Should().Be("a1");
        }
    }
}