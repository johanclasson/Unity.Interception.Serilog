using System;
using System.Collections.Generic;

namespace Unity.Interception.Serilog
{
    internal interface ISerilogOptions
    {
        IEnumerable<Type> ExpectedExceptions { get; }
        IEnumerable<MethodIdentifier> IgnoredMethods { get; }
    }

    internal class SerilogOptions : ISerilogOptions
    {
        public SerilogOptions(IEnumerable<Type> expectedExceptions, IEnumerable<MethodIdentifier> ignoredMethods)
        {
            ExpectedExceptions = expectedExceptions ?? new Type[0];
            IgnoredMethods = ignoredMethods ?? new MethodIdentifier[0];
        }

        public IEnumerable<Type> ExpectedExceptions { get; }
        public IEnumerable<MethodIdentifier> IgnoredMethods { get; }
    }
}