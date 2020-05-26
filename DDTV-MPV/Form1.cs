using Mpv.NET.Player;
using System;
using System.Windows.Forms;

namespace DDTV_MPV
{
    public partial class Form1 : Form
    {
        private const int OFFSET = 5;

        private MpvPlayer player;
        private string id;
        private string platform;

        public Form1(string platform, string id)
        {
            this.platform = platform;
            this.id = id;
            InitializeComponent();

            player = new MpvPlayer(Handle)
            {
                Volume = 50,
            };

            Load += Form1_Load;
            FormClosing += Form1_FormClosing;
            MouseDown += Form1_MouseDown;
            MouseWheel += ChangeVolume;
            KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    player.Stop();
                    Close();
                    break;
                case Keys.T:
                    TopMost = !TopMost;
                    break;
                case Keys.Add:
                case Keys.VolumeUp:
                    if (player.Volume + OFFSET < 1)
                    {
                        player.Volume += OFFSET;
                    }
                    break;
                case Keys.Subtract:
                case Keys.VolumeDown:
                    if (player.Volume - OFFSET > 0)
                    {
                        player.Volume -= OFFSET;
                    }
                    break;
            }
        }

        private void ChangeVolume(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (player.Volume + OFFSET < 100)
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

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            switch (platform)
            {
                case "a":
                case "A":
                case "ac":
                case "AC":
                case "AcFun":
                case "ACFun":
                    player.Load($@"{AcFunLiveServer.Server.Address}/{id}");
                    break;
                case "b":
                case "B":
                case "bili":
                    case "Bili":
                case "bilibili":
                case "Bilibili":
                case "BiliBili":
                    player.Load($@"{BilibiliLiveServer.Server.Address}/{id}");
                    break;
                default:
                    Close();
                    break;
            }
            player.Resume();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            player.Dispose();
        }

    }
}
