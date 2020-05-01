using FFmpeg.AutoGen;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Unosquare.FFME.Common;

namespace DDTV_FFmpeg
{
    public partial class FFmpegWindow : Window
    {
        private const double OFFSET = 0.05;
        private const string HOST = "live.bilibili.com";
        private const string LIVE = "https://" + HOST;
        private const string API = "https://api." + HOST;
        private readonly string id;

        public FFmpegWindow(string id)
        {
            this.id = id;
            InitializeComponent();

            player.Volume = 0.5;

            Loaded += MainWindow_Loaded;

            player.MediaClosed += (s, e) => Close();
            player.MediaEnded += (s, e) => Close();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using var client = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
            });

            //client.DefaultRequestHeaders.Add("Origin", LIVE);
            client.DefaultRequestHeaders.Referrer = new Uri($"{LIVE}/{id}");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0");

            var resp = await client.GetStreamAsync(new Uri($"{API}/room/v1/Room/playUrl?cid={id}&platform=web")).ConfigureAwait(false);
            var json = await JsonDocument.ParseAsync(resp).ConfigureAwait(false);

            var urls = json.RootElement.GetProperty("data").GetProperty("durl").EnumerateArray().ToArray();
            var url = new Uri(json.RootElement.GetProperty("data").GetProperty("durl").EnumerateArray().First().GetProperty("url").GetString());

            var stream = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            var bili = await stream.Content.ReadAsStreamAsync().ConfigureAwait(false);

            await player.Open(new BiliSource(id, bili));

        }

        private void ChangeVolume(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (player.Volume + OFFSET < 1)
                {
                    player.Volume += OFFSET;
                }
            }
            else
            {
                if (player.Volume - OFFSET > 0)
                {
                    player.Volume -= OFFSET;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    player.Stop();
                    Close();
                    break;
                case Key.T:
                    Topmost = !Topmost;
                    break;
                case Key.Add:
                case Key.VolumeUp:
                    if (player.Volume + OFFSET < 1)
                    {
                        player.Volume += OFFSET;
                    }
                    break;
                case Key.Subtract:
                case Key.VolumeDown:
                    if (player.Volume - OFFSET > 0)
                    {
                        player.Volume -= OFFSET;
                    }
                    break;
                case Key.M:
                case Key.VolumeMute:
                    player.IsMuted = !player.IsMuted;
                    break;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }

    sealed unsafe class BiliSource : IMediaInputStream
    {
        private Stream Stream;
        private readonly object Lock = new object();
        private byte[] Buffer;

        public BiliSource(string id, Stream stream)
        {
            StreamUri = new Uri($"bili://live/{id}");
            Stream = stream;
            Buffer = new byte[ReadBufferLength];

            OnInitializing += (config, source) => Debug.WriteLine("Initializing");
            OnInitialized += (format, context, info) => Debug.WriteLine("Initialized");
        }

        public Uri StreamUri { get; }

        public bool CanSeek => false;

        public int ReadBufferLength => 1 << 16;

        public InputStreamInitializing OnInitializing { get; }

        public InputStreamInitialized OnInitialized { get; }

        public int Read(void* opaque, byte* targetBuffer, int targetBufferLength)
        {
            lock (Lock)
            {
                try
                {
                    var count = Stream.Read(Buffer, 0, Buffer.Length);
                    if (count > 0)
                    {
                        Marshal.Copy(Buffer, 0, (IntPtr)targetBuffer, count);
                    }
                    return count;
                }
                catch (Exception)
                {
                    return ffmpeg.AVERROR_EOF;
                }
            }
        }

        public long Seek(void* opaque, long offset, int whence)
        {
            lock (Lock)
            {
                try
                {
                    return whence == ffmpeg.AVSEEK_SIZE ? Stream.Length : Stream.Seek(offset, SeekOrigin.Begin);
                }
                catch
                {
                    return ffmpeg.AVERROR_EOF;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stream?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
