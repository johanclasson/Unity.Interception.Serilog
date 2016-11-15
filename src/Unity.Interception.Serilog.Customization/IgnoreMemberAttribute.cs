using System;

namespace Unity.Interception.Serilog.Customization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}