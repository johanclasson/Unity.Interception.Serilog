using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Interception.Serilog
{
    class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;

        public bool WillExecute => true;
    }
}
