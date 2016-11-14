using System;
using Unity.Interception.Serilog.Customization;

namespace Unity.Interception.Serilog.Tests.Support
{
    public interface IDummy
    {
        // ReSharper disable once UnusedMethodReturnValue.Global - Logged through interception
        string ReturnStuff(int a, string b);

        void DoStuff();

        [IgnoreMember]
        void DoSecretStuff();

        // ReSharper disable UnusedParameter.Global - Logged through interception
        void DoStuffWithSecretParameter(string username, [IgnoreMember] string password);
        // ReSharper restore UnusedParameter.Global

        // ReSharper disable once UnusedMethodReturnValue.Global - Logged through interception
        int ThrowException();
    }

    public class Dummy : IDummy
    {
        public string ReturnStuff(int a, string b)
        {
            return $"{a} {b}";
        }

        public void DoStuff()
        {
        }

        public void DoSecretStuff()
        {
        }

        public void DoStuffWithSecretParameter(string username, string password)
        {
        }

        public int ThrowException()
        {
            throw new InvalidOperationException("Something bad happened");
        }
    }
}