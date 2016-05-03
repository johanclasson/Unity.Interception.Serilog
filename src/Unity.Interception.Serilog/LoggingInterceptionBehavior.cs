using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;

namespace Unity.Interception.Serilog
{
    class LoggingInterceptionBehavior : IInterceptionBehavior
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
            var message = "Method: {Method} called with arguments {@Arguments} returned {@Result} after {Duration}";
            var arguments = GetArguments(input).ToArray();
            _logger.Information(message, GetName(input), arguments, result.ReturnValue, _stopWatch.Elapsed);
            return result;
        }

        private IEnumerable<object> GetArguments(IMethodInvocation input)
        {
            var arguments = input.Arguments;
            for (int i = 0; i < arguments.Count; i++)
            {
                yield return new
                {
                    Name = arguments.ParameterName(i),
                    Value = arguments[i].ToString()
                };
            }
        }

        private static string GetName(IMethodInvocation input)
        {
            return $"{input?.MethodBase?.ReflectedType?.FullName}.{input?.MethodBase?.Name}";
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

        public bool WillExecute => true;
    }
}
