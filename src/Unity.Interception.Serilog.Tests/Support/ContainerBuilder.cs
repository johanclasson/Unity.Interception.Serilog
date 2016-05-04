using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Moq;
using Serilog;

namespace Unity.Interception.Serilog.Tests.Support
{
    public class ContainerBuilder : IDisposable
    {
        public ContainerBuilder()
        {
            Log = new Dictionary<string, List<LogEntry>>
            {
                ["Information"] = new List<LogEntry>(),
                ["Error"] = new List<LogEntry>()
            };
        }

        public UnityContainer Container { get; } = new UnityContainer();
        public Dictionary<string, List<LogEntry>> Log { get; }

        public ContainerBuilder WithAnInformationLogger()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock
                .Setup(m => m.Information(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((m, p) => Log["Information"].Add(new LogEntry { Message = m, Parameters = p }));
            loggerMock
                .Setup(m => m.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((e, m, p) => Log["Error"].Add(new LogEntry { Exception = e, Message = m, Parameters = p }));
            Container.RegisterInstance(loggerMock.Object);
            return this;
        }

        public ContainerBuilder WithAStopWatch()
        {
            var stopWatchMock = new Mock<IStopWatch>();
            stopWatchMock
                .SetupGet(m => m.Elapsed)
                .Returns(TimeSpan.FromSeconds(2));
            Container.RegisterInstance(stopWatchMock.Object);
            return this;
        }

        public ContainerBuilder WithADummyTypeRegistered()
        {
            Container.RegisterLoggedType<IDummy, Dummy>();
            return this;
        }

        public ContainerBuilder WithADummyInstanceRegistered()
        {
            Container.RegisterLoggedInstance<IDummy>(new Dummy());
            return this;
        }

        public void Dispose()
        {
            Container?.Dispose();
        }

    }
}