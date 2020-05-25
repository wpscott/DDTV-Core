using System;

namespace AcFunLiveServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AcFun Live Server start");
            Server.Start().Wait();
            Console.WriteLine("AcFun Live Server stop");
        }
    }
}
