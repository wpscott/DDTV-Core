using System;
using System.Threading.Tasks;

namespace AcFunLiveServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("AcFun Live Server starting");
            if(args.Length == 2)
            {
                await Server.Start(args[0], args[1]);
            }
            else
            {
                await Server.Start();
            }
            Console.WriteLine("AcFun Live Server stopped");
        }
    }
}
