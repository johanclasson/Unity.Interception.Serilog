using System;

namespace Unity.Interception.Serilog
{
    public interface IStopWatch
    {
        void Start();
        void Stop();
        TimeSpan Elapsed { get; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global - Unity
    internal class Stopwatch : System.Diagnostics.Stopwatch, IStopWatch
    {
    }
}