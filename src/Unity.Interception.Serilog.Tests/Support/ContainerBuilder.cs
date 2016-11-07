using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Practices.Unity;
using Moq;
using Serilog.Debugging;

namespace Unity.Interception.Serilog.Tests.Support
{
    public class ContainerBuilder : IDisposable
    {
        private LoggerMock _loggerMock;
        private StringWriter _selfLogOutput;
        public string SelfLogOutput => _selfLogOutput.ToString();

        public UnityContainer Container { get; } = new UnityContainer();
        public IReadOnlyCollection<LogEntry> Log => _loggerMock.Log;

        public ContainerBuilder WithConfiguredSerilog(Type[] expectedExceptions = null, MethodIdentifier[] ignoredMethods = null)
        {
            _loggerMock = new LoggerMock();
            Container.ConfigureSerilog(c => c.WriteTo.Logger(_loggerMock.Object), expectedExceptions, ignoredMethods);
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

        public ContainerBuilder WithFakeCommonProperties(int managedThreadId = 0, string machineName = null,
            int processId = 0, string processName = null, string threadName = null, string appDomainName = null)
        {
            _selfLogOutput = new StringWriter();
            SelfLog.Enable(TextWriter.Synchronized(_selfLogOutput));

            var mock = new Mock<ICommonProperties>();
            mock.SetupGet(m => m.ManagedThreadId).Returns(managedThreadId);
            mock.SetupGet(m => m.MachineName).Returns(machineName);
            mock.SetupGet(m => m.ProcessId).Returns(processId);
            mock.SetupGet(m => m.ProcessName).Returns(processName);
            mock.SetupGet(m => m.ThreadName).Returns(threadName);
            mock.SetupGet(m => m.AppDomainName).Returns(appDomainName);
            Container.RegisterInstance(mock.Object);
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