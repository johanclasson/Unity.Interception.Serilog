using System;

namespace Unity.Interception.Serilog.Customization
{
    /// <summary>Used to identify a method or parameter to ignore to log trace- or error logs from.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}