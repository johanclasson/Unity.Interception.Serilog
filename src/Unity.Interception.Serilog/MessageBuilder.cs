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
            return sb.ToString();
        }

        public object[] Arguments => GetArguments().ToArray();

        private StringBuilder AddMethod()
        {
            var sb = new StringBuilder("Method {SourceContext:l}.{EventId:l}");
            _propertyValues.Add(SourceContext);
            _propertyValues.Add(EventId);
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

        private string EventId => _input?.MethodBase?.Name;

        private string SourceContext => _input?.MethodBase?.ReflectedType?.FullName;

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