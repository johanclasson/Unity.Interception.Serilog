using System;

namespace Unity.Interception.Customization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}