using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AcFunDanmu
{
    public delegate void SignalHandler(string messageType, byte[] payload);
    public class Client
    {
        #region Constants
        public static readonly string[] Gifts = new string[] {
            "?",
            "香蕉",
            "吃瓜",
            "?",
            "牛啤",
            "手柄",
            "手柄",
            "好人卡",
            "星蕉雨",
            "告白",
            "666",
            "菜鸡",
            "打Call",
            "立FLAG",
            "窜天猴",
            "AC机娘",
            "猴岛",
            "快乐水",
        };

        private const string _ACFUN_HOST = "https://live.acfun.cn";
        private static readonly Uri ACFUN_HOST = new Uri(_ACFUN_HOST);
        private const string LIVE_URL = "https://live.acfun.cn/live";
        private const string LOGIN_URL = "https://id.app.acfun.cn/rest/app/visitor/login";
        private static readonly Uri LOGIN_URI = new Uri(LOGIN_URL);
        private const string GET_TOKEN_URL = "https://id.app.acfun.cn/rest/web/token/get";
        private static readonly Uri GET_TOKEN_URI = new Uri(GET_TOKEN_URL);
        private const string PLAY_URL = "https://api.kuaishouzt.com/rest/zt/live/web/startPlay?subBiz=mainApp&kpn=ACFUN_APP&kpf=PC_WEB&userId={0}&did={1}&acfun.api.visitor_st={2}";
        private const string GIFT_URL = "http://api.kuaishouzt.com/rest/zt/live/web/gift/list?subBiz=mainApp&kpn=ACFUN_APP&kpf=PC_WEB&userId={0}&did={1}&acfun.midground.api_st={2}";

        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36";

        private static readonly Dictionary<string, string> LOGIN_FORM = new Dictionary<string, string> { { "sid", "acfun.api.visitor" } };
        private static readonly Dictionary<string, string> GET_TOKEN_FORM = new Dictionary<string, string> { { "sid", "acfun.midground.api" } };

        private const string _Host = "wss://link.xiatou.com/";
        private static readonly Uri Host = new Uri(_Host);
        private const int Offset = 12;
        private const int BufferSize = 1 << 16;
        private const int AppId = 13;
        private const string AppName = "link-sdk";
        private const string SdkVersion = "1.2.1";
        private const string KPN = "ACFUN_APP";
        private const string KPF = "PC_WEB";
        private const string SubBiz = "mainApp";
        private const string ClientLiveSdkVersion = "kwai-acfun-live-link";

        private const int PushInterval = 1000;
        #endregion

        public SignalHandler Handler { get; set; }

        #region Properties and Fields
        private long UserId = -1;
        private string ServiceToken;
        private string SecurityKey;
        private string LiveId;
        private string EnterRoomAttach;
        private string[] Tickets;

        private long InstanceId = 0;
        private string SessionKey;
        private long HeaartbeatInterval;
        private long Lz4CompressionThreshold;

        private long SeqId = 1;
        private long HeaderSeqId = 1;
        private long HeartbeatSeqId = 1;
        private uint RetryCount = 1;
        private int TicketIndex = 0;

        private bool PrintHeader = false;
        #endregion

        #region Constructor
        public Client()
        {

        }

        public Client(string uid) : this()
        {
            Initialize(uid).Wait();
        }

        public Client(long userId, string serviceToken, string securityKey, string[] tickets, string enterRoomAttach, string liveId) : this()
        {
            UserId = userId;
            ServiceToken = serviceToken;
            SecurityKey = securityKey;
            Tickets = tickets;
            EnterRoomAttach = enterRoomAttach;
            LiveId = liveId;
        }

        public Client(long userId, string serviceToken, string securityKey, string[] tickets, string enterRoomAttach, string liveId, string sessionKey) : this(userId, serviceToken, securityKey, tickets, enterRoomAttach, liveId)
        {
            SessionKey = sessionKey;
            PrintHeader = true;
        }
        #endregion

        private async Task Initialize(string uid)
        {
            Console.WriteLine("Client initializing");
            var Cookies = new CookieContainer();

            var loaded = LoadCookies(Cookies);

            var client = new HttpClient(
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.All,
                    UseCookies = true,
                    CookieContainer = Cookies
                }
            );
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");

            using var index = await client.GetAsync($"{LIVE_URL}/{uid}");
            if (!index.IsSuccessStatusCode)
            {
                Console.WriteLine(await index.Content.ReadAsStringAsync());
                return;
            }
            var deviceId = Cookies.GetCookies(ACFUN_HOST).Where(cookie => cookie.Name == "_did").First().Value;

            if (loaded)
            {
                using var gettokenContent = new FormUrlEncodedContent(GET_TOKEN_FORM);
                using var gettoken = await client.PostAsync(GET_TOKEN_URI, gettokenContent);
                if (!gettoken.IsSuccessStatusCode)
                {
                    Console.WriteLine(await gettoken.Content.ReadAsStringAsync());
                    return;
                }
                using var tokenData = await JsonDocument.ParseAsync(await gettoken.Content.ReadAsStreamAsync());

                UserId = tokenData.RootElement.GetProperty("userId").GetInt64();
                ServiceToken = tokenData.RootElement.GetProperty("acfun.api.visitor_st").ToString();
                SecurityKey = tokenData.RootElement.GetProperty("acSecurity").ToString();
            }
            else
            {
                using var loginContent = new FormUrlEncodedContent(LOGIN_FORM);
                using var login = await client.PostAsync(LOGIN_URI, loginContent);
                if (!login.IsSuccessStatusCode)
                {
                    Console.WriteLine(await login.Content.ReadAsStringAsync());
                    return;
                }
                using var loginData = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());

                UserId = loginData.RootElement.GetProperty("userId").GetInt64();
                ServiceToken = loginData.RootElement.GetProperty("acfun.api.visitor_st").ToString();
                SecurityKey = loginData.RootElement.GetProperty("acSecurity").ToString();
            }

            using var form = new FormUrlEncodedContent(new Dictionary<string, string> { { "authorId", uid } });
            using var play = await client.PostAsync(string.Format(PLAY_URL, UserId, deviceId, ServiceToken), form);

            if (!play.IsSuccessStatusCode)
            {
                Console.WriteLine(await play.Content.ReadAsStringAsync());
                return;
            }

            using var playData = await JsonDocument.ParseAsync(await play.Content.ReadAsStreamAsync());
            if (playData.RootElement.GetProperty("result").GetInt32() != 1)
            {
                Console.WriteLine(playData.RootElement.GetProperty("error_msg").GetString());
                return;
            }
            Tickets = playData.RootElement.GetProperty("data").GetProperty("availableTickets").EnumerateArray().Select(ticket => ticket.ToString()).ToArray();
            EnterRoomAttach = playData.RootElement.GetProperty("data").GetProperty("enterRoomAttach").ToString();
            LiveId = playData.RootElement.GetProperty("data").GetProperty("liveId").ToString();

            Console.WriteLine("Client initialized");
        }

        public async Task Start()
        {
            if (UserId == -1 || string.IsNullOrEmpty(ServiceToken) || string.IsNullOrEmpty(SecurityKey) || string.IsNullOrEmpty(LiveId) || string.IsNullOrEmpty(EnterRoomAttach) || Tickets == null)
            {
                Console.WriteLine("Not initialized or live is ended");
                return;
            }
            using var client = new ClientWebSocket();

            await client.ConnectAsync(Host, default);
            if (client.State == WebSocketState.Open)
            {
                #region Register & Enter Room
                //Register
                await client.SendAsync(Register(), WebSocketMessageType.Binary, true, default);
                var resp = new byte[BufferSize];
                await client.ReceiveAsync(resp, default);
                var registerDown = Decode(resp);
                var regResp = RegisterResponse.Parser.ParseFrom(registerDown.PayloadData);
                InstanceId = regResp.InstanceId;
                SessionKey = regResp.SessKey.ToBase64();
                Lz4CompressionThreshold = regResp.SdkOption.Lz4CompressionThresholdBytes;

                //Ping
                //await client.SendAsync(Ping(), WebSocketMessageType.Binary, true, default);

                //Keep Alive
                await client.SendAsync(KeepAlive(), WebSocketMessageType.Binary, true, default);
                SeqId++;
                HeaderSeqId++;

                //Enter room
                await client.SendAsync(EnterRoom(), WebSocketMessageType.Binary, true, default);
                #endregion

                #region Timers
                //Push message
                using var heartbeatTimer = new System.Timers.Timer();
                heartbeatTimer.Elapsed += async (s, e) =>
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            await client.SendAsync(
                                Heartbeat(),
                                WebSocketMessageType.Binary,
                                true,
                                default
                            );

                            await client.SendAsync(KeepAlive(), WebSocketMessageType.Binary, true, default);
                        }
                        catch (WebSocketException ex)
                        {
                            Console.WriteLine("Heartbeat - WebSocket Exception: {0}", ex);
                            heartbeatTimer.Stop();
                        }
                    }
                    else
                    {
                        heartbeatTimer.Stop();
                    }
                };
                heartbeatTimer.AutoReset = true;

                using var pushTimer = new System.Timers.Timer(PushInterval);
                pushTimer.Elapsed += async (s, e) =>
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            await client.SendAsync(PushMessage(), WebSocketMessageType.Binary, true, default);
                        }
                        catch (WebSocketException ex)
                        {
                            Console.WriteLine("Push - WebSocket Exception: {0}", ex);
                            pushTimer.Stop();
                        }
                    }
                    else
                    {
                        pushTimer.Stop();
                    }
                };
                pushTimer.AutoReset = true;
                pushTimer.Enabled = true;
                #endregion

                #region Main loop
                while (client.State == WebSocketState.Open)
                {
                    try
                    {
                        var buffer = new byte[BufferSize];
                        await client.ReceiveAsync(buffer, default);

                        var stream = Decode(buffer);

                        HandleCommand(client, stream, heartbeatTimer);

                    }
                    catch (WebSocketException e)
                    {
                        Console.WriteLine("Main - WebSocket Exception: {0}", e);
                        break;
                    }
                }
                #endregion
            }
        }

        async void HandleCommand(ClientWebSocket client, DownstreamPayload stream, System.Timers.Timer heartbeatTimer)
        {
            if (stream == null) { return; }
            switch (stream.Command)
            {
                case Enums.Command.GLOBAL_COMMAND:
                    ZtLiveCsCmdAck cmd = ZtLiveCsCmdAck.Parser.ParseFrom(stream.PayloadData);

                    switch (cmd.CmdAckType)
                    {
                        case Enums.GlobalCommand.ENTER_ROOM_ACK:
                            var enterRoom = ZtLiveCsEnterRoomAck.Parser.ParseFrom(cmd.Payload);
                            heartbeatTimer.Interval = enterRoom.HeartbeatIntervalMs;
                            heartbeatTimer.Start();
                            break;
                        case Enums.GlobalCommand.HEARTBEAT_ACK:
                            var heartbeat = ZtLiveCsHeartbeatAck.Parser.ParseFrom(cmd.Payload);
                            break;
                        default:
                            Console.WriteLine("Unhandled Global.ZtLiveInteractive.CsCmd: {0}", cmd.CmdAckType);
                            Console.WriteLine(cmd);
                            break;
                    }
                    break;
                case Enums.Command.KEEP_ALIVE:
                    var keepalive = KeepAliveResponse.Parser.ParseFrom(stream.PayloadData);
                    break;
                case Enums.Command.PING:
                    var ping = PingResponse.Parser.ParseFrom(stream.PayloadData);
                    break;
                case Enums.Command.UNREGISTER:
                    var unregister = UnregisterResponse.Parser.ParseFrom(stream.PayloadData);
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unregister", default);
                    break;
                case Enums.Command.PUSH_MESSAGE:
                    ZtLiveScMessage message = ZtLiveScMessage.Parser.ParseFrom(stream.PayloadData);

                    var payload = message.CompressionType == ZtLiveScMessage.Types.CompressionType.Gzip ? Decompress(message.Payload) : message.Payload;

                    switch (message.MessageType)
                    {
                        case Enums.PushMessage.ACTION_SIGNAL:
                            // Handled by user
                            Handler(message.MessageType, payload.ToByteArray());
                            break;
                        case Enums.PushMessage.STATE_SIGNAL:
                            // Handled by user
                            Handler(message.MessageType, payload.ToByteArray());
                            break;
                        case Enums.PushMessage.STATUS_CHANGED:
                            var statusChanged = ZtLiveScStatusChanged.Parser.ParseFrom(payload);
                            if (statusChanged.Type == ZtLiveScStatusChanged.Types.Type.LiveClosed || statusChanged.Type == ZtLiveScStatusChanged.Types.Type.LiveBanned)
                            {
                                await client.SendAsync(Unregister(), WebSocketMessageType.Binary, true, default);
                                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Live closed", default);
                            }
                            break;
                        case Enums.PushMessage.TICKET_INVALID:
                            var ticketInvalid = ZtLiveScTicketInvalid.Parser.ParseFrom(payload);
                            TicketIndex = (TicketIndex + 1) % Tickets.Length;
                            await client.SendAsync(EnterRoom(), WebSocketMessageType.Binary, true, default);
                            break;
                    }
                    break;
                default:
                    if (stream.ErrorCode > 0)
                    {
                        Console.WriteLine("Error： {0} - {1}", stream.ErrorCode, stream.ErrorMsg);
                        Console.WriteLine(stream.ErrorData.ToBase64());
                    }
                    else
                    {
                        Console.WriteLine("Unhandled DownstreamPayload command: {0}", stream.Command);
                        Console.WriteLine(stream);
                    }
                    break;
            }
        }

        #region Common Functions
        byte[] Register()
        {
            var request = new RegisterRequest
            {
                AppInfo = new AppInfo
                {
                    AppName = AppName,
                    SdkVersion = SdkVersion,
                },
                DeviceInfo = new DeviceInfo
                {
                    PlatformType = DeviceInfo.Types.PlatformType.H5,
                    DeviceModel = "h5",
                },
                PresenceStatus = RegisterRequest.Types.PresenceStatus.KPresenceOnline,
                AppActiveStatus = RegisterRequest.Types.ActiveStatus.KAppInForeground,
                InstanceId = InstanceId,
                ZtCommonInfo = new ZtCommonInfo
                {
                    Kpn = KPN,
                    Kpf = KPF,
                    Uid = UserId,
                }
            };

            var payload = new UpstreamPayload
            {
                Command = Enums.Command.REGISTER,
                SeqId = SeqId++,
                RetryCount = RetryCount,
                PayloadData = request.ToByteString(),
                SubBiz = SubBiz,
            };

            var body = payload.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionServiceToken,
                DecodedPayloadLen = body.Length,
                TokenInfo = new TokenInfo
                {
                    TokenType = TokenInfo.Types.TokenType.KServiceToken,
                    Token = ByteString.CopyFromUtf8(ServiceToken),
                },
                SeqId = HeaderSeqId++,
                Kpn = KPN,
            };

            return Encode(header, body);
        }

        byte[] Unregister()
        {
            var unregister = new UnregisterRequest();

            var payload = new UpstreamPayload
            {
                Command = Enums.Command.UNREGISTER,
                RetryCount = RetryCount,
                PayloadData = unregister.ToByteString(),
                SubBiz = SubBiz
            };

            var body = payload.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionSessionKey,
                DecodedPayloadLen = body.Length,
                Kpn = KPN
            };

            return Encode(header, body);
        }

        byte[] Ping()
        {
            var ping = new PingRequest
            {
                PingType = PingRequest.Types.PingType.KPostRegister,
            };

            var payload = new UpstreamPayload
            {
                Command = Enums.Command.PING,
                SeqId = SeqId,
                RetryCount = RetryCount,
                PayloadData = ping.ToByteString(),
                SubBiz = SubBiz
            };

            var body = payload.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                InstanceId = InstanceId,
                DecodedPayloadLen = body.Length,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionSessionKey,
                Kpn = KPN
            };

            return Encode(header, body);
        }

        byte[] EnterRoom()
        {
            var request = new ZtLiveCsEnterRoom
            {
                EnterRoomAttach = EnterRoomAttach,
                ClientLiveSdkVersion = ClientLiveSdkVersion
            };

            var cmd = new ZtLiveCsCmd
            {
                CmdType = Enums.GlobalCommand.ENTER_ROOM,
                Payload = request.ToByteString(),
                Ticket = Tickets[TicketIndex],
                LiveId = LiveId,
            };

            var payload = new UpstreamPayload
            {
                Command = Enums.Command.GLOBAL_COMMAND,
                SeqId = SeqId++,
                RetryCount = RetryCount,
                PayloadData = cmd.ToByteString(),
                SubBiz = SubBiz,
            };

            var body = payload.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                InstanceId = InstanceId,
                DecodedPayloadLen = body.Length,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionSessionKey,
                SeqId = HeaderSeqId++,
                Kpn = KPN
            };

            return Encode(header, body);
        }

        byte[] KeepAlive()
        {
            var keepalive = new KeepAliveRequest
            {
                PresenceStatus = RegisterRequest.Types.PresenceStatus.KPresenceOnline,
                AppActiveStatus = RegisterRequest.Types.ActiveStatus.KAppInForeground,
            };

            var payload = new UpstreamPayload
            {
                Command = Enums.Command.KEEP_ALIVE,
                SeqId = SeqId,
                RetryCount = RetryCount,
                PayloadData = keepalive.ToByteString(),
                SubBiz = SubBiz
            };

            var body = payload.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                InstanceId = InstanceId,
                DecodedPayloadLen = body.Length,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionSessionKey,
                SeqId = SeqId,
                Kpn = KPN,
            };

            return Encode(header, body);
        }

        byte[] PushMessage()
        {
            var msg = new UpstreamPayload
            {
                Command = Enums.Command.PUSH_MESSAGE,
                SeqId = SeqId,
                RetryCount = RetryCount,
                SubBiz = SubBiz
            };

            var body = msg.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                InstanceId = InstanceId,
                DecodedPayloadLen = body.Length,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionSessionKey,
                SeqId = HeaderSeqId,
                Kpn = KPN,
            };

            return Encode(header, body);
        }

        byte[] Heartbeat()
        {
            var hearbeat = new ZtLiveCsHeartbeat
            {
                ClientTimestampMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Sequence = HeartbeatSeqId++,
            };

            var cmd = new ZtLiveCsCmd
            {
                CmdType = Enums.GlobalCommand.HEARTBEAT,
                Payload = hearbeat.ToByteString(),
                Ticket = Tickets[TicketIndex],
                LiveId = LiveId,
            };

            var payload = new UpstreamPayload
            {
                Command = Enums.Command.GLOBAL_COMMAND,
                SeqId = SeqId,
                RetryCount = RetryCount,
                PayloadData = cmd.ToByteString(),
                SubBiz = SubBiz,
            };

            var body = payload.ToByteString();

            var header = new PacketHeader
            {
                AppId = AppId,
                Uid = UserId,
                InstanceId = InstanceId,
                DecodedPayloadLen = body.Length,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionSessionKey,
                SeqId = SeqId++,
                Kpn = KPN
            };

            return Encode(header, body);
        }
        #endregion

        #region Utils
        byte[] Encode(PacketHeader header, ByteString body)
        {
            var bHeader = header.ToByteString();

            var key = header.EncryptionMode == PacketHeader.Types.EncryptionMode.KEncryptionServiceToken ? SecurityKey : SessionKey;
            var encrypt = Encrypt(key, body);

            var data = new byte[Offset + bHeader.Length + encrypt.Length];
            data[0] = 0xAB;
            data[1] = 0xCD;
            data[2] = 0x0;
            data[3] = 0x1;

            var packetLength = BitConverter.GetBytes(bHeader.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(packetLength);
            }
            Array.Copy(packetLength, 0, data, 4, packetLength.Length);

            var bodyLength = BitConverter.GetBytes(encrypt.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bodyLength);
            }
            Array.Copy(bodyLength, 0, data, 8, bodyLength.Length);

            bHeader.CopyTo(data, Offset);

            Array.Copy(encrypt, 0, data, Offset + bHeader.Length, encrypt.Length);

            return data;
        }

#if DEBUG
        public UpstreamPayload DecodeUpstream(byte[] bytes)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength, PrintHeader);

            byte[] payload;
            if (header.EncryptionMode != PacketHeader.Types.EncryptionMode.KEncryptionNone)
            {
                var key = header.EncryptionMode == PacketHeader.Types.EncryptionMode.KEncryptionServiceToken ? SecurityKey : SessionKey;

                payload = Decrypt(bytes, headerLength, payloadLength, key);
            }
            else
            {
                payload = new byte[payloadLength];
                Array.Copy(bytes, Offset + headerLength, payload, 0, payloadLength);
            }


            if (payload.Length != header.DecodedPayloadLen)
            {
#if DEBUG
                Console.WriteLine("Payload length does not match");
                Console.WriteLine(Convert.ToBase64String(payload));
#endif
                return null;
            }

            UpstreamPayload upstream = UpstreamPayload.Parser.ParseFrom(payload);

            return upstream;
        }
#endif

        public DownstreamPayload Decode(byte[] bytes)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength, PrintHeader);

            byte[] payload;
            if (header.EncryptionMode != PacketHeader.Types.EncryptionMode.KEncryptionNone)
            {
                var key = header.EncryptionMode == PacketHeader.Types.EncryptionMode.KEncryptionServiceToken ? SecurityKey : SessionKey;

                payload = Decrypt(bytes, headerLength, payloadLength, key);
            }
            else
            {
                payload = new byte[payloadLength];
                Array.Copy(bytes, Offset + headerLength, payload, 0, payloadLength);
            }


            if (payload.Length != header.DecodedPayloadLen)
            {
#if DEBUG
                Console.WriteLine("Payload length does not match");
                Console.WriteLine(Convert.ToBase64String(payload));
#endif
                return null;
            }

            DownstreamPayload downstream = DownstreamPayload.Parser.ParseFrom(payload);

            if (downstream.Command == "Push.ZtLiveInteractive.Message")
            {
                HeaderSeqId = header.SeqId;
            }


            return downstream;
        }

        internal static PacketHeader DecodeHeader(byte[] bytes, int headerLength, bool print)
        {
            PacketHeader header = PacketHeader.Parser.ParseFrom(bytes, Offset, headerLength);

            if (print)
            {
                Console.WriteLine("Header SeqId: {0}", header.SeqId);
            }

            return header;
        }

        internal static (int, int) DecodeLengths(byte[] bytes)
        {
            int headerLength, payloadLength;
            if (BitConverter.IsLittleEndian)
            {
                var header = new byte[4];
                var payload = new byte[4];

                Array.Copy(bytes, 4, header, 0, 4);
                Array.Reverse(header);
                headerLength = BitConverter.ToInt32(header);

                Array.Copy(bytes, 8, payload, 0, 4);
                Array.Reverse(payload);
                payloadLength = BitConverter.ToInt32(payload);
            }
            else
            {
                headerLength = BitConverter.ToInt32(bytes, 4);

                payloadLength = BitConverter.ToInt32(bytes, 8);
            }

            return (headerLength, payloadLength);
        }

        internal static byte[] Encrypt(string key, ByteString body)
        {
            using var aes = Aes.Create();

            using var encryptor = aes.CreateEncryptor(Convert.FromBase64String(key), aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            body.WriteTo(cs);
            cs.FlushFinalBlock();

            var encrypted = ms.ToArray();

            var payload = new byte[aes.IV.Length + encrypted.Length];
            Array.Copy(aes.IV, 0, payload, 0, aes.IV.Length);
            Array.Copy(encrypted, 0, payload, aes.IV.Length, encrypted.Length);

            return payload;
        }

        internal static byte[] Decrypt(byte[] bytes, int headerLength, int payloadLength, string key)
        {
            var payload = new byte[payloadLength];
            Array.Copy(bytes, Offset + headerLength, payload, 0, payloadLength);
            var IV = new byte[16];
            Array.Copy(payload, 0, IV, 0, 16);

            using var aes = Aes.Create();
            using var decryptor = aes.CreateDecryptor(Convert.FromBase64String(key), IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
            cs.Write(payload, 16, payloadLength - 16);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        internal static ByteString Compress(ByteString payload)
        {
            return GZip(CompressionMode.Compress, payload);
        }

        public static ByteString Decompress(ByteString payload)
        {
            return GZip(CompressionMode.Decompress, payload);
        }

        internal static ByteString GZip(CompressionMode mode, ByteString payload)
        {
            using var input = new MemoryStream(payload.ToByteArray());
            using var gzip = new GZipStream(input, mode);
            using var output = new MemoryStream();

            gzip.CopyTo(output);

            output.Position = 0;

            return ByteString.FromStream(output);
        }

        public static object Parse(string type, ByteString payload)
        {
            var t = Type.GetType($"AcFunDanmu.{type}");
            if (t != null)
            {
                var pt = typeof(MessageParser<>).MakeGenericType(new Type[] { t });

                var parser = t.GetProperty("Parser", pt).GetValue(null);
                var method = pt.GetMethod("ParseFrom", new Type[] { typeof(ByteString) });

                var ack = method.Invoke(parser, new object[] { payload });
                return ack;
            }
            else
            {
                Console.WriteLine("Unhandled type: {0}", type);
                Console.WriteLine(payload.ToBase64());
                return null;
            }
        }

        internal static bool LoadCookies(CookieContainer container)
        {
            var dir = new DirectoryInfo(".");
            var file = dir.GetFiles("cookies.txt");

            if (file.Count() == 1)
            {
                using var cookies = file[0].OpenText();

                Regex re = new Regex(@"^(?<host>[\w\.]+)\s+(?<host_only>TRUE|FALSE)\s+(?<path>.*?)\s+(?<secure>TRUE|FALSE)\s+(?<expire>\d+)\s+(?<name>.*?)\s+(?<value>.*?)$", RegexOptions.Multiline);

                var matches = re.Matches(cookies.ReadToEnd());

                foreach (Match match in matches)
                {
#if DEBUG
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", match.Groups["host"].Value, match.Groups["host_only"].Value, match.Groups["path"].Value, match.Groups["secure"].Value, match.Groups["expire"].Value, match.Groups["name"].Value, match.Groups["value"].Value);
#endif
                    var cookie = new Cookie
                    {
                        Domain = match.Groups["host"].Value,
                        HttpOnly = match.Groups["host_only"].Value == "TRUE",
                        Secure = match.Groups["secure"].Value == "TRUE",
                        Path = match.Groups["path"].Value,
                        Name = match.Groups["name"].Value,
                        Value = match.Groups["value"].Value
                    };
                    long expire = long.Parse(match.Groups["expire"].Value);
                    if (expire > 0)
                    {
                        cookie.Expires = DateTime.FromFileTimeUtc(expire);
                    }
                    container.Add(cookie);
                }
                return true;
            }
            return false;
        }
        #endregion
    }
}
