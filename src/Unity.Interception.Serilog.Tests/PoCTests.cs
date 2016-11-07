using System;
using Microsoft.Practices.Unity;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;
using Unity.Interception.Serilog.Tests.Support;
using Xunit;

namespace Unity.Interception.Serilog.Tests
{
    public class PoCTests
    {
        [Fact(Skip = "PoC")]
        public void ShouldLogArgumentsToElasticsearch()
        {
            var container = new UnityContainer()
                .ConfigureSerilog(
                    c =>
                    {
                        c.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                        {
                            IndexFormat = "poc-{0:yyyy.MM.dd}"
                        });
                    }, expectedExceptions: new[] { typeof(InvalidOperationException) })
                .RegisterLoggedType<IDummy, Dummy>();
            using (container)
            {
                container.Resolve<IDummy>().ReturnStuff(1, "a");
                container.Resolve<IDummy>().DoStuff();
                try
                {
                    container.Resolve<IDummy>().ThrowException();
                }
                catch (Exception)
                {
                    // ignored
                }

            }
            // Examine log manually
        }



        [Fact(Skip = "PoC")]
        public void ShouldLogArgumentsToSqlServer()
        {
            var configuration = new LoggerConfiguration();
            configuration.WriteTo.MSSqlServer(
                "Server=.;Database=SerilogTest;Trusted_Connection=True;",
                "Log",
                autoCreateSqlTable: true,
                columnOptions: CreateColumnOptions());
            var container = new UnityContainer()
                .ConfigureSerilog(configuration, expectedExceptions: new[] { typeof(InvalidOperationException) })
                .RegisterLoggedType<IDummy, Dummy>();
            using (container)
            {
                container.Resolve<IDummy>().ReturnStuff(1, "a");
                container.Resolve<IDummy>().DoStuff();
                try
                {
                    container.Resolve<IDummy>().ThrowException();
                }
                catch (Exception)
                {
                    // ignored
                } 
            }

            // Examine log manually
        }

        private static ColumnOptions CreateColumnOptions()
        {
            var columnOptions = new ColumnOptions {AdditionalDataColumns = TraceLog.DataColumns};
            columnOptions.Properties.ExcludeAdditionalProperties = true;
            return columnOptions;
        }
    }
}
