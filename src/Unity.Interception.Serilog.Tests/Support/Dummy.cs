using Unity.Interception.Customization;

namespace Unity.Interception.Serilog.Tests.Support
{
    public interface IDummy
    {
        string ReturnStuff(int a, string b);

        void DoStuff();

        [IgnoreMember]
        void DoSecretStuff();

        void DoStuffWithSecretParameter(string username, [IgnoreMember] string password);
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
    }
}