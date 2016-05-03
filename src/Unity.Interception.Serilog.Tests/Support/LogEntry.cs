namespace Unity.Interception.Serilog.Tests.Support
{
    internal class LogEntry
    {
        public string Message { get; set; }
        public object[] Parameters { get; set; }
    }
}