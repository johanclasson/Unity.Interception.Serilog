using System;

namespace Unity.Interception.Serilog
{
    public interface IStopWatch
    {
        void Start();
        void Stop();
        TimeSpan Elapsed { get; }
    }

    internal class Stopwatch : System.Diagnostics.Stopwatch, IStopWatch
    {
    }
}