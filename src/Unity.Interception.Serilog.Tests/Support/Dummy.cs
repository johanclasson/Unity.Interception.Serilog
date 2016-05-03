namespace Unity.Interception.Serilog.Tests.Support
{
    public interface IDummy
    {
        string ReturnStuff(int a, string b);

        void DoStuff();
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
    }
}