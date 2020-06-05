﻿using AcFunDanmu;
using AcFunDanmu.Enums;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AcFunDanmuConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // Support Emoji

            var retry = 0;

            Client client;

            if (args.Length == 1)
            {
                client = new Client(args[0]);  // Visitor/Anonymous mode
            }
            else if (args.Length == 3)
            {
                client = new Client(args[0], args[1], args[2]); // User mode
            }
            else
            {
                return;
            }

            client.Handler = HandleSignal; // Use your own signal handler

            var resetTimer = new System.Timers.Timer(5000);
            resetTimer.Elapsed += (s, e) =>
            {
                retry = 0;
            };

            while (!await client.Start() && retry < 5)
            {
                if (retry > 0)
                {
                    resetTimer.Stop();
                }
                Console.WriteLine("Client closed, retrying");
                retry++;
                resetTimer.Start();
            }

            //DecodeHar(@".\your.own.har");
            //await LoginToGetGiftList();
        }

        static void HandleSignal(string messagetType, byte[] payload)
        {
            switch (messagetType)
            {
                // Includes comment, gift, enter room, like, follower
                case PushMessage.ACTION_SIGNAL:
                    var actionSignal = ZtLiveScActionSignal.Parser.ParseFrom(payload);

                    foreach (var item in actionSignal.Item)
                    {
                        switch (item.SingalType)
                        {
                            case PushMessage.ActionSignal.COMMENT:
                                foreach (var pl in item.Payload)
                                {
                                    var comment = CommonActionSignalComment.Parser.ParseFrom(pl);
                                    Console.WriteLine("{0} - {1}({2}): {3}", comment.SendTimeMs, comment.UserInfo.Nickname, comment.UserInfo.UserId, comment.Content);
                                }
                                break;
                            case PushMessage.ActionSignal.LIKE:
                                foreach (var pl in item.Payload)
                                {
                                    var like = CommonActionSignalLike.Parser.ParseFrom(pl);
                                    Console.WriteLine("{0} - {1}({2}) liked", like.SendTimeMs, like.UserInfo.Nickname, like.UserInfo.UserId);
                                }
                                break;
                            case PushMessage.ActionSignal.ENTER_ROOM:
                                foreach (var pl in item.Payload)
                                {
                                    var enter = CommonActionSignalUserEnterRoom.Parser.ParseFrom(pl);
                                    Console.WriteLine("{0} - {1}({2}) entered", enter.SendTimeMs, enter.UserInfo.Nickname, enter.UserInfo.UserId);
                                }
                                break;
                            case PushMessage.ActionSignal.FOLLOW:
                                foreach (var pl in item.Payload)
                                {
                                    var follower = CommonActionSignalUserFollowAuthor.Parser.ParseFrom(pl);
                                    Console.WriteLine("{0} - {1}({2} followed", follower.SendTimeMs, follower.UserInfo.Nickname, follower.UserInfo.UserId);
                                }
                                break;
                            case PushMessage.ActionSignal.KICKED_OUT:
                            case PushMessage.ActionSignal.VIOLATION_ALERT:
                                break;
                            case PushMessage.ActionSignal.THROW_BANANA:
                                foreach (var pl in item.Payload)
                                {
                                    var enter = AcfunActionSignalThrowBanana.Parser.ParseFrom(pl);
                                    Console.WriteLine("{0} - {1}({2}) throwed {3} banana(s)", enter.SendTimeMs, enter.Visitor.Name, enter.Visitor.UserId, enter.Count);
                                }
                                break;
                            case PushMessage.ActionSignal.GIFT:
                                foreach (var pl in item.Payload)
                                {
                                    /*
                                     * Item Id
                                     * 1 - 香蕉
                                     * 2 - 吃瓜
                                     * 3 - 
                                     * 4 - 牛啤
                                     * 5 - 手柄
                                     * 6 - 魔法棒
                                     * 7 - 好人卡
                                     * 8 - 星蕉雨
                                     * 9 - 告白
                                     * 10 - 666
                                     * 11 - 菜鸡
                                     * 12 - 打Call
                                     * 13 - 立FLAG
                                     * 14 - 窜天猴
                                     * 15 - AC机娘
                                     * 16 - 猴岛
                                     * 17 - 快乐水
                                     * 18 - 
                                     * 19 - 
                                     * 20 - 
                                     * 21 - 生日快乐
                                     * 22 - 六一快乐
                                     */
                                    var gift = CommonActionSignalGift.Parser.ParseFrom(pl);
                                    var giftName = Client.Gifts[gift.ItemId];
                                    Console.WriteLine("{0} - {1}({2}) sent gift {3} × {4}, Combo: {5}, value: {6}", gift.SendTimeMs, gift.User.Name, gift.User.UserId, giftName, gift.Count, gift.Combo, gift.Value);
#if DEBUG
                                    if (string.IsNullOrEmpty(giftName))
                                    {
                                        Console.WriteLine("ItemId: {0}, Value: {1}", gift.ItemId, gift.Value);
                                    }
#endif
                                }
                                break;
                            default:
                                foreach (var p in item.Payload)
                                {
                                    var pi = Client.Parse(item.SingalType, p);
#if DEBUG
                                    Console.WriteLine("Unhandled action type: {0}, content: {1}", item.SingalType, pi);
#endif
                                }
                                break;
                        }
                    }
                    break;
                // Includes current banana counts, watching count, like count and top 3 users sent gifts
                case PushMessage.STATE_SIGNAL:
                    ZtLiveScStateSignal signal = ZtLiveScStateSignal.Parser.ParseFrom(payload);

                    foreach (var item in signal.Item)
                    {
                        switch (item.SingalType)
                        {
                            case PushMessage.StateSignal.ACFUN_DISPLAY_INFO:
                                var acInfo = AcfunStateSignalDisplayInfo.Parser.ParseFrom(item.Payload);
                                //Console.WriteLine("Current banada count: {0}", acInfo.BananaCount);
                                break;
                            case PushMessage.StateSignal.DISPLAY_INFO:
                                var stateInfo = CommonStateSignalDisplayInfo.Parser.ParseFrom(item.Payload);
                                //Console.WriteLine("{0} watching, {1} likes", stateInfo.WatchingCount, stateInfo.LikeCount);
                                break;
                            case PushMessage.StateSignal.TOP_USRES:
                                var users = CommonStateSignalTopUsers.Parser.ParseFrom(item.Payload);
                                //Console.WriteLine("Top 3 users: {0}", string.Join(", ", users.User.Select(user => user.Detail.Name)));
                                break;
                            case PushMessage.StateSignal.RECENT_COMMENT:
                                var comments = CommonStateSignalRecentComment.Parser.ParseFrom(item.Payload);
                                foreach (var comment in comments.Comment)
                                {
                                    Console.WriteLine("{0} - {1}({2}): {3}", comment.SendTimeMs, comment.UserInfo.Nickname, comment.UserInfo.UserId, comment.Content);
                                }
                                break;
                            default:
                                var pi = Client.Parse(item.SingalType, item.Payload);
#if DEBUG
                                Console.WriteLine("Unhandled state type: {0}, content: {1}", item.SingalType, pi);
#endif
                                break;
                        }
                    }
                    break;
            }
        }

        #region Others
        static void DumpCookie(CookieContainer cookiesContainer)
        {
            Hashtable hs = (Hashtable)cookiesContainer.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cookiesContainer, new object[] { });
            foreach (string key in hs.Keys)
            {
                var uri = new Uri($"http://{key.Substring(1)}/");
                var collection = cookiesContainer.GetCookies(uri);
                foreach (Cookie ck in collection)
                {
                    Console.WriteLine($"{ck.Name} - {ck.Value}");
                }
            }
        }
        class Message
        {
            public string type { get; set; }
            public string data { get; set; }
        }
        static void DecodeHar(string filePath)
        {
            var client = new Client(
                1000000040695861,
                "ChRhY2Z1bi5hcGkudmlzaXRvci5zdBJwtK7G9ZWM4z4HKc4lbhpGECjvz6aOCfXQ36xgrn8wRFvS_L30QdkR2wQ9pRfxcj6GRn3Ymv2UwSrJpfzIDny8yDcqoDTdxFahdqhV58kJFnAUAQy8bZNgZksalDUyBi4Z9FTKRNKmxK5Xyz9typfPwhoS7u141z2YqvpCVbrK-LpEG0LFIiALNuXXoUE54trRZ9ZsQ-vcUwYRRvGOdTyZ4-mFNzz-bigFMAE",
                "emX88MTKZHee/PwU8yYLMw==",
                new string[] { "CgwxMC40NC4yMjIuMjMQkk4=", "CgwxMC40NC40MC4xODEQkk4=", "CgwxMC40NC4yMjIuMjMQk04=", "Cg0xMC41Mi4xODkuMTczEJNO" },
                "nXc9rLOlKxcYhjIIURG6ZpXGOeCGewCG8aeVAQrHzMi7a/Pw9njsOuwH49nQwWf4HmSyLmC6yWz/cagN4tcSbg==",
                 "y9Pi8EC4ulk",
                 "JV3kqr1nefizjwACBCJ0DA=="
                );

            using var file = new StreamReader(filePath);

            using var json = JsonDocument.Parse(file.ReadToEnd());

            var ws = json.RootElement.GetProperty("log").GetProperty("entries").EnumerateArray().First(item => item.GetProperty("request").GetProperty("url").ToString() == "wss://link.xiatou.com/");

            Message[] messages = JsonSerializer.Deserialize<Message[]>(ws.GetProperty("_webSocketMessages").ToString());

            using var writer = new StreamWriter(@".\output.txt");

            foreach (var message in messages)
            {
                if (message.type == "send")
                {
                    var us = client.DecodeUpstream(Convert.FromBase64String(message.data));
                    writer.WriteLine("Up\tSeqId {0}, Command: {1}", us.SeqId, us.Command);
                    //writer.Write("\t\t");
                    switch (us.Command)
                    {
                        case Command.REGISTER:
                            var register = RegisterRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(register);
                            break;
                        case Command.KEEP_ALIVE:
                            var keepalive = KeepAliveRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(keepalive);
                            break;
                        case Command.PING:
                            var ping = PingRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(ping);
                            break;
                        case Command.MESSAGE_SESSION:
                            var session = AcFunDanmu.Im.Message.Types.SessionListRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(session);
                            break;
                        case Command.MESSAGE_PULL_OLD:
                            var pullold = AcFunDanmu.Im.Message.Types.PullOldRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(pullold);
                            break;
                        case Command.CLIENT_CONFIG_GET:
                            var configget = AcFunDanmu.Im.Cloud.Types.Config.Types.ClientConfigGetRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(configget);
                            break;
                        case Command.GROUP_USER_GROUP_LIST:
                            var usergrouplist = AcFunDanmu.Im.Cloud.Types.Message.Types.UserGroupListRequest.Parser.ParseFrom(us.PayloadData);
                            writer.WriteLine(usergrouplist);
                            break;
                        case Command.GLOBAL_COMMAND:
                            ZtLiveCsCmd cmd = ZtLiveCsCmd.Parser.ParseFrom(us.PayloadData);
                            switch (cmd.CmdType)
                            {
                                case GlobalCommand.ENTER_ROOM:
                                    var enterRoom = ZtLiveCsEnterRoom.Parser.ParseFrom(cmd.Payload);
                                    break;
                                case GlobalCommand.HEARTBEAT:
                                    var heartbeat = ZtLiveCsHeartbeat.Parser.ParseFrom(cmd.Payload);
                                    break;
                                default:
                                    Console.WriteLine("Unhandled Global.ZtLiveInteractive.CsCmd: {0}", cmd.CmdType);
                                    Console.WriteLine(cmd);
                                    break;
                            }
                            break;
                        case Command.PUSH_MESSAGE:
                            Console.WriteLine(us.PayloadData.ToBase64());
                            break;
                        default:
                            writer.WriteLine(us);
                            break;
                    }
                    writer.WriteLine("--------------------------------");
                }
                else if (message.type == "receive")
                {
                    var ds = client.Decode(Convert.FromBase64String(message.data));
                    writer.WriteLine("Down\tSeqId {0}, Command: {1}", ds.SeqId, ds.Command);
                    //writer.Write("\t\t");
                    switch (ds.Command)
                    {
                        case Command.REGISTER:
                            var register = RegisterResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(register);
                            break;
                        case Command.KEEP_ALIVE:
                            var keepalive = KeepAliveResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(keepalive);
                            break;
                        case Command.PING:
                            var ping = PingResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(ping);
                            break;
                        case Command.MESSAGE_SESSION:
                            var session = AcFunDanmu.Im.Message.Types.SessionListResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(session);
                            break;
                        case Command.MESSAGE_PULL_OLD:
                            var pullold = AcFunDanmu.Im.Message.Types.PullOldResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(pullold);
                            break;
                        case Command.CLIENT_CONFIG_GET:
                            var configget = AcFunDanmu.Im.Cloud.Types.Config.Types.ClientConfigGetResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(configget);
                            break;
                        case Command.GROUP_USER_GROUP_LIST:
                            var usergrouplist = AcFunDanmu.Im.Cloud.Types.Message.Types.UserGroupListResponse.Parser.ParseFrom(ds.PayloadData);
                            writer.WriteLine(usergrouplist);
                            break;
                        case Command.GLOBAL_COMMAND:
                            ZtLiveCsCmdAck cmd = ZtLiveCsCmdAck.Parser.ParseFrom(ds.PayloadData);
                            switch (cmd.CmdAckType)
                            {
                                case GlobalCommand.ENTER_ROOM_ACK:
                                    var enterRoom = ZtLiveCsEnterRoomAck.Parser.ParseFrom(cmd.Payload);
                                    break;
                                case GlobalCommand.HEARTBEAT_ACK:
                                    var heartbeat = ZtLiveCsHeartbeatAck.Parser.ParseFrom(cmd.Payload);
                                    break;
                                default:
                                    Console.WriteLine("Unhandled Global.ZtLiveInteractive.CsCmd: {0}", cmd.CmdAckType);
                                    Console.WriteLine(cmd);
                                    break;
                            }
                            break;
                        case Command.PUSH_MESSAGE:
                            ZtLiveScMessage scmessage = ZtLiveScMessage.Parser.ParseFrom(ds.PayloadData);

                            var payload = scmessage.CompressionType == ZtLiveScMessage.Types.CompressionType.Gzip ? Client.Decompress(scmessage.Payload) : scmessage.Payload;

                            switch (scmessage.MessageType)
                            {
                                case PushMessage.ACTION_SIGNAL:
                                    // Handled by user
                                    HandleSignal(scmessage.MessageType, payload.ToByteArray());
                                    break;
                                case PushMessage.STATE_SIGNAL:
                                    // Handled by user
                                    HandleSignal(scmessage.MessageType, payload.ToByteArray());
                                    break;
                                case PushMessage.STATUS_CHANGED:
                                    var statusChanged = ZtLiveScStatusChanged.Parser.ParseFrom(payload);
                                    break;
                                case PushMessage.TICKET_INVALID:
                                    var ticketInvalid = ZtLiveScTicketInvalid.Parser.ParseFrom(payload);
                                    break;
                                default:
                                    Console.WriteLine("Unhandled Push.ZtLiveInteractive.Message: {0}", scmessage.MessageType);
                                    break;
                            }
                            break;
                        default:
                            writer.WriteLine(ds);
                            break;
                    }
                    writer.WriteLine("--------------------------------");
                }
            }
        }
        #endregion
    }
}
