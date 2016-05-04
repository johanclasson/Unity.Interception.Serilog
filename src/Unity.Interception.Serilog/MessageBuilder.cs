using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Interception.Serilog
{
    internal class MessageBuilder
    {
        private readonly IMethodInvocation _input;
        private readonly IMethodReturn _result;
        private readonly TimeSpan _duration;
        public object[] PropertyValues => _propertyValues.ToArray();
        private readonly List<object> _propertyValues = new List<object>();

        public MessageBuilder(IMethodInvocation input, IMethodReturn result, TimeSpan duration)
        {
            _input = input;
            _result = result;
            _duration = duration;
        }

        public string Build()
        {
            var sb = AddMethod();
            AddArguments(sb);
            AddResultAndDuration(sb);
            return sb.ToString();
        }

        private StringBuilder AddMethod()
        {
            var sb = new StringBuilder("Method {Method}");
            _propertyValues.Add(MethodName);
            if (IsFailedResult)
                sb.Append(" failed");
            return sb;
        }

        private void AddArguments(StringBuilder sb)
        {
            object[] arguments = GetArguments().ToArray();
            if (arguments.Length != 0)
            {
                sb.Append(" called with arguments {@Arguments}");
                _propertyValues.Add(arguments);
            }
        }

        private void AddResultAndDuration(StringBuilder sb)
        {
            if (IsFailedResult)
            {
                sb.Append(" after {Duration}");
                _propertyValues.Add(_duration);
                return;
            }
            if (_input.MethodBase.ToString().StartsWith("Void "))
            {
                sb.Append(" ran for {Duration}");
                _propertyValues.Add(_duration);
                return;
            }
            sb.Append(" returned {@Result} after {Duration}");
            _propertyValues.Add(_result.ReturnValue);
            _propertyValues.Add(_duration);
        }

        private bool IsFailedResult => _result.Exception != null;

        private string MethodName => $"{_input?.MethodBase?.ReflectedType?.FullName}.{_input?.MethodBase?.Name}";

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