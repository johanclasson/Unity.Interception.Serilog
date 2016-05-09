using Serilog.Core;
using Serilog.Events;

namespace Unity.Interception.Serilog
{
    internal class CommonPropertiesEnricher : ILogEventEnricher
    {
        private readonly ICommonProperties _properties;

        public CommonPropertiesEnricher(ICommonProperties properties)
        {
            _properties = properties;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ManagedThreadId", _properties.ManagedThreadId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("MachineName", _properties.MachineName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ProcessId", _properties.ProcessId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ProcessName", _properties.ProcessName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadName", _properties.ThreadName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("AppDomainName", _properties.AppDomainName));
        }
    }
}