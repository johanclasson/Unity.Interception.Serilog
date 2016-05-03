using System;
using Microsoft.Practices.Unity;
using Moq;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class LogTests
    {
        [Fact(Skip = "PoC")]
        public void ShouldBeAbleToWriteToSqlServer()
        {
            ILogger log = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString: @"Server=.;Database=Serilog;Trusted_Connection=True;",
                    tableName: "Logs")
                .CreateLogger();
            var exception = new NotImplementedException("Level 1",
                new NotImplementedException("Level 2", new NotImplementedException("Level 3")));
            log.Information(exception, "The current weather is {@Weather}.", new {Condition = "Sunny", WindSpeed = 12.3});
        }

        [Fact(Skip = "PoC")]
        public void ShouldBeAbleToWriteToElasticsearch()
        {
            ILogger log = new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true
                })
                .CreateLogger();
            var exception = new NotImplementedException("Level 1",
                new NotImplementedException("Level 2", new NotImplementedException("Level 3")));
            log.Information(exception, "The current weather is {@Weather}.", new {Condition = "Sunny", WindSpeed = 8.4});
        }

        [Fact]
        public void FactMethodName()
        {
            var container = new UnityContainer();
            container.RegisterLoggedType<IDummy, Dummy>();
            var loggerMock = new Mock<ILogger>();
            container.RegisterInstance(loggerMock.Object);

            container.Resolve<IDummy>().DoStuff(1, "b");
            loggerMock.Verify(m => m.Information(It.IsAny<string>()));
        }
    }

    internal interface IDummy
    {
        string DoStuff(int a, string b);
    }

    public class Dummy : IDummy
    {
        public string DoStuff(int a, string b)
        {
            return $"{a} {b}";
        }
    }
}