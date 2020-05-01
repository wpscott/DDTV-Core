using System.IO;
using System.Windows;
using Unosquare.FFME;
using FFmpeg.AutoGen.Native;
using BilibiliLiveServer;
using System.Threading.Tasks;

namespace DDTV_FFmpeg
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Library.FFmpegDirectory = @".\ffmpeg";
            Library.LoadFFmpeg();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var wmd = new FFmpegWindow(e.Args[0]);
            wmd.Show();
        }
    }
}
