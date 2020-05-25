using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BilibiliLiveServer
{
    public static class Server
    {
        private const string HOST = "live.bilibili.com";
        private const string LIVE = "http://" + HOST;
        private const string API = "http://api." + HOST;

        private static readonly Regex IdRegex = new Regex(@"^GET /(?<id>\d+) HTTP");

        private const int Port = 29119;
        private static readonly TcpListener server;

        private const int BufferSize = 1 << 16;

        private static readonly Encoding Encoding = Encoding.ASCII;

        public static readonly string Address = $"http://{IPAddress.Loopback}:{Port}";

        static Server()
        {
            server = new TcpListener(IPAddress.Loopback, Port);
        }

        public async static Task Start()
        {
            server.Start();

            while (true)
            {
                var client = await server.AcceptTcpClientAsync();

                //new Thread(new ParameterizedThreadStart(Stream)).Start(client);

                await Task.Run(() => Stream(client));
            }
        }

        private async static void Stream(object obj)
        {
            var buffer = new byte[BufferSize];
            using var stream = (obj as TcpClient)?.GetStream();

            try
            {
                var count = stream.Read(buffer, 0, buffer.Length);

                if (count > 0)
                {
                    var data = Encoding.GetString(buffer);

                    var match = IdRegex.Match(data);

                    if (match.Success)
                    {
                        string id = match.Groups["id"].Value;

                        Console.WriteLine("Found id: {0}", id);

                        using var httpclient = new HttpClient();

                        httpclient.DefaultRequestHeaders.Referrer = new Uri($"{LIVE}/{id}");
                        httpclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0");

                        using var resp = await httpclient.GetStreamAsync(new Uri($"{API}/room/v1/Room/playUrl?cid={id}&platform=web"));
                        using var json = await JsonDocument.ParseAsync(resp);

                        //Get the main stream url
                        var url = new Uri(json.RootElement.GetProperty("data").GetProperty("durl").EnumerateArray().First().GetProperty("url").GetString());

                        using var livestream = await httpclient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                        if (livestream.IsSuccessStatusCode)
                        {
                            //This is the live stream
                            using var bili = await livestream.Content.ReadAsStreamAsync();

                            //Send HTTP response headers
                            stream.Write(Encoding.GetBytes("HTTP/1.1 200\r\n"));
                            foreach (var pair in livestream.Headers)
                            {
                                //Copy headers from Bilibili
                                stream.Write(Encoding.GetBytes($"{pair.Key}: {string.Join(' ', pair.Value)}\r\n"));
                            }
                            stream.Write(Encoding.GetBytes("\r\n"));
                            stream.Flush();

                            //Send HTTP response body
                            while (true)
                            {
                                count = bili.Read(buffer, 0, buffer.Length);

                                if (count == 0)
                                {
                                    break;
                                }

                                stream.Write(buffer, 0, count);
                                stream.Flush();
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Exception: {0}", e);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Client Disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                stream.Close();

                (obj as TcpClient)?.Close();
                (obj as TcpClient)?.Dispose();

                buffer = null;

                GC.Collect();
            }
        }
    }
}
