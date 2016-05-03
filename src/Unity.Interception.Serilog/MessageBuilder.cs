using System;
using System.Collections.Generic;
using System.Linq;
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
            var sb = new StringBuilder("Method: {Method}");
            _propertyValues.Add(MethodName);
            var arguments = GetArguments().ToArray();
            if (arguments.Length != 0)
            {
                sb.Append(" called with arguments {@Arguments}");
                _propertyValues.Add(arguments);
            }
            if (!_input.MethodBase.ToString().StartsWith("Void "))
            {
                sb.Append(" returned {@Result} after {Duration}");
                _propertyValues.Add(_result.ReturnValue);
            }
            else
            {
                sb.Append(" ran for {Duration}");
            }
            _propertyValues.Add(_duration);
            return sb.ToString();
        }

        private string MethodName => $"{_input?.MethodBase?.ReflectedType?.FullName}.{_input?.MethodBase?.Name}";

        private IEnumerable<object> GetArguments()
        {
            var arguments = _input.Arguments;
            for (int i = 0; i < arguments.Count; i++)
            {
                yield return new
                {
                    Name = arguments.ParameterName(i),
                    Value = arguments[i].ToString()
                };
            }
        }
    }
}