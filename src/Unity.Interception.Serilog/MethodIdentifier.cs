using System;

namespace Unity.Interception.Serilog
{
    public class MethodIdentifier
    {
        public MethodIdentifier(Type type, string methodName)
        {
            Type = type;
            MethodName = methodName;
        }

        public Type Type { get; }
        public string MethodName { get; }
    }
}