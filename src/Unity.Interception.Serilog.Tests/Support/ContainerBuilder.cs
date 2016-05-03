using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Moq;
using Serilog;

namespace Unity.Interception.Serilog.Tests.Support
{
    internal class ContainerBuilder : IDisposable
    {
        public ContainerBuilder()
        {
            Log = new Dictionary<string, List<LogEntry>>
            {
                ["Information"] = new List<LogEntry>()
            };
        }

        public UnityContainer Container { get; } = new UnityContainer();
        public Dictionary<string, List<LogEntry>> Log { get; }

        public ContainerBuilder WithInformationLoggerMock()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock
                .Setup(m => m.Information(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((m, p) => Log["Information"].Add(new LogEntry { Message = m, Parameters = p }));
            Container.RegisterInstance(loggerMock.Object);
            return this;
        }

        public ContainerBuilder WithStopWatchMock()
        {
            var stopWatchMock = new Mock<IStopWatch>();
            stopWatchMock
                .SetupGet(m => m.Elapsed)
                .Returns(TimeSpan.FromSeconds(2));
            Container.RegisterInstance(stopWatchMock.Object);
            return this;
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}