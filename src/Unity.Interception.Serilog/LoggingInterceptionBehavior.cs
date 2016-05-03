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

        public LoggingInterceptionBehavior(ILogger logger)
        {
            _logger = logger;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var result = getNext()(input, getNext);
            var message = "Method: {Name} called with arguments {@Arguments} returned {@Result}";
            var arguments = GetArguments(input).ToArray();
            _logger.Information(message, GetName(input), arguments, result.ReturnValue);
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
