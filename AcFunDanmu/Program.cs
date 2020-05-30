using System;

namespace AcFunDanmu
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 1)
            {
                var client = new Client(args[0]);

                client.Handler += HandleCommand; //Add your own handler

                client.Start().Wait();
            }
        }

        static void HandleCommand(DownstreamPayload stream)
        {
            switch (stream.Command)
            {
                // Includes comment, gift, enter room, like
                case "Push.ZtLiveInteractive.Message":
                    ZtLiveScMessage message = ZtLiveScMessage.Parser.ParseFrom(stream.PayloadData);

                    var payload = message.CompressionType == ZtLiveScMessage.Types.CompressionType.Gzip ? Client.Decompress(message.Payload) : message.Payload;

                    switch (message.MessageType)
                    {
                        case "ZtLiveScActionSignal":
                            var actionSignal = ZtLiveScActionSignal.Parser.ParseFrom(payload);

                            foreach (var item in actionSignal.Item)
                            {
                                foreach (var p in item.Payload)
                                {
                                    var pi = Client.Parse(item.SingalType, p);
#if DEBUG
                                    Console.WriteLine(pi);
#endif
                                }
                            }
                            break;
                        case "ZtLiveScStateSignal":
                            ZtLiveScStateSignal signal = ZtLiveScStateSignal.Parser.ParseFrom(payload);

                            foreach (var item in signal.Item)
                            {
                                var pi = Client.Parse(item.SingalType, item.Payload);
#if DEBUG
                                Console.WriteLine(pi);
#endif
                            }
                            break;
                        case "ZtLiveScStatusChanged":
                            var statusChanged = ZtLiveScStatusChanged.Parser.ParseFrom(payload);
                            Console.WriteLine(statusChanged);
                            break;
                        case "ZtLiveScTicketInvalid":
                            var ticketInvalid = ZtLiveScTicketInvalid.Parser.ParseFrom(payload);
                            break;
                        default:
                            Console.WriteLine("Unhandled message: {0}", message.MessageType);
                            Console.WriteLine(message);
                            break;
                    }
                    break;
            }
        }
    }
}
