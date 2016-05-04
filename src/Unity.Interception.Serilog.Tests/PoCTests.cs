using System;
using Microsoft.Practices.Unity;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class PoCTests
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
            log.Information(exception, "The current weather is {@Weather}.", new { Condition = "Sunny", WindSpeed = 12.3 });
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
            log.Information(exception, "The current weather is {@Weather}.", new { Condition = "Sunny", WindSpeed = 8.4 });
        }

        [Fact(Skip = "PoC")]
        public void ShouldLogArgumentsToElasticsearch()
        {
            ILogger log = new LoggerConfiguration()
               .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
               {
                   AutoRegisterTemplate = true
               })
               .CreateLogger();
            var container = new UnityContainer();
            container.RegisterLoggedType<IDummy, Dummy>();
            container.RegisterInstance(log);
            container.Resolve<IDummy>().ReturnStuff(1, "a");
            container.Resolve<IDummy>().DoStuff();
            try { container.Resolve<IDummy>().ThrowException(); }
            catch (Exception)
            {
                // ignored
            }

            // Examine log manually
        }
    }
}