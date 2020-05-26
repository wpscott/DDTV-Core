using System;

namespace AcFunLiveServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AcFun Live Server starting");
            Server.Start().Wait();
            Console.WriteLine("AcFun Live Server stopped");
        }
    }
}
