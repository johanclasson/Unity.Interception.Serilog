namespace Unity.Interception.Serilog.Tests.Support
{
    public interface IDummy
    {
        string DoStuff(int a, string b);
    }

    public class Dummy : IDummy
    {
        public string DoStuff(int a, string b)
        {
            return $"{a} {b}";
        }
    }
}