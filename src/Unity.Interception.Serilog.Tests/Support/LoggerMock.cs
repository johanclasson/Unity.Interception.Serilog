using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Serilog;
using Serilog.Events;

namespace Unity.Interception.Serilog.Tests.Support
{
    internal class LoggerMock : Mock<ILogger>
    {
        public LoggerMock() : base(MockBehavior.Strict)
        {
            Setup(m => m.Write(It.IsAny<LogEvent>())).Callback<LogEvent>(AddLog);
        }

        public IReadOnlyCollection<LogEntry> Log => _log.AsReadOnly();
        private readonly List<LogEntry> _log = new List<LogEntry>();

        private void AddLog(LogEvent logEvent)
        {
            var properties = FetchProperties(logEvent);
            _log.Add(new LogEntry
            {
                Exception = logEvent.Exception,
                Properties = properties,
                Message = logEvent.RenderMessage(),
                Level = logEvent.Level
            });
        }

        private static IReadOnlyDictionary<string, object> FetchProperties(LogEvent logEvent)
        {
            return logEvent.Properties.ToDictionary(p => p.Key, p => FetchProperty(p.Value)).AsReadOnly();
        }

        private static object FetchProperty(LogEventPropertyValue property)
        {
            var scalar = property as ScalarValue;
            if (scalar != null)
                return scalar.Value;
            var structure = property as StructureValue;
            if (structure != null)
                return FetchStructureProperties(structure);
            var sequence = property as SequenceValue;
            if (sequence != null)
                return FetchSequence(sequence);
            throw new InvalidOperationException();
        }

        private static object FetchSequence(SequenceValue sequence)
        {
            var result = sequence.Elements.Select(FetchProperty).ToArray();
            var dics = result.OfType<IReadOnlyDictionary<string, object>>().ToArray();
            var allResultsAreDics = result.Length == dics.Length;
            if (!allResultsAreDics)
                return result;
            var mergedDic = new Dictionary<string, object>();
            foreach (var dic in dics)
            {
                mergedDic[(string) dic["Name"]] = dic["Value"];
            }
            return mergedDic;
        }

        private static IReadOnlyDictionary<string, object> FetchStructureProperties(StructureValue structure)
        {
            return structure.Properties.ToDictionary(p => p.Name, p => FetchProperty(p.Value)).AsReadOnly();
        }
    }
}