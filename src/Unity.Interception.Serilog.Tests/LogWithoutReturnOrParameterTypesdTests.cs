using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogWithoutReturnOrParameterTypesdTests : IDisposable
    {
        private readonly ContainerBuilder _builder;

        public LogWithoutReturnOrParameterTypesdTests()
        {
            _builder = new ContainerBuilder()
                .WithInformationLoggerMock()
                .WithStopWatchMock();
        }

        [Fact]
        public void InvokingMethodOnInterceptedType()
        {
            _builder.Container
                .RegisterLoggedType<IDummy, Dummy>()
                .Resolve<IDummy>().DoStuff();
        }

        public void Dispose()
        {
            _builder.Log["Information"].Count.Should().Be(1);
            _builder.Log["Information"][0].Message.Should()
                .Be("Method: {Method} ran for {Duration}");
            _builder.Dispose();
        }
    }
}