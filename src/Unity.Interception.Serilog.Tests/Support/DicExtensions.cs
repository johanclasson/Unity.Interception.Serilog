using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity.Interception.Serilog.Tests.Support
{
    internal static class DicExtensions
    {
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dic);
        }
    }
}