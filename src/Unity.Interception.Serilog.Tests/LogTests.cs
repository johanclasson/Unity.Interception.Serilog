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

            _container = new UnityContainer();
            _container.RegisterInstance(loggerMock.Object);

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
            _loggedInformationMessages[0].Message.Should().Be("Method: {Name} called with arguments {@Arguments} returned {@Result}");
            _container?.Dispose();
        }
    }
}