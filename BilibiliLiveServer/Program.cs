using System;

namespace BilibiliLiveServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bilibili Live Server start");
            Server.Start().Wait();
            Console.WriteLine("Bilibili Live Server stop");
        }
    }
}
