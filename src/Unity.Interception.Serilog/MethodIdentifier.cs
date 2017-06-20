using System;

namespace Unity.Interception.Serilog
{
    /// <summary>Used to identify a method to ignore to log trace- or error logs from.</summary>
    public class MethodIdentifier
    {
        /// <summary>Creates a new instance of a method identifier used to identify a method to ignore to log trace- or error logs from.</summary>
        /// <param name="type">The type containing the method to ignore.</param>
        /// <param name="methodName">The name of the method to ignore.</param>
        public MethodIdentifier(Type type, string methodName)
        {
            Type = type;
            MethodName = methodName;
        }

        /// <summary>The type containing the method to ignore.</summary>
        public Type Type { get; }

        /// <summary>The name of the method to ignore.</summary>
        public string MethodName { get; }
    }
}