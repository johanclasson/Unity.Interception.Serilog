using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;
using Serilog.Events;

namespace Unity.Interception.Serilog
{
    // ReSharper disable once ClassNeverInstantiated.Global - Unity
    internal class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        private readonly ILogger _logger;
        private readonly IStopWatch _stopWatch;
        private readonly ISerilogOptions _options;

        public LoggingInterceptionBehavior(ILogger logger, IStopWatch stopWatch, ISerilogOptions options)
        {
            _logger = logger;
            _stopWatch = stopWatch;
            _options = options;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            _stopWatch.Start();
            var result = getNext()(input, getNext);
            _stopWatch.Stop();
            if (!IgnoreMethod(input))
                LogMethodResult(input, result);
            return result;
        }

        private void LogMethodResult(IMethodInvocation input, IMethodReturn result)
        {
            var builder = new TraceLogBuilder(input, result, _stopWatch.Elapsed, _logger);
            var message = builder.Build();
            var level = GetLevel(result);
            builder.Logger.Write(level, result.Exception, message, builder.PropertyValues);
        }

        private LogEventLevel GetLevel(IMethodReturn result)
        {
            if (result.Exception == null)
                return _options.Level;
            var expectedException = _options.ExpectedExceptions.Contains(result.Exception.GetType());
            return expectedException ? _options.Level : LogEventLevel.Error;
        }

        private bool IgnoreMethod(IMethodInvocation input)
        {
            var c = new MethodNameConverter(input);
            bool ignoredByMethodIdentifier = _options.IgnoredMethods.Any(m => m.Type.FullName == c.SourceContext && m.MethodName == c.EventId);
            return ignoredByMethodIdentifier || input.MethodBase.CustomAttributes.ContainsIgnoreAttribute();
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

        public bool WillExecute => true;
    }
}