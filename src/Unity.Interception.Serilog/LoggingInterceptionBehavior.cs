using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;

namespace Unity.Interception.Serilog
{
    internal class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        private readonly ILogger _logger;
        private readonly IStopWatch _stopWatch;

        public LoggingInterceptionBehavior(ILogger logger, IStopWatch stopWatch)
        {
            _logger = logger;
            _stopWatch = stopWatch;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            _stopWatch.Start();
            var result = getNext()(input, getNext);
            _stopWatch.Stop();
            var builder = new MessageBuilder(input, result, _stopWatch.Elapsed);
            _logger.Information(builder.Build(), builder.PropertyValues);
            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

        public bool WillExecute => true;
    }
}