namespace AcFunDanmu
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.Initialize(args[0]).Wait();
            client.Start().Wait();
        }
    }
}
