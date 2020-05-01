using System.Windows;
using Unosquare.FFME;

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

            if(e.Args.Length == 1)
            {
                var wmd = new FFmpegWindow(e.Args[0]);
                wmd.Show();
            }
            else
            {
                Shutdown(1);
            }   
        }
    }
}
