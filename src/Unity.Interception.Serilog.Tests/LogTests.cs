using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Moq;
using Serilog;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogTests : IDisposable
    {
        private readonly UnityContainer _container;
        private readonly List<LogEntry> _loggedInformationMessages;

        public LogTests()
        {
            var loggerMock = new Mock<ILogger>();
            _loggedInformationMessages = new List<LogEntry>();
            loggerMock.Setup(m => m.Information(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((m, p) => _loggedInformationMessages.Add(new LogEntry {Message = m, Parameters = p}));

            var stopWatchMock = new Mock<IStopWatch>();
            stopWatchMock.SetupGet(m => m.Elapsed).Returns(TimeSpan.FromSeconds(2));

            _container = new UnityContainer();
            _container.RegisterInstance(loggerMock.Object);
            _container.RegisterInstance(stopWatchMock.Object);
        }

        [Fact]
        public void InvokingMethodOnInterceptedType()
        {
            _container.RegisterLoggedType<IDummy, Dummy>();
        }

        [Fact]
        public void InvokingMethodOnInterceptedInstance()
        {
            _container.RegisterLoggedInstance<IDummy>(new Dummy());
        }

        public void Dispose()
        {
            _container.Resolve<IDummy>().DoStuff(1, "b");
            _loggedInformationMessages.Count.Should().Be(1);
            _loggedInformationMessages[0].Message.Should().Be("Method: {Method} called with arguments {@Arguments} returned {@Result} after {Duration}");
            _container?.Dispose();
        }

        //TODO: null as parameter tests
        //TODO: test parameters
        //TODO: Exception
        //TODO: Cache? Configure log and cache with attributes?
        //TODO: How do log message read for void methods?
        //TODO: How do log message read for methods without parameters?
        //TODO: Ignore logging parameter
    }
}