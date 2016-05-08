using System.Linq;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogsShouldBeEnrichedWithThreadId : TestBase
    {
        public LogsShouldBeEnrichedWithThreadId()
        {
            GivenThereExistsAContainer()
                .WithConfiguredSerilog()
                .WithADummyTypeRegistered();
            WhenDummyIsResolvedAnd().DoStuff();
        }

        [Fact]
        public void ThenAllLogsShouldHaveManagedThreadIdProperty()
        {
            Log.Single().Properties.ContainsKey("ManagedThreadId");
        }
    }
}