using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Unity.Interception.Serilog
{
    internal interface ISerilogOptions
    {
        IEnumerable<Type> ExpectedExceptions { get; }
        IEnumerable<MethodIdentifier> IgnoredMethods { get; }
        LogEventLevel Level { get; }
    }

    internal class SerilogOptions : ISerilogOptions
    {
        public SerilogOptions(
            IEnumerable<Type> expectedExceptions,
            IEnumerable<MethodIdentifier> ignoredMethods,
            LogEventLevel level)
        {
            ExpectedExceptions = expectedExceptions ?? new Type[0];
            IgnoredMethods = ignoredMethods ?? new MethodIdentifier[0];
            Level = level;
        }

        public IEnumerable<Type> ExpectedExceptions { get; }
        public IEnumerable<MethodIdentifier> IgnoredMethods { get; }
        public LogEventLevel Level { get; }
    }
}