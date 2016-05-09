using System;
using System.Diagnostics;
using System.Threading;

namespace Unity.Interception.Serilog
{
    public interface ICommonProperties
    {
        int ManagedThreadId { get; }
        string MachineName { get; }
        int ProcessId { get; }
        string ProcessName { get; }
        string ThreadName { get; }
        string AppDomainName { get; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Unity
    class CommonProperties : ICommonProperties
    {
        public int ManagedThreadId => Thread.CurrentThread.ManagedThreadId;
        public string MachineName => Environment.MachineName;
        public int ProcessId => Process.GetCurrentProcess().Id;
        public string ProcessName => Process.GetCurrentProcess().ProcessName;
        public string ThreadName => Thread.CurrentThread.Name; //TODO: Check for null?
        public string AppDomainName => AppDomain.CurrentDomain.FriendlyName;
    }
}