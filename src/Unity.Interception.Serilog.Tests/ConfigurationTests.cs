using System.Linq;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Serilog;
using Serilog.Filters;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void Function()
        {
            var loggerMock = new LoggerMock();
            var container = new UnityContainer();
            container.ConfigureSerilog(c => c
                .WriteTo.Logger(loggerMock.Object)
                .Filter.ByExcluding(Matching.WithProperty<string>("Name", n => n == "Johanna")));
            container.Resolve<ILogger>().Information("Hello {Name}!?", "Johan");
            container.Resolve<ILogger>().Information("Hello {Name}!?", "Johanna");
            loggerMock.Log.Single().Message.Should().Be("Hello {Name}!?");
        }
    }
}