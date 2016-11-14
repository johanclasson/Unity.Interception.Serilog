using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Interception.Serilog.Customization;

namespace Unity.Interception.Serilog
{
    internal static class CustomAttributesExtensions
    {
        public static bool ContainsIgnoreAttribute(this IEnumerable<CustomAttributeData> attributes)
        {
            return attributes.Any(a => a.AttributeType == typeof (IgnoreMemberAttribute));
        }
    }
}