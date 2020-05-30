using Google.Protobuf;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Text.Json;

namespace AcFunDanmu
{
    public delegate void MessageHandler(DownstreamPayload payload);
    public class Client
    {
        private const string _ACFUN_HOST = "https://m.acfun.cn";
        private static readonly Uri ACFUN_HOST = new Uri(_ACFUN_HOST);
        private const string LIVE_URL = "https://m.acfun.cn/live/detail";
        private const string LOGIN_URL = "https://id.app.acfun.cn/rest/app/visitor/login";
        private static readonly Uri LOGIN_URI = new Uri(LOGIN_URL);
        private const string PLAY_URL = "https://api.kuaishouzt.com/rest/zt/live/web/startPlay?subBiz=mainApp&kpn=ACFUN_APP&kpf=OUTSIDE_IOS_H5&userId={0}&did={1}&acfun.api.visitor_st={2}";

        private const string UserAgent = "Mozilla/5.0 (iPad; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";

        private static readonly Dictionary<string, string> LOGIN_FORM = new Dictionary<string, string> { { "sid", "acfun.api.visitor" } };

        const string _Host = "wss://link.xiatou.com/";
        static readonly Uri Host = new Uri(_Host);
        const int Offset = 12;
        const int BufferSize = 2 << 16;
        const int AppId = 13;
        const string AppName = "link-sdk";
        const string SdkVersion = "1.2.1";
        const string KPN = "ACFUN_APP";
        const string SubBiz = "mainApp";
        const string ClientLiveSdkVersion = "kwai-acfun-live-link";

        public MessageHandler Handler { get; set; }

        private long UserId { get; set; } = -1;
        private string ServiceToken { get; set; }
        private string SecurityKey { get; set; }
        private string LiveId { get; set; }
        private string EnterRoomAttach { get; set; }
        private string[] Tickets { get; set; }

        private long InstanceId = 0;
        private string SessionKey { get; set; }
        private long HeaartbeatInterval { get; set; }
        private long Lz4CompressionThreshold { get; set; }

        private CancellationTokenSource CancellationTokenSource;

        private ClientWebSocket client;

        private System.Timers.Timer heartbeatTimer = null;
        private System.Timers.Timer pushTimer = null;

        private long SeqId = 1;
        private uint RetryCount = 1;

        public Client()
        {
            Handler = HandleCommand;
        }

        public Client(string uid) : this()
        {
            CancellationTokenSource = new CancellationTokenSource();

            client = new ClientWebSocket();

            Initialize(uid).Wait();
        }

        public Client(long userId, string serviceToken, string securityKey, string[] tickets, string enterRoomAttach, string liveId) : this()
        {
            UserId = userId;
            ServiceToken = serviceToken;
            SecurityKey = securityKey;
            Tickets = tickets;
            EnterRoomAttach = enterRoomAttach;
            LiveId = LiveId;
        }

        private async Task Initialize(string uid)
        {
            Console.WriteLine("Client initializing");
            var Cookies = new CookieContainer();

            var client = new HttpClient(
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.All,
                    UseCookies = true,
                    CookieContainer = Cookies
                }
            );
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent); // Mobile only
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");

            using var index = await client.GetAsync(ACFUN_HOST);
            if (!index.IsSuccessStatusCode) { return; }
            var deviceId = Cookies.GetCookies(ACFUN_HOST).Where(cookie => cookie.Name == "_did").First().Value;

            using var loginContent = new FormUrlEncodedContent(LOGIN_FORM);
            using var login = await client.PostAsync(LOGIN_URL, loginContent);
            if (!login.IsSuccessStatusCode) { return; }
            using var loginData = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());

            UserId = loginData.RootElement.GetProperty("userId").GetInt64();
            ServiceToken = loginData.RootElement.GetProperty("acfun.api.visitor_st").ToString();
            SecurityKey = loginData.RootElement.GetProperty("acSecurity").ToString();

            using var form = new FormUrlEncodedContent(new Dictionary<string, string> { { "authorId", uid } });
            using var play = await client.PostAsync(string.Format(PLAY_URL, UserId, deviceId, ServiceToken), form);

            if (!play.IsSuccessStatusCode) { return; }

            using var playData = await JsonDocument.ParseAsync(await play.Content.ReadAsStreamAsync());
            if (playData.RootElement.GetProperty("result").GetInt32() != 1) { return; }
            Tickets = playData.RootElement.GetProperty("data").GetProperty("availableTickets").EnumerateArray().Select(ticket => ticket.ToString()).ToArray();
            EnterRoomAttach = playData.RootElement.GetProperty("data").GetProperty("enterRoomAttach").ToString();
            LiveId = playData.RootElement.GetProperty("data").GetProperty("liveId").ToString();

            Console.WriteLine("Client initialized");
        }

        public async Task Start()
        {
            if(UserId == -1 || string.IsNullOrEmpty(ServiceToken) || string.IsNullOrEmpty(SecurityKey) || string.IsNullOrEmpty(LiveId) || string.IsNullOrEmpty(EnterRoomAttach) || Tickets == null)
            {
                Console.WriteLine("Not initialized or live is ended");
                return;
            }
            await client.ConnectAsync(Host, CancellationTokenSource.Token);
            if (client.State == WebSocketState.Open)
            {
                //Register
                await client.SendAsync(Register(), WebSocketMessageType.Binary, true, CancellationTokenSource.Token);
                var resp = new byte[BufferSize];
                await client.ReceiveAsync(resp, CancellationTokenSource.Token);
                var registerDown = Decode(resp);
                var regResp = RegisterResponse.Parser.ParseFrom(registerDown.PayloadData);
                InstanceId = regResp.InstanceId;
                SessionKey = regResp.SessKey.ToBase64();
                Lz4CompressionThreshold = regResp.SdkOption.Lz4CompressionThresholdBytes;

                //Enter room
                await client.SendAsync(EnterRoom(), WebSocketMessageType.Binary, true, CancellationTokenSource.Token);

                //Keep Alive
                await client.SendAsync(KeepAlive(), WebSocketMessageType.Binary, true, CancellationTokenSource.Token);

                //Push message
                pushTimer = new System.Timers.Timer(2000);
                pushTimer.Elapsed += async (s, e) =>
                {
                    var msg = new UpstreamPayload
                    {
                        Command = "Push.ZtLiveInteractive.Message",
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
                        SeqId = SeqId,
                        Kpn = KPN,
                    };

                    var message = Encode(header, body);
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            await client.SendAsync(message, WebSocketMessageType.Binary, true, CancellationTokenSource.Token);
                        }
                        catch (WebSocketException ex)
                        {
                            Console.WriteLine("WebSocket Exception: {0}", ex);
                            pushTimer.Stop();
                            pushTimer.Close();
                            pushTimer.Dispose();
                        }
                    }
                    else
                    {
                        pushTimer.Stop();
                        pushTimer.Close();
                        pushTimer.Dispose();
                    }
                };
                pushTimer.AutoReset = true;
                pushTimer.Enabled = true;

                while (client.State == WebSocketState.Open)
                {
                    try
                    {
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        var buffer = new byte[BufferSize];
                        await client.ReceiveAsync(buffer, CancellationTokenSource.Token);

                        var stream = Decode(buffer);

                        Handler(stream);

                    }
                    catch (WebSocketException e)
                    {
                        Console.WriteLine("WebSocket Exception: {0}", e.Message);
                        break;
                    }
                }
                await client.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationTokenSource.Token);
                client.Dispose();
            }
        }

        void HandleCommand(DownstreamPayload stream)
        {
            if (stream == null) { return; }
            switch (stream.Command)
            {
                case "Global.ZtLiveInteractive.CsCmd":
                    ZtLiveCsCmdAck cmd = ZtLiveCsCmdAck.Parser.ParseFrom(stream.PayloadData);

                    switch (cmd.CmdAckType)
                    {
                        case "ZtLiveCsEnterRoomAck":
                            var enterRoom = ZtLiveCsEnterRoomAck.Parser.ParseFrom(cmd.Payload);
                            HeaartbeatInterval = enterRoom.HeartbeatIntervalMs;
                            if (heartbeatTimer == null)
                            {
                                heartbeatTimer = new System.Timers.Timer(HeaartbeatInterval);
                                heartbeatTimer.Elapsed += Heartbeat;
                                heartbeatTimer.AutoReset = true;
                                heartbeatTimer.Enabled = true;
                            }
                            break;
                        case "ZtLiveCsHeartbeatAck":
                            var heartbeat = ZtLiveCsHeartbeatAck.Parser.ParseFrom(cmd.Payload);
                            break;
                        default:
                            Console.WriteLine("Unhandled Global.ZtLiveInteractive.CsCmd: {0}", cmd.CmdAckType);
                            Console.WriteLine(cmd);
                            break;
                    }
                    break;
                case "Basic.KeepAlive":
                    var keepalive = KeepAliveResponse.Parser.ParseFrom(stream.PayloadData);
                    break;
                case "Push.ZtLiveInteractive.Message":
                    // Handled by caller
                    break;
                default:
                    Console.WriteLine("Unhandled DownstreamPayload command: {0}", stream.Command);
                    Console.WriteLine(stream);
                    break;
            }
        }

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
                    Kpf = "OUTSIDE_ANDROID_H5",
                    Uid = UserId,
                }
            };

            var payload = new UpstreamPayload
            {
                Command = "Basic.Register",
                SeqId = SeqId,
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
                SeqId = SeqId,
                Kpn = KPN,
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
                CmdType = "ZtLiveCsEnterRoom",
                Payload = request.ToByteString(),
                Ticket = Tickets[0],
                LiveId = LiveId,
            };

            var payload = new UpstreamPayload
            {
                Command = "Global.ZtLiveInteractive.CsCmd",
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
                Command = "Basic.KeepAlive",
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

        async void Heartbeat(object source, ElapsedEventArgs e)
        {
            var hearbeat = new ZtLiveCsHeartbeat
            {
                ClientTimestampMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Sequence = SeqId
            };

            var cmd = new ZtLiveCsCmd
            {
                CmdType = "ZtLiveCsHeartbeat",
                Payload = hearbeat.ToByteString(),
                Ticket = Tickets[0],
                LiveId = LiveId,
            };

            var payload = new UpstreamPayload
            {
                Command = "Global.ZtLiveInteractive.CsCmd",
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
                SeqId = SeqId,
                Kpn = KPN
            };

            var message = Encode(header, body);

            if (client.State == WebSocketState.Open)
            {
                try
                {
                    await client.SendAsync(
                        message,
                        WebSocketMessageType.Binary,
                        true,
                        CancellationTokenSource.Token
                    );
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine("WebSocket Exception: {0}", ex);
                    heartbeatTimer.Stop();
                    heartbeatTimer.Close();
                    heartbeatTimer.Dispose();
                }
            }
            else
            {
                heartbeatTimer.Stop();
                heartbeatTimer.Close();
                heartbeatTimer.Dispose();
            }
        }

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

            SeqId++;

            return data;
        }

        DownstreamPayload Decode(byte[] bytes)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength);

            var key = header.EncryptionMode == PacketHeader.Types.EncryptionMode.KEncryptionServiceToken ? SecurityKey : SessionKey;

            var payload = Decrypt(bytes, headerLength, payloadLength, key);

            if (payload.Length != header.DecodedPayloadLen)
            {
#if DEBUG
                Console.WriteLine("Payload length does not match");
                Console.WriteLine(Convert.ToBase64String(payload));
#endif
                return null;
            }

            DownstreamPayload downstream = DownstreamPayload.Parser.ParseFrom(payload);

            SeqId++;

            return downstream;
        }

        internal static PacketHeader DecodeHeader(byte[] bytes, int headerLength)
        {
            PacketHeader header;
            header = PacketHeader.Parser.ParseFrom(bytes, Offset, headerLength);

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
            var t = Type.GetType(type);
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
    }
}
