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
            if (!IgnoreMethod(input))
            {
                LogMethodResult(input, result);
            }
            return result;
        }

        private void LogMethodResult(IMethodInvocation input, IMethodReturn result)
        {
            var builder = new MessageBuilder(input, result, _stopWatch.Elapsed);
            if (result.Exception != null)
            {
                _logger.Error(result.Exception, builder.Build(), builder.PropertyValues);
                return;
            }
            _logger.Information(builder.Build(), builder.PropertyValues);
        }

        private bool IgnoreMethod(IMethodInvocation input)
        {
            return input.MethodBase.CustomAttributes.ContainsIgnoreAttribute();
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

        public bool WillExecute => true;
    }
}