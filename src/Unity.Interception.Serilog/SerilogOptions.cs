using System;
using System.Collections.Generic;

namespace Unity.Interception.Serilog
{
    public interface ISerilogOptions
    {
        IEnumerable<Type> ExpectedExceptions { get; set; }
    }

    public class SerilogOptions : ISerilogOptions
    {
        public IEnumerable<Type> ExpectedExceptions { get; set; } = new Type[0];
    }
}