using System;
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
            Add(logEvent, propertyFactory, "ManagedThreadId", _properties.ManagedThreadId, 0);
            Add(logEvent, propertyFactory, "MachineName", _properties.MachineName, null);
            Add(logEvent, propertyFactory, "ProcessId", _properties.ProcessId, 0);
            Add(logEvent, propertyFactory, "ProcessName", _properties.ProcessName, null);
            Add(logEvent, propertyFactory, "ThreadName", _properties.ThreadName, null);
            Add(logEvent, propertyFactory, "AppDomainName", _properties.AppDomainName, null);
        }

        private static void Add<T>(LogEvent logEvent, ILogEventPropertyFactory propertyFactory, string name, T value, T defaultValue) where T : IComparable
        {
            if (value.CompareTo(defaultValue) != 0)
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(name, value));
        }
    }
}