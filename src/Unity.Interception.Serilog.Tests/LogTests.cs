using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Moq;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogTests : IDisposable
    {
        private readonly UnityContainer _container;
        private readonly IDummy _dummy;
        private readonly List<LogEntry> _loggedInformationMessages;

        public LogTests()
        {
            _container = new UnityContainer();
            _container.RegisterLoggedType<IDummy, Dummy>();
            var loggerMock = new Mock<ILogger>();
            _loggedInformationMessages = new List<LogEntry>();
            loggerMock.Setup(m => m.Information(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((m, p) => _loggedInformationMessages.Add(new LogEntry {Message = m, Parameters = p}));
            _container.RegisterInstance(loggerMock.Object);

            _dummy = _container.Resolve<IDummy>();
        }

        [Fact]
        public void InvokingMethodOnInterceptedType_ShouldLog()
        {
            _dummy.DoStuff(1, "b");
            _loggedInformationMessages.Count.Should().Be(1);
            _loggedInformationMessages[0].Message.Should().Be("Method: {Name} called with arguments {@Arguments} returned {@Result}");
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}