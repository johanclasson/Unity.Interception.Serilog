using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Interception.Serilog
{
    internal class MethodNameConverter
    {
        private readonly IMethodInvocation _input;

        public MethodNameConverter(IMethodInvocation input)
        {
            _input = input;
        }

        public string EventId => _input?.MethodBase?.Name;

        public string SourceContext => _input?.MethodBase?.ReflectedType?.FullName;
    }
}