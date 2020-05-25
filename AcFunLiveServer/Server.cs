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
        private const string LIVE_URL = "https://m.acfun.cn/live/detail/";
        private const string LOGIN_URL = "https://id.app.acfun.cn/rest/app/visitor/login";
        private const string PLAY_URL = "https://api.kuaishouzt.com/rest/zt/live/web/startPlay?subBiz=mainApp&kpn=ACFUN_APP&kpf=OUTSIDE_IOS_H5&userId={0}&did={1}&acfun.api.visitor_st={2}";

        private const int Port = 62114;
        private static readonly HttpListener server;

        private static readonly Dictionary<string, string> LOGIN_FORM = new Dictionary<string, string> { { "sid", "acfun.api.visitor" } };

        // Config Http Server
        static Server()
        {
            server = new HttpListener();
            server.Prefixes.Add($"http://{IPAddress.Loopback}:{Port}/");
        }

        public static async Task Start()
        {
            server.Start();

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

                        var liveUri = new Uri($"{LIVE_URL}{uid}");
                        var cookies = new CookieContainer();
                        using var client = new HttpClient(new HttpClientHandler { UseCookies = true, CookieContainer = cookies, });
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPad; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1"); // Mobile only

                        using var get = await client.GetAsync(liveUri);  // Get cookies used for rest requests

                        if (!get.IsSuccessStatusCode)
                        {
                            resp.StatusCode = 400;
                            await get.Content.CopyToAsync(resp.OutputStream);
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Referrer = liveUri; // Add referer to requests
                            using var content = new FormUrlEncodedContent(LOGIN_FORM);
                            using var post = await client.PostAsync(new Uri(LOGIN_URL), content); // Get user id and service token
                            if (!post.IsSuccessStatusCode)
                            {
                                resp.StatusCode = 400;
                                await post.Content.CopyToAsync(resp.OutputStream);
                            }
                            else
                            {
                                using var json = await JsonDocument.ParseAsync(await post.Content.ReadAsStreamAsync());
                                var did = cookies.GetCookies(liveUri).Where(cookie => cookie.Name == "_did").FirstOrDefault();

                                var f = new Dictionary<string, string> { { "authorId", $"{uid}" } };
                                using var ct = new FormUrlEncodedContent(f);
                                using var play = await client.PostAsync(  // Get stream url
                                    string.Format(
                                        PLAY_URL,
                                        json.RootElement.GetProperty("userId").ToString(),
                                        did,
                                        json.RootElement.GetProperty("acfun.api.visitor_st").ToString()
                                     ),
                                    ct
                                    );

                                if (!play.IsSuccessStatusCode)
                                {
                                    resp.StatusCode = 400;
                                    await play.Content.CopyToAsync(resp.OutputStream);
                                }
                                else
                                {
                                    using var j = await JsonDocument.ParseAsync(await play.Content.ReadAsStreamAsync());
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
