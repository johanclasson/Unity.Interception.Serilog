using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Moq;

namespace Unity.Interception.Serilog.Tests.Support
{
    public class ContainerBuilder : IDisposable
    {
        private LoggerMock _loggerMock;
        public UnityContainer Container { get; } = new UnityContainer();
        public IReadOnlyCollection<LogEntry> Log => _loggerMock.Log;

        public ContainerBuilder WithConfiguredSerilog()
        {
            _loggerMock = new LoggerMock();
            Container.ConfigureSerilog(c => c.WriteTo.Logger(_loggerMock.Object));
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