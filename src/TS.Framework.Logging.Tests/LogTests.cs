using System;
using Serilog;
using Xunit;

namespace TS.Framework.Logging.Tests
{
    public class LogTests
    {
        [Fact]
        public void ShouldBeAbleToWriteToSqlServer()
        {
            ILogger log = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString: @"Server=.;Database=Serilog;Trusted_Connection=True;", tableName: "Logs")
                .CreateLogger();
            var exception = new NotImplementedException("Level 1", new NotImplementedException("Level 2", new NotImplementedException("Level 3")));
            log.Information(exception, "The current weather is {@Weather}.", new { Condition = "Sunny", WindSpeed = 12.3});
        }
    }
}