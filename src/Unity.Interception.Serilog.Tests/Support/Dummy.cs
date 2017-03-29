using System;
using System.Threading.Tasks;
using Unity.Interception.Serilog.Customization;

namespace Unity.Interception.Serilog.Tests.Support
{
    public interface IDummy
    {
        // ReSharper disable once UnusedMethodReturnValue.Global - Logged through interception
        string ReturnStuff(int a, string b);

        Task<string> ReturnStuffAsync(int a, string b);

        void DoStuff();
        Task DoStuffAsync();


        [IgnoreMember]
        void DoSecretStuff();

        // ReSharper disable UnusedParameter.Global - Logged through interception
        void DoStuffWithSecretParameter(string username, [IgnoreMember] string password);
        // ReSharper restore UnusedParameter.Global

        // ReSharper disable once UnusedMethodReturnValue.Global - Logged through interception
        int ThrowException();

        Task<int> ThrowExceptionWithResultAsync();
        Task ThrowExceptionWithoutResultAsync();
    }

    public class Dummy : IDummy
    {
        private readonly Task _asyncDelay = Task.Delay(200);

        public string ReturnStuff(int a, string b)
        {
            return $"{a} {b}";
        }

        public async Task<string> ReturnStuffAsync(int a, string b)
        {
            await _asyncDelay;
            return $"{a} {b}";
        }


        public void DoStuff()
        {
        }

        public async Task DoStuffAsync()
        {
            await _asyncDelay;
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

        public async Task<int> ThrowExceptionWithResultAsync()
        {
            await _asyncDelay;
            throw new InvalidOperationException("Something bad happened");
        }

        public async Task ThrowExceptionWithoutResultAsync()
        {
            await _asyncDelay;
            throw new InvalidOperationException("Something bad happened");
        }
    }
}