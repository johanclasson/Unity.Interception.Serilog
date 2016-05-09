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
        public void ShouldLogArgumentsToElasticsearch()
        {
            var container = new UnityContainer();
            container
                .ConfigureSerilog(
                    c =>
                    {
                        c.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                        {
                            AutoRegisterTemplate = true
                        });
                    }, expectedExceptions: new[] {typeof (InvalidOperationException)})
                .RegisterLoggedType<IDummy, Dummy>();
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

            // Examine log manually
        }
    }
}