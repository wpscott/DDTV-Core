using System;
using System.IO;
using System.Security.Cryptography;

namespace AcFunDanmu
{
    class Program
    {
        const long UserId = 1000000039853473;
        const string DeviceId = "web_31706660307C719F";
        const string SecurityKey = "GUSzetFTEQG1knY847DefA==";
        const string SessionKey = "XlfISOm/HZzW25wOZ3+W2Q==";
        const int Offset = 12;
        static void Main(string[] args)
        {
            RegisterUp();
            RegisterDown();

            KeepAliveUp();
            KeepAliveDown();

            ZtLiveCsCmdUp();
        }

        static void RegisterUp()
        {
            var bytes = Convert.FromBase64String("q80AAQAAASkAAACACA0Qobuaueqv4wEYADhqQAFKiAIIARKDAkNoUmhZMloxYmk1aGNHa3VkbWx6YVhSdmNpNXpkQkp3UDdKZTZoM1NVMWhKR21LYmZzQTdHeWdFSWljRmJFVHUwN2ZFNmZoZzJ1OG1mUmxqQW96elY1WXpsZjhJaUhLYnY1UmZXLXgzdV9mR2d6aElKSzZIaGlyN3ljXzhBNWNEWHBYV1dCSHpOWUpOcnZBYVBpMlRaSjNja0FKd0J6cVNBV2dYdkNCZFRUWV9OQ3JXUTlmZjlCb1NMNWNRWjFPdU1Ha2FGVzZwSjhGMGFqRjVJaUJBWEtXczZTQjlJaXRRZW5sYzR6ZmlXSk91LXRfUzg5ZDZMUmFfbHdqMUJpZ0ZNQUVQAWIJQUNGVU5fQVBQFcG8B9GlyZdIamzJo4AmJ9SOUu952Smy7NBeKovpedSQyNM9fheVW1hMjMRvUaNx0e6vXRyl7x93dvELU96BAW63FeqGWy+rLt4sXMvAWvD40tiYf/q+pWRLojKxnVVSkRl6mLbknDDqJIgpvmnyJ8JK0/VpdWDb6F7obZLWy8Q=");

            UpstreamPayload upstream = DecodeUpstream(bytes, SecurityKey);

            RegisterRequest request = RegisterRequest.Parser.ParseFrom(upstream.PayloadData);
            Console.WriteLine(request);
        }

        static void RegisterDown()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACoAAACgCA0Qobuaueqv4wEY25SOjqnCoYqmASgBOIcBQAFQAWIJQUNGVU5fQVBQHjz3LTAMExMzVqCfKNeyI0ymEaFnMLImEklmIPd0eYuXaXYNEGSZYUExqfhIkIgZS8EjLBq7a9v2GzM/ov6inZvHojT0c6jpKks6yJggzOWYeEea+0YurQYPWeNHab5mWqBfda9Ngh0IAVpTZWLhvLmcn2LGywntX58OrTuPvFQCV3TXKOzWOrNWcZ31nn/lHbx7/ExU+UD+ewdg43HsJQ==");

            DownstreamPayload downstream = Decode(bytes, SecurityKey);

            RegisterResponse response = RegisterResponse.Parser.ParseFrom(downstream.PayloadData);
            Console.WriteLine(response);
        }

        static void KeepAliveUp()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACcAAABACA0Qobuaueqv4wEY25SOjqnCoYqmATgkQAJQAmIJQUNGVU5fQVBQyOKlJtixhaxHOojZV0lZoI23BhRoPjQdhUMt6TesJv/KTBLQQzjdg1INgblNLpsswKsa0bm/QZp0gBrqHbivTA==");

            var stream = DecodeUpstream(bytes, SessionKey);

            KeepAliveRequest request = KeepAliveRequest.Parser.ParseFrom(stream.PayloadData);
            Console.WriteLine(request);
        }

        static void KeepAliveDown()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACkAAABACA0Qobuaueqv4wEY25SOjqnCoYqmASgBOCtAAlACYglBQ0ZVTl9BUFAJ0COzU30GFM93HV0SgPEciw8Yx1X9TXccihr8ME/Cwnaw9ldC5RjCfUkL2MB8oqLxquXdkyGsdzwDiOLMSOM1");

            var stream = Decode(bytes, SessionKey);

            KeepAliveResponse response = KeepAliveResponse.Parser.ParseFrom(stream.PayloadData);
            Console.WriteLine(response);
        }

        static void ZtLiveCsCmdUp()
        {
            var bytes = Convert.FromBase64String("q80AAQAAACgAAAEACA0Qobuaueqv4wEY25SOjqnCoYqmATjgAUACUANiCUFDRlVOX0FQUDF2Rd4QAzWbEGTQfwZUPf0oBEt77vmR+KO2wCO5XgTEj9TsOnalZVXe9gOHw9KPIEEovgusVijmlLHPispISnS16Y0Kh/5jYhHIcIZRXuWk6yduMhIMbD/WDdNDMeJ2y56sOv6dEGqSpElh8HYF5taen3AUi4t1kyqhlN6cbJf+CtUK2ffTDFZGIyImRUBhNCHmIeFFl5qr8nKKxw9LXTcaWHHi4xcJJVGkzoWMrumGhhGn59FAp0XavFyvQm3XbCdCJaJRH0K5bgGco8zBhCHmASxOzWY+0p1lB5sXKnm8hL3UtH0mcgR9nj9kNAYHOrnnpHGPDNynABOvP2xWv6I=");

            var stream = DecodeUpstream(bytes, SessionKey);

            ZtLiveCsCmd cmd = ZtLiveCsCmd.Parser.ParseFrom(stream.PayloadData);
            Console.WriteLine(cmd);

            switch (cmd.CmdType)
            {
                case "ZtLiveCsEnterRoom":
                    var enterRoom = ZtLiveCsEnterRoom.Parser.ParseFrom(cmd.Payload);
                    Console.WriteLine(enterRoom);
                    break;
            }
        }


        static UpstreamPayload DecodeUpstream(byte[] bytes, string key)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength);

            var payload = Decrypt(bytes, headerLength, payloadLength, key);

            UpstreamPayload upstream = UpstreamPayload.Parser.ParseFrom(payload);
            Console.WriteLine(upstream);

            return upstream;
        }

        static DownstreamPayload Decode(byte[] bytes, string key)
        {
            var (headerLength, payloadLength) = DecodeLengths(bytes);

            PacketHeader header = DecodeHeader(bytes, headerLength);

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
    }
}
