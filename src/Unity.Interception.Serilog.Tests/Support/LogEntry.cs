using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Unity.Interception.Serilog.Tests.Support
{
    public class LogEntry
    {
        public string Message { get; set; }
        public IReadOnlyDictionary<string, object> Properties { get; set; }
        public Exception Exception { get; set; }
        public LogEventLevel Level { get; set; }
    }
}