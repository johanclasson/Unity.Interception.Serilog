using System;
using Serilog;

namespace Unity.Interception.Serilog
{
    public static class LoggerExtensions
    {
        public static void Dispose(this ILogger logger)
        {
            var disposable = logger as IDisposable;
            disposable?.Dispose();
        }
    }
}