using System;
using System.Collections.Generic;

namespace Unity.Interception.Serilog
{
    public interface ISerilogOptions
    {
        IEnumerable<Type> ExpectedExceptions { get; set; }
    }

    internal class SerilogOptions : ISerilogOptions
    {
        public SerilogOptions(IEnumerable<Type> expectedExceptions)
        {
            ExpectedExceptions = expectedExceptions ?? new Type[0];
        }

        public IEnumerable<Type> ExpectedExceptions { get; set; }
    }
}