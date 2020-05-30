﻿using Google.Protobuf;
using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace AcFunDanmu
{
    class Program
    {
        const long UserId = 1000000039853473;
        const string DeviceId = "web_31706660307C719F";
        const string ServiceToken = "ChRhY2Z1bi5hcGkudmlzaXRvci5zdBJwP7Je6h3SU1hJGmKbfsA7GygEIicFbETu07fE6fhg2u8mfRljAozzV5Yzlf8IiHKbv5RfW-x3u_fGgzhIJK6Hhir7yc_8A5cDXpXWWBHzNYJNrvAaPi2TZJ3ckAJwBzqSAWgXvCBdTTY_NCrWQ9ff9BoSL5cQZ1OuMGkaFW6pJ8F0ajF5IiBAXKWs6SB9IitQenlc4zfiWJOu-t_S89d6LRa_lwj1BigFMAE";
        const string SecurityKey = "GUSzetFTEQG1knY847DefA==";
        const string SessionKey = "XlfISOm/HZzW25wOZ3+W2Q==";
        const int Offset = 12;
        static void Main(string[] args)
        {
            RegisterUp();

            RegisterEncode();
            //RegisterDown();

            //KeepAliveUp();
            //KeepAliveDown();

            //ZtLiveCsCmdUp("q80AAQAAACgAAAEACA0Qobuaueqv4wEY25SOjqnCoYqmATjgAUACUANiCUFDRlVOX0FQUDF2Rd4QAzWbEGTQfwZUPf0oBEt77vmR+KO2wCO5XgTEj9TsOnalZVXe9gOHw9KPIEEovgusVijmlLHPispISnS16Y0Kh/5jYhHIcIZRXuWk6yduMhIMbD/WDdNDMeJ2y56sOv6dEGqSpElh8HYF5taen3AUi4t1kyqhlN6cbJf+CtUK2ffTDFZGIyImRUBhNCHmIeFFl5qr8nKKxw9LXTcaWHHi4xcJJVGkzoWMrumGhhGn59FAp0XavFyvQm3XbCdCJaJRH0K5bgGco8zBhCHmASxOzWY+0p1lB5sXKnm8hL3UtH0mcgR9nj9kNAYHOrnnpHGPDNynABOvP2xWv6I=");
            //ZtLiveCsCmdDown("q80AAQAAACkAAABgCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOEhAAlADYglBQ0ZVTl9BUFDiuj9tT7Rj0eiXNiyeuj+L7DlgLZoMStU2eJH18NjI6Auua/NEvf8RuW6RQQp1CWvUZiEZFoa0+q0jt8ZTl9JHqfXd85NFW+ZWqvek0qeYlkKaDkUVyLmP/BTPRDCHR3s=");

            //ZtLiveCsCmdDown("q80AAQAAACMAAANQCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOL4GQAJQ4fqowwHxv24HnotW4u2QsJqgiearWcUfdhP1y7nrCIb5i8MHdqKAJNitZkWR7rL1iTOCHazW1ai6RQc2cqQ3dRL+J2YUznZvbVbjPhSWhVpehTsPmDgAHgO398CauY92XU6G7sZkg7i4BZCN8WeRrD46VUGLXDRLpRuk2oKD7unfHUx8Y8I+JrRS8MWGPPa5OP8di95JtXaR7Oa3omgFhpwt/BfuPgM3Sb82cWQBQ2vsLs6MH+mLasMZvl2+5G8d15rc3fELg0DcxmLzvlSl65ELcoDizuKl7azuf2/A7vaPpPhczc4WqOdAV9d25kXVcC6xbtYop/z/4aBn1mBpLVY0sq7DrmdvQD8od+MTEemzvXD7h8oaHd83qmVX0DsXuOZtM/O+C7tZindSUzbC2ksKTf97qB5lQP/c0I8aLBtSC6YfNIjMNaqcWlt4PNMhsFkwXaJ3LjCB9sXsIGNeWiXfeqLJys2YH5u2oMlhyoj0x5tjDGYV4ZvzKI5hh6DA1X4naqaqW2xzt1XN4SkLwuIGdm9wdrGXggmGiC+gd9CmeyeGq+OvWsjuKblV3bWwtOhmPqYwGPPQkpl3vG4PEcY0rLin3RsYMMDI7f59koSxN9D6rYcqDDtEizNRcz/wdrJE8b9/Z68qSzIe4kK+Lv7TXkX9F5Igq/AV0vCVrVlFCHF/pLGwQrLCfnDneum2lpjofLj7ZlKa4/J1JoSEw34k2aHqmBYmx7MGdDYqvi0xyPTthVruR/KAj7nGasj7AB6JjRpMCWOdgAO18DCfL9bZwT9CPK8GidF9DUvzKb2H4viOzuRac6kXesV/wKttv3cbAn9PUTMcYGmaryd4i43waVO7X5IuR/mh2QxyqrEyxEk0jt3DrhlqjXdMneZBSH4cx8nAYWLBrMZ3nuDH6lYtyZPxQ45tj3yKp7b69kwQpq+Dv1eUJMa1WRAdQY2UNMWeoyk63FPRgX17HJfWupRrguKN5peS9apq1n87UCzqbUbYyZhujrOpW/7hVgbTPgvntN6lq0vSAlhkQnXTbxU86zRqbxVhOaOZeutUgVPQnKpJ/M6uKyQbVZWeNcIXpLCl7UhPinJF0cISgGtuNCy4axYpVyrbW49a0aj3PBbtyrmasA==");

            //ZtLiveCsCmdUp("q80AAQAAACsAAABACA0Qobuaueqv4wEY25SOjqnCoYqmATgtQAJQ4fqowwFiCUFDRlVOX0FQUD9DOs30E7izyzzuf2dMRpvEUfFbvhWdBX9Efk74y9dLYQ9pNnSej8EgrMIYP2v5gb8HDqawzCXMYop2VQ28P5I=");

            //ZtLiveCsCmdDown("q80AAQAAACMAAACwCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOJcBQAJQ4vqowwFullLQ2pqSJ9vWy37La+H6UufNokELtIRNbg7Zm78KwLB3l/uUGGyMWcPSC7VKCP+6AlfavJ/HPvXiPxXE+u+7vzDEakZRxCCjJyZjlCbQ/wGXHquagxC2ekFBk64qZQY8Q4zNM29XKBE3WZnV/Xv+4NrgJTAmkJYWFMqPPmUF96zQQP9TSyqWziWkR7qD2XehyXd/kekPcAnheVCRjX7LzcCJEErJCVOcSqAF2nrPGA==");

            //ZtLiveCsCmdUp("q80AAQAAACsAAABACA0Qobuaueqv4wEY25SOjqnCoYqmATgtQAJQ4vqowwFiCUFDRlVOX0FQUF/x2ghNl1Zpw0SJX8NiRC+5/pHYFNuJGVTt9VoyhNoiKjqgsaqd3K0Q3wli+Atm95tSCzoRNUBsxO5K/BJxuyE=");

            //ZtLiveCsCmdDown("q80AAQAAACMAAAHgCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOM4DQAJQ4/qowwGpl2n06TTnu3O5eCasVda9LZlQEFIEtio3ekpNcevPaKfmP2W68eJSWnDyAc7pOFtcmfv7l5ag/XZNmjoIYkLz8rR+I+s87H1zWdkEX4yadfJ9J5o/NchAh8KJ6D6LEMjkJpoc2fYfxA+98Ce5Ggq/QuoPVWRX57qoCbFn+6DVD2nXoCj0yb92GsHaAnuxU69mixg5sFX8uUATjKTGDJUoaaNCJEiXDEnYlP7/5QcX5nBjSva74S0mbDpc8W8gvx8e0DlrwzrfO1yWpcMABu/eBLvUtMlkB9skdpi2SK3Eatp/Zx7ALP8/lE0Z10jkXJu/JnJnFMNAC5TrblIGM/zM46hsFjoWwpY42FeC2lQhO8p3OfGLjR/e6t5ASwnzeCzopqVOErCd5TnV3pucmNfZXzIWmhnb+M/69M4YEHV94f1AcAJdvJ0I90XVTbKYYOKLvuKIQG8TxytnE+y+G5kKekux/Z56gQ14CqwbHptYNUOAwD9u/7uXpIQOv9wzAIt8f+E7mpVyjCtHiA/M3FCQJ363r++CVjrmOn2jJs9u6W3ce3jHg0ESBoThFEu0fMlUkpWgR6vniLnDQ3e03FRrnj8ov2DxztQKSoy94ymErjpDlG9pU+pGYhkz9fdr9xU=");

            //ZtLiveCsCmdUp("q80AAQAAACsAAABACA0Qobuaueqv4wEY25SOjqnCoYqmATgtQAJQ4/qowwFiCUFDRlVOX0FQUKp/tWMxO7N2FIFKfe8ZpYclUWNjqfW8FWsgSJlat0aqb6X3oK7XQhA4bFjSBgPQqkGLVKrXn+g2xayB4QJqz7g=");

            //ZtLiveCsCmdDown("q80AAQAAACMAAACwCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOJcBQAJQ5PqowwEoFxFFJ0gRa8JxQfreU+QR2nJfj/9tn21gLQriAIkgP99cvZKbvjqyBSoUmKStE7VXC+jpIF+Vi0Zix2qdrgNU7k1xb6PN0Jcou4vsv2TD3QaN3l0xuWk2DZ2rJBtSgMGLBoYnJxBnW8jq7nt5kp/zOwLJQgnkKEdHcVTN2OpwJ7X/U/gem2Q2kjkS327QuCLIWQvYueYFD00BbirZCamzSEYYzdLbbpDbq2BTK50EnA==");

            //ZtLiveCsCmdUp("q80AAQAAACsAAABACA0Qobuaueqv4wEY25SOjqnCoYqmATgtQAJQ5PqowwFiCUFDRlVOX0FQUBmyWrNL4T15cB2KTksvfji2kNRd6SDOdAo+gvnBLyfkGdjgZn+zNCd5KQiRUfhLPifEGdrCw5fNQ/csbI7GwOQ=");

            //ZtLiveCsCmdDown("q80AAQAAACMAAACwCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOJcBQAJQ5fqowwFrs0I25yJF/Yd1OloIp91w+R3z7Nn7zfY6hYjir6kiQEJcfPLsOgSpyUDwMP9mhztGx6tHQtKZKceVBnSSOM9SEj1miFGoPRjUQRydfZVNfSIcrgs1gg3k2/BhpH1xAJOyN0zRvCIjqRcppiD5Tn8Y3VVj4ONkwP00u5GJe3JDKtLuaJkaJTZGAZBXO3FDzJHsXUjEhxhiDghtkdyBWSYiInDxrzBkUhk+ub9q2E3aDg==");

        }

        static void RegisterEncode()
        {
            RegisterRequest request = new RegisterRequest
            {
                AppInfo = new AppInfo
                {
                    AppName = "link-sdk",
                    SdkVersion = "1.2.1",
                },
                DeviceInfo = new DeviceInfo
                {
                    PlatformType = DeviceInfo.Types.PlatformType.H5,
                    DeviceModel = "h5",
                },
                PresenceStatus = RegisterRequest.Types.PresenceStatus.KPresenceOnline,
                AppActiveStatus = RegisterRequest.Types.ActiveStatus.KAppInForeground,
                InstanceId = 0,
                ZtCommonInfo = new ZtCommonInfo
                {
                    Kpn = "ACFUN_APP",
                    Kpf = "OUTSIDE_ANDROID_H5",
                    Uid = UserId,
                }
            };

            UpstreamPayload payload = new UpstreamPayload
            {
                Command = "Basic.Register",
                SeqId = 1,
                RetryCount = 1,
                PayloadData = request.ToByteString(),
                SubBiz = "mainApp"
            };

            var body = payload.ToByteString();

            PacketHeader header = new PacketHeader
            {
                AppId = 13,
                Uid = UserId,
                EncryptionMode = PacketHeader.Types.EncryptionMode.KEncryptionServiceToken,
                DecodedPayloadLen = body.Length,
                TokenInfo = new TokenInfo
                {
                    TokenType = TokenInfo.Types.TokenType.KServiceToken,
                    Token = ByteString.CopyFromUtf8(ServiceToken),
                },
                SeqId = 1,
                Kpn = "AFUN_APP",
            };

            var data = Encode(header, body);

            UpstreamPayload upstream = DecodeUpstream(data);

            RegisterRequest request2 = RegisterRequest.Parser.ParseFrom(upstream.PayloadData);
            Console.WriteLine(request2);
        }

        static void RegisterUp()
        {
            var bytes = Convert.FromBase64String("q80AAQAAASkAAACACA0Qobuaueqv4wEYADhqQAFKiAIIARKDAkNoUmhZMloxYmk1aGNHa3VkbWx6YVhSdmNpNXpkQkp3UDdKZTZoM1NVMWhKR21LYmZzQTdHeWdFSWljRmJFVHUwN2ZFNmZoZzJ1OG1mUmxqQW96elY1WXpsZjhJaUhLYnY1UmZXLXgzdV9mR2d6aElKSzZIaGlyN3ljXzhBNWNEWHBYV1dCSHpOWUpOcnZBYVBpMlRaSjNja0FKd0J6cVNBV2dYdkNCZFRUWV9OQ3JXUTlmZjlCb1NMNWNRWjFPdU1Ha2FGVzZwSjhGMGFqRjVJaUJBWEtXczZTQjlJaXRRZW5sYzR6ZmlXSk91LXRfUzg5ZDZMUmFfbHdqMUJpZ0ZNQUVQAWIJQUNGVU5fQVBQFcG8B9GlyZdIamzJo4AmJ9SOUu952Smy7NBeKovpedSQyNM9fheVW1hMjMRvUaNx0e6vXRyl7x93dvELU96BAW63FeqGWy+rLt4sXMvAWvD40tiYf/q+pWRLojKxnVVSkRl6mLbknDDqJIgpvmnyJ8JK0/VpdWDb6F7obZLWy8Q=");

            UpstreamPayload upstream = DecodeUpstream(bytes);

            RegisterRequest request = RegisterRequest.Parser.ParseFrom(upstream.PayloadData);
            Console.WriteLine(request);
        }

        static void RegisterDown()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACoAAACgCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOIcBQAFQAWIJQUNGVU5fQVBQHjz3LTAMExMzVqCfKNeyI0ymEaFnMLImEklmIPd0eYuXaXYNEGSZYUExqfhIkIgZS8EjLBq7a9v2GzM/ov6inZvHojT0c6jpKks6yJggzOWYeEea+0YurQYPWeNHab5mWqBfda9Ngh0IAVpTZWLhvLmcn2LGywntX58OrTuPvFQCV3TXKOzWOrNWcZ31nn/lHbx7/ExU+UD+ewdg43HsJQ==");

            DownstreamPayload downstream = Decode(bytes);

            RegisterResponse response = RegisterResponse.Parser.ParseFrom(downstream.PayloadData);
            Console.WriteLine(response);
        }

        static void KeepAliveUp()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACcAAABACA0Qobuaueqv4wEY25SOjqnCoYqmATgkQAJQAmIJQUNGVU5fQVBQyOKlJtixhaxHOojZV0lZoI23BhRoPjQdhUMt6TesJv/KTBLQQzjdg1INgblNLpsswKsa0bm/QZp0gBrqHbivTA==");

            var stream = DecodeUpstream(bytes);

            KeepAliveRequest request = KeepAliveRequest.Parser.ParseFrom(stream.PayloadData);
            Console.WriteLine(request);
        }

        static void KeepAliveDown()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACkAAABACA0Qobuaueqv4wEY25SOjqnCoYqmASgBOCtAAlACYglBQ0ZVTl9BUFAJ0COzU30GFM93HV0SgPEciw8Yx1X9TXccihr8ME/Cwnaw9ldC5RjCfUkL2MB8oqLxquXdkyGsdzwDiOLMSOM1");

            var stream = Decode(bytes);

            KeepAliveResponse response = KeepAliveResponse.Parser.ParseFrom(stream.PayloadData);
            Console.WriteLine(response);
        }

        static void ZtLiveCsCmdUp(string str)
        {
            var bytes = Convert.FromBase64String(str);

            var stream = DecodeUpstream(bytes);

            switch (stream.Command)
            {
                case "Push.ZtLiveInteractive.Message":
                    ZtLiveScMessage message = ZtLiveScMessage.Parser.ParseFrom(stream.PayloadData);
                    Console.WriteLine(message);

                    switch (message.MessageType)
                    {
                        case "ZtLiveScStateSignal":
                            ZtLiveScStateSignal signal = null;
                            switch (message.CompressionType)
                            {
                                case ZtLiveScMessage.Types.CompressionType.None:
                                    signal = ZtLiveScStateSignal.Parser.ParseFrom(message.Payload);
                                    break;
                                case ZtLiveScMessage.Types.CompressionType.Gzip:
                                    signal = ZtLiveScStateSignal.Parser.ParseFrom(Decompress(message.Payload));
                                    break;
                                case ZtLiveScMessage.Types.CompressionType.Unknown:
                                    return;
                            }
                            Console.WriteLine(signal);
                            foreach (var item in signal.Item)
                            {
                                var pi = Parse(item.SingalType, item.Payload);
                                Console.WriteLine(pi);
                            }
                            break;
                        default:
                            if (!string.IsNullOrEmpty(message.MessageType))
                            {
                                Console.WriteLine("Unhandled type: {0}", message.MessageType);
                                Console.WriteLine(message);
                            }
                            break;
                    }
                    break;
                case "ZtLiveCsCmd":
                    ZtLiveCsCmd cmd = ZtLiveCsCmd.Parser.ParseFrom(stream.PayloadData);
                    Console.WriteLine(cmd);

                    switch (cmd.CmdType)
                    {
                        case "ZtLiveCsEnterRoom":
                            var enterRoom = ZtLiveCsEnterRoom.Parser.ParseFrom(cmd.Payload);
                            Console.WriteLine(enterRoom);
                            break;
                        default:
                            Console.WriteLine("Unhandled command: {0}", cmd.CmdType);
                            Console.WriteLine(cmd);
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled command: {0}", stream.Command);
                    Console.WriteLine(stream);
                    break;
            }
        }

        static void ZtLiveCsCmdDown(string str)
        {
            var bytes = Convert.FromBase64String(str);

            var stream = DecodeUpstream(bytes);

            switch (stream.Command)
            {
                case "Push.ZtLiveInteractive.Message":
                    ZtLiveScMessage message = ZtLiveScMessage.Parser.ParseFrom(stream.PayloadData);
                    Console.WriteLine(message);

                    switch (message.MessageType)
                    {
                        case "ZtLiveScStateSignal":
                            ZtLiveScStateSignal signal = null;
                            switch (message.CompressionType)
                            {
                                case ZtLiveScMessage.Types.CompressionType.None:
                                    signal = ZtLiveScStateSignal.Parser.ParseFrom(message.Payload);
                                    break;
                                case ZtLiveScMessage.Types.CompressionType.Gzip:
                                    signal = ZtLiveScStateSignal.Parser.ParseFrom(Decompress(message.Payload));
                                    break;
                                case ZtLiveScMessage.Types.CompressionType.Unknown:
                                    return;
                            }
                            Console.WriteLine(signal);
                            foreach (var item in signal.Item)
                            {
                                var pi = Parse(item.SingalType, item.Payload);
                                Console.WriteLine(pi);
                            }
                            break;
                    }
                    break;
                case "ZtLiveCsCmdAck":
                    ZtLiveCsCmdAck cmd = ZtLiveCsCmdAck.Parser.ParseFrom(stream.PayloadData);
                    Console.WriteLine(cmd);

                    switch (cmd.CmdAckType)
                    {
                        case "ZtLiveCsEnterRoomAck":
                            var enterRoom = ZtLiveCsEnterRoomAck.Parser.ParseFrom(cmd.Payload);
                            Console.WriteLine(enterRoom);
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled command: {0}", stream.Command);
                    Console.WriteLine(stream);
                    break;
            }
        }

        static byte[] Encode(PacketHeader header, ByteString body)
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

        static UpstreamPayload DecodeUpstream(byte[] bytes)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength);

            var key = header.EncryptionMode == PacketHeader.Types.EncryptionMode.KEncryptionServiceToken ? SecurityKey : SessionKey;

            var payload = Decrypt(bytes, headerLength, payloadLength, key);

            UpstreamPayload upstream = UpstreamPayload.Parser.ParseFrom(payload);
            Console.WriteLine(upstream);

            return upstream;
        }

        static DownstreamPayload Decode(byte[] bytes)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength);

            var key = header.EncryptionMode == PacketHeader.Types.EncryptionMode.KEncryptionServiceToken ? SecurityKey : SessionKey;

            var payload = Decrypt(bytes, headerLength, payloadLength, key);

            DownstreamPayload downstream = DownstreamPayload.Parser.ParseFrom(payload);
            Console.WriteLine(downstream);

            return downstream;
        }

        static PacketHeader DecodeHeader(byte[] bytes, int headerLength)
        {
            PacketHeader header;
            header = PacketHeader.Parser.ParseFrom(bytes, Offset, headerLength);

            Console.WriteLine(header);

            return header;
        }

        static (int, int) DecodeLengths(byte[] bytes)
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

        static byte[] Encrypt(string key, ByteString body)
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

        static byte[] Decrypt(byte[] bytes, int headerLength, int payloadLength, string key)
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

        static byte[] Compress(ByteString payload)
        {
            using var input = new MemoryStream(payload.ToByteArray());
            using var gzip = new GZipStream(input, CompressionMode.Compress);
            using var output = new MemoryStream();

            gzip.CopyTo(output);

            return output.ToArray();
        }

        static byte[] Decompress(ByteString payload)
        {
            using var input = new MemoryStream(payload.ToByteArray());
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();

            gzip.CopyTo(output);

            return output.ToArray();
        }

        static object Parse(string type, ByteString payload)
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
