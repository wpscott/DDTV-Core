using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AcFunLiveServer
{
    public static class Server
    {
        private const string _HOST = "https://m.acfun.cn";
        private static readonly Uri HOST = new Uri(_HOST);
        private const string LIVE_URL = "https://m.acfun.cn/live/detail";
        private const string LOGIN_URL = "https://id.app.acfun.cn/rest/app/visitor/login";
        private static readonly Uri LOGIN_URI = new Uri(LOGIN_URL);
        private const string PLAY_URL = "https://api.kuaishouzt.com/rest/zt/live/web/startPlay?subBiz=mainApp&kpn=ACFUN_APP&kpf=OUTSIDE_IOS_H5&userId={0}&did={1}&acfun.api.visitor_st={2}";

        private const string UserAgent = "Mozilla/5.0 (iPad; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";

        private const int Port = 62114;
        private static readonly Encoding Encoding = Encoding.UTF8;
        private static readonly HttpListener server;

        private static readonly CookieContainer Cookies;
        private static readonly HttpClient client;

        private static readonly Dictionary<string, string> LOGIN_FORM = new Dictionary<string, string> { { "sid", "acfun.api.visitor" } };
        private static readonly FormUrlEncodedContent Content = new FormUrlEncodedContent(LOGIN_FORM);

        public static readonly string Address = $"http://{IPAddress.Loopback}:{Port}";

        // Config Http Server
        static Server()
        {
            server = new HttpListener();
            server.Prefixes.Add($"{Address}/");

            Cookies = new CookieContainer();

            client = new HttpClient(
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.All,
                    UseCookies = true,
                    CookieContainer = Cookies
                }
            );
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent); // Mobile only
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
        }

        public static async Task Start()
        {
            server.Start();

            using var index = await client.GetAsync(HOST); // Get cookies for rest requests
            if (!index.IsSuccessStatusCode)
            {
                Console.WriteLine(await index.Content.ReadAsStringAsync());
                return;
            }
            var devideId = Cookies.GetCookies(HOST).Where(cookie => cookie.Name == "_did").First().Value;

            using var login = await client.PostAsync(LOGIN_URI, Content); // Get visitor id and service token for rest requests
            if (!login.IsSuccessStatusCode)
            {
                Console.WriteLine(await login.Content.ReadAsStringAsync());
                return;
            }

            using var loginData = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
            var userId = loginData.RootElement.GetProperty("userId").ToString();
            var serviceToken = loginData.RootElement.GetProperty("acfun.api.visitor_st").ToString();

            Console.WriteLine("AcFun live server is ready");
            while (true)
            {
                var context = await server.GetContextAsync();

                var req = context.Request;
                using var resp = context.Response;

                try
                {
                    var path = req.Url.LocalPath.Substring(1);
                    if (long.TryParse(path, out var uid)) // Get user id
                    {
                        Console.WriteLine("Found user id: {0}", uid);

                        var liveUri = new Uri($"{LIVE_URL}/{uid}");

                        var form = new Dictionary<string, string> { { "authorId", $"{uid}" } };
                        using var post = new FormUrlEncodedContent(form);
                        using var play = await client.PostAsync(  // Get stream url
                            string.Format(
                                PLAY_URL,
                                userId,
                                devideId,
                                serviceToken
                             ),
                            post
                            );

                        if (!play.IsSuccessStatusCode)
                        {
                            WriteContent(uid, resp, 400, await play.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            using var playData = await JsonDocument.ParseAsync(await play.Content.ReadAsStreamAsync());
                            if (playData.RootElement.GetProperty("result").GetInt32() != 1)
                            {
                                WriteContent(uid, resp, 404, playData.RootElement.GetProperty("error_msg").GetString());
                            }
                            else
                            {
                                using var playRes = JsonDocument.Parse(
                                    playData.RootElement
                                    .GetProperty("data")
                                    .GetProperty("videoPlayRes")
                                    .ToString()
                                    );

                                //redirect to stream url (highest bitrate)
                                resp.StatusCode = 302;
                                resp.RedirectLocation = playRes.RootElement
                                    .GetProperty("liveAdaptiveManifest")
                                    .EnumerateArray().First()
                                    .GetProperty("adaptationSet")
                                    .GetProperty("representation")
                                    .EnumerateArray()
                                    .OrderByDescending(rep => rep.GetProperty("bitrate").GetInt32())
                                    .First()
                                    .GetProperty("url")
                                    .ToString();
                            }
                        }
                    }
                    else { WriteContent(uid, resp, 400, $"Invalid Request: {path}"); }
                }
                catch (HttpRequestException e) { WriteContent(resp, e); }
                catch (JsonException e) { WriteContent(resp, e); }
                catch (KeyNotFoundException e) { WriteContent(resp, e); }
                catch (System.IO.IOException e) { WriteContent(resp, e); }
                catch (Exception e) { WriteContent(resp, e); }
                finally
                {
                    resp.Close();

                    GC.Collect();
                }
            }
        }

        internal static void WriteContent(HttpListenerResponse resp, Exception e)
        {
            Console.WriteLine("Unhandled Exception: {0}", e);
            WriteContent(resp, 500, e.Message);
        }

        internal static void WriteContent(long uid, HttpListenerResponse resp, int code, string msg)
        {
            Console.WriteLine("Id: {0} not available, reason: {1}", uid, msg);
            WriteContent(resp, code, msg);
        }

        internal static void WriteContent(HttpListenerResponse resp, int code, string msg)
        {
            resp.ContentEncoding = Encoding;

            resp.StatusCode = code;
            var buffer = Encoding.GetBytes(msg);
            resp.ContentLength64 = buffer.Length;
            resp.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
