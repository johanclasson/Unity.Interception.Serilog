using System;

namespace Unity.Interception.Serilog.Tests.Support
{
    public class LogEntry
    {
        public string Message { get; set; }
        public object[] Parameters { get; set; }
        public Exception Exception { get; set; }
    }
}