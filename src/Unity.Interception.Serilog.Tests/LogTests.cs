using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogTests : IDisposable
    {
        private readonly ContainerBuilder _builder;

        public LogTests()
        {
            _builder = new ContainerBuilder()
                .WithInformationLoggerMock()
                .WithStopWatchMock();
        }

        [Fact]
        public void InvokingMethodOnInterceptedType()
        {
            _builder.Container.RegisterLoggedType<IDummy, Dummy>();
        }

        [Fact]
        public void InvokingMethodOnInterceptedInstance()
        {
            _builder.Container.RegisterLoggedInstance<IDummy>(new Dummy());
        }

        public void Dispose()
        {
            _builder.Container.Resolve<IDummy>().DoStuff(1, "b");
            _builder.Log["Information"].Count.Should().Be(1);
            _builder.Log["Information"][0].Message.Should().Be("Method: {Method} called with arguments {@Arguments} returned {@Result} after {Duration}");
            _builder.Dispose();
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