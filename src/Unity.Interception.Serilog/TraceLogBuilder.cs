using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;

namespace Unity.Interception.Serilog
{
    internal class TraceLogBuilder
    {
        private readonly IMethodInvocation _input;
        private readonly object _value;
        private readonly Exception _exception;
        private readonly TimeSpan _duration;
        public ILogger Logger { get; private set; }
        public object[] PropertyValues => _propertyValues.ToArray();
        private readonly List<object> _propertyValues = new List<object>();

        public TraceLogBuilder(IMethodInvocation input, object value, Exception exception, TimeSpan duration, ILogger logger)
        {
            _input = input;
            _value = value;
            _exception = exception;
            _duration = duration;
            Logger = logger;
        }

        public string Build()
        {
            var sb = AddMethod();
            AddArguments();
            AddResultAndDuration();
            AddExceptionType();
            AddLogType();
            return sb.ToString();
        }

        private void AddLogType()
        {
            Logger = Logger.ForContext("LogType", "Trace");
        }

        private void AddExceptionType()
        {
            if (_exception == null)
                return;
            Logger = Logger.ForContext("ExceptionType", _exception.GetType().FullName);
        }

        private StringBuilder AddMethod()
        {
            var sb = new StringBuilder("Method {EventId}");
            var c = new MethodNameConverter(_input);
            _propertyValues.Add(c.EventId);
            Logger = Logger.ForContext("SourceContext", c.SourceContext);
            sb.Append(IsFailedResult ? " failed" : " returned");
            return sb;
        }

        private void AddArguments()
        {
            object[] arguments = GetArguments().ToArray();
            if (arguments.Length == 0)
                return;
            Logger = Logger.ForContext("Arguments", arguments, destructureObjects: true);
        }

        private void AddResultAndDuration()
        {
            Logger = Logger.ForContext("Duration", _duration.TotalMilliseconds);
            if (IsFailedResult)
                return;
            if (_input.MethodBase.ToString().StartsWith("Void "))
                return;
            Logger = Logger.ForContext("Result", _value);
        }

        private bool IsFailedResult => _exception != null;

        private IEnumerable<object> GetArguments()
        {
            IParameterCollection arguments = _input.Arguments;
            for (int i = 0; i < arguments.Count; i++)
            {
                string value = IgnoreArgumentValue(arguments.GetParameterInfo(i)) ? "[hidden]" : arguments[i].ToString();
                yield return new
                {
                    Name = arguments.ParameterName(i),
                    Value = value
                };
            }
        }

        private bool IgnoreArgumentValue(ParameterInfo info)
        {
            return info.CustomAttributes.ContainsIgnoreAttribute();
        }
    }
}