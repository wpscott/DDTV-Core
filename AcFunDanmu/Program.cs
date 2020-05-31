using System;

namespace AcFunDanmu
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var client = new Client(args[0]);

                client.Handler += HandleCommand; //Add your own handler

                client.Start().Wait();
            }
        }

        static void HandleCommand(DownstreamPayload stream)
        {
            if (stream == null) { return; }
            switch (stream.Command)
            {
                // Includes comment, gift, enter room, like, follower
                case "Push.ZtLiveInteractive.Message":
                    ZtLiveScMessage message = ZtLiveScMessage.Parser.ParseFrom(stream.PayloadData);

                    var payload = message.CompressionType == ZtLiveScMessage.Types.CompressionType.Gzip ? Client.Decompress(message.Payload) : message.Payload;

                    switch (message.MessageType)
                    {
                        case "ZtLiveScActionSignal":
                            var actionSignal = ZtLiveScActionSignal.Parser.ParseFrom(payload);

                            foreach (var item in actionSignal.Item)
                            {
                                switch (item.SingalType)
                                {
                                    case "CommonActionSignalComment":
                                        foreach (var pl in item.Payload)
                                        {
                                            var comment = CommonActionSignalComment.Parser.ParseFrom(pl);
                                            Console.WriteLine("{0} - {1}({2}): {3}", comment.SendTimeMs, comment.UserInfo.Nickname, comment.UserInfo.UserId, comment.Content);
                                        }
                                        break;
                                    case "CommonActionSignalLike":
                                        foreach (var pl in item.Payload)
                                        {
                                            var like = CommonActionSignalLike.Parser.ParseFrom(pl);
                                            Console.WriteLine("{0} - {1}({2}) liked", like.SendTimeMs, like.UserInfo.Nickname, like.UserInfo.UserId);
                                        }
                                        break;
                                    case "CommonActionSignalUserEnterRoom":
                                        foreach (var pl in item.Payload)
                                        {
                                            var enter = CommonActionSignalUserEnterRoom.Parser.ParseFrom(pl);
                                            Console.WriteLine("{0} - {1}({2}) entered", enter.SendTimeMs, enter.UserInfo.Nickname, enter.UserInfo.UserId);
                                        }
                                        break;
                                    case "CommonActionSignalUserFollowAuthor":
                                        foreach (var pl in item.Payload)
                                        {
                                            var follower = CommonActionSignalUserFollowAuthor.Parser.ParseFrom(pl);
                                            Console.WriteLine("{0} - {1}({2} entered", follower.SendTimeMs, follower.UserInfo.Nickname, follower.UserInfo.UserId);
                                        }
                                        break;
                                    case "CommonNotifySignalKickedOut":
                                    case "CommonNotifySignalViolationAlert":
                                        break;
                                    case "AcfunActionSignalThrowBanana":
                                        foreach (var pl in item.Payload)
                                        {
                                            var enter = AcfunActionSignalThrowBanana.Parser.ParseFrom(pl);
                                            Console.WriteLine("{0} - {1}({2}) throwed {3} banana(s)", enter.SendTimeMs, enter.Visitor.Name, enter.Visitor.UserId, enter.Count);
                                        }
                                        break;
                                    case "CommonActionSignalGift":
                                        foreach (var pl in item.Payload)
                                        {
                                            /*
                                             * Item Id
                                             * 1 - 香蕉
                                             * 2 - 吃瓜
                                             * 6 - 魔法棒
                                             * 15 - AC机娘
                                             * 
                                             */
                                            var gift = CommonActionSignalGift.Parser.ParseFrom(pl);
                                            Console.WriteLine("{0} - {1}({2}) sent gift", gift.SendTimeMs, gift.User.Name, gift.User.UserId);
#if DEBUG
                                            Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", gift.ItemId, gift.Count, gift.Combo, gift.Value, gift.Uuid, gift.Number5, gift.Number6);
#endif
                                        }
                                        break;
                                    default:
                                        foreach (var p in item.Payload)
                                        {
                                            var pi = Client.Parse(item.SingalType, p);
#if DEBUG
                                            Console.WriteLine("Action type: {0}, content: {1}", item.SingalType, pi);
#endif
                                        }
                                        break;
                                }
                            }
                            break;
                        case "ZtLiveScStateSignal":
                            ZtLiveScStateSignal signal = ZtLiveScStateSignal.Parser.ParseFrom(payload);

                            foreach (var item in signal.Item)
                            {
                                switch (item.SingalType)
                                {
                                    case "AcfunStateSignalDisplayInfo":
                                        var acInfo = AcfunStateSignalDisplayInfo.Parser.ParseFrom(item.Payload);
                                        //Console.WriteLine("Current banada count: {0}", acInfo.BananaCount);
                                        break;
                                    case "CommonStateSignalDisplayInfo":
                                        var stateInfo = CommonStateSignalDisplayInfo.Parser.ParseFrom(item.Payload);
                                        //Console.WriteLine("{0} watching, {1} likes", stateInfo.WatchingCount, stateInfo.LikeCount);
                                        break;
                                    case "CommonStateSignalTopUsers":
                                        var users = CommonStateSignalTopUsers.Parser.ParseFrom(item.Payload);
                                        //Console.WriteLine("Top 3 users: {0}", string.Join(", ", users.User.Select(user => user.Detail.Name)));
                                        break;
                                    case "CommonStateSignalRecentComment":
                                        var comments = CommonStateSignalRecentComment.Parser.ParseFrom(item.Payload);
                                        foreach (var comment in comments.Comment)
                                        {
                                            Console.WriteLine("{0} - {1}({2}): {3}", comment.SendTimeMs, comment.UserInfo.Nickname, comment.UserInfo.UserId, comment.Content);
                                        }
                                        break;
                                    default:
                                        var pi = Client.Parse(item.SingalType, item.Payload);
#if DEBUG
                                        Console.WriteLine("State type: {0}, content: {1}", item.SingalType, pi);
#endif
                                        break;
                                }
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
