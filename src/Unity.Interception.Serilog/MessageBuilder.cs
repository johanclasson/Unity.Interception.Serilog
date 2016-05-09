using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;

namespace Unity.Interception.Serilog
{
    internal class MessageBuilder
    {
        private readonly IMethodInvocation _input;
        private readonly IMethodReturn _result;
        private readonly TimeSpan _duration;
        public ILogger Logger { get; private set; }
        public object[] PropertyValues => _propertyValues.ToArray();
        private readonly List<object> _propertyValues = new List<object>();

        public MessageBuilder(IMethodInvocation input, IMethodReturn result, TimeSpan duration, ILogger logger)
        {
            _input = input;
            _result = result;
            _duration = duration;
            Logger = logger;
        }

        public string Build()
        {
            var sb = AddMethod();
            AddArguments();
            AddResultAndDuration();
            AddExceptionType();
            return sb.ToString();
        }

        private void AddExceptionType()
        {
            if (_result.Exception == null)
                return;
            Logger = Logger.ForContext("ExceptionType", _result.Exception.GetType().FullName);
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
            Logger = Logger.ForContext("Result", _result.ReturnValue, destructureObjects: true);
        }

        private bool IsFailedResult => _result.Exception != null;

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