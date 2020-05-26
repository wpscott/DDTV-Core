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
        private static readonly HttpListener server;
        private static readonly HttpClient client;
        private static readonly CookieContainer Cookies;

        private static readonly Dictionary<string, string> LOGIN_FORM = new Dictionary<string, string> { { "sid", "acfun.api.visitor" } };
        private static readonly FormUrlEncodedContent Content = new FormUrlEncodedContent(LOGIN_FORM);

        // Config Http Server
        static Server()
        {
            server = new HttpListener();
            server.Prefixes.Add($"http://{IPAddress.Loopback}:{Port}/");

            Cookies = new CookieContainer();

            client = new HttpClient(new HttpClientHandler { UseCookies = true, CookieContainer = Cookies });
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent); // Mobile only
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
            var did = Cookies.GetCookies(HOST).Where(cookie => cookie.Name == "_did").First().Value;

            using var login = await client.PostAsync(LOGIN_URI, Content); // Get user id and service token for rest requests
            if (!login.IsSuccessStatusCode)
            {
                Console.WriteLine(await login.Content.ReadAsStringAsync());
                return;
            }

            using var data = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
            var userId = data.RootElement.GetProperty("userId").ToString();
            var serviceToken = data.RootElement.GetProperty("acfun.api.visitor_st").ToString();

            Console.WriteLine("AcFun live server is ready");
            while (true)
            {
                var context = await server.GetContextAsync();

                var req = context.Request;
                using var resp = context.Response;

                try
                {
                    long uid;
                    if (long.TryParse(req.Url.LocalPath.Substring(1), out uid)) // Get user id
                    {
                        Console.WriteLine("Found id: {0}", uid);

                        var liveUri = new Uri($"{LIVE_URL}/{uid}");

                        var form = new Dictionary<string, string> { { "authorId", $"{uid}" } };
                        using var post = new FormUrlEncodedContent(form);
                        using var play = await client.PostAsync(  // Get stream url
                            string.Format(
                                PLAY_URL,
                                userId,
                                did,
                                serviceToken
                             ),
                            post
                            );

                        if (!play.IsSuccessStatusCode)
                        {
                            resp.StatusCode = 400;
                            await play.Content.CopyToAsync(resp.OutputStream);
                        }
                        else
                        {
                            using var j = await JsonDocument.ParseAsync(await play.Content.ReadAsStreamAsync());
                            if(j.RootElement.GetProperty("result").GetInt32() != 1)
                            {
                                resp.StatusCode = 404;
                                resp.OutputStream.Write(Encoding.UTF8.GetBytes(j.RootElement.GetProperty("error_msg").GetString()));
                            }
                            else
                            {
                                using var res = JsonDocument.Parse(j.RootElement.GetProperty("data").GetProperty("videoPlayRes").ToString());

                                //redirect to stream url (highest bitrate)
                                resp.StatusCode = 302;
                                resp.RedirectLocation = res.RootElement
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
                    else
                    {
                        resp.StatusCode = 400;
                        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid Request"));
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("RequestException: {0}", e);
                    resp.StatusCode = 500;
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(e.Message));
                }
                catch (JsonException e)
                {
                    Console.WriteLine("JsonException: {0}", e);
                    resp.StatusCode = 500;
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(e.Message));
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine("KeyNotFoundException: {0}", e);
                    resp.StatusCode = 500;
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(e.Message));
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("IOException: {0}", e);
                    resp.StatusCode = 500;
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(e.Message));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                    resp.StatusCode = 500;
                    resp.OutputStream.Write(Encoding.UTF8.GetBytes(e.Message));
                }
                finally
                {
                    resp.Close();

                    GC.Collect();
                }
            }
        }
    }
}
