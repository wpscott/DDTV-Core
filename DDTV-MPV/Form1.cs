using Mpv.NET.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

        const int WM_NCHITTEST = 0x0084;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_SIZING = 0x214;
        const int WM_NCLBUTTONDOWN = 0xA1;

        const float ConstantWidth = 16f;
        const float ConstantHeight = 10f;

        const int WMSZ_LEFT = 1;
        const int WMSZ_RIGHT = 2;
        const int WMSZ_TOP = 3;
        const int WMSZ_BOTTOM = 6;

        const int HT_LEFT = 10;
        const int HT_RIGHT = 11;
        const int HT_BOTTOM_RIGHT = 17;
        const int HT_BOTTOM = 15;
        const int HT_BOTTOM_LEFT = 16;
        const int HT_TOP = 12;
        const int HT_TOP_LEFT = 13;
        const int HT_TOP_RIGHT = 14;
        const int HT_CAPTION = 0x2;

        const int RESIZE_HANDLE_SIZE = 10;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void WndProc(ref Message m)
        {
            bool handled = false;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                case WM_MOUSEMOVE:
                    var formSize = Size;
                    var screenPoint = new Point(m.LParam.ToInt32());
                    var clientPoint = PointToClient(screenPoint);

                    var boxes = new Dictionary<int, Rectangle>()
                    {
                        {HT_BOTTOM_LEFT, new Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                        {HT_BOTTOM, new Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2 * RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                        {HT_BOTTOM_RIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                        {HT_RIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE) },
                        {HT_TOP_RIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                        {HT_TOP, new Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                        {HT_TOP_LEFT, new Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
                        {HT_LEFT, new Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE) }
                    };

                    var hitbox = boxes.Where(hitbox => hitbox.Value.Contains(clientPoint)).FirstOrDefault();
                    if (hitbox.Key > 0)
                    {
                        m.Result = (IntPtr)hitbox.Key;
                        handled = true;
                    }
                    break;
                //case WM_SIZING:
                //    var rc = Marshal.PtrToStructure<Rectangle>(m.LParam);

                //    switch (m.WParam.ToInt32())
                //    {
                //        case WMSZ_LEFT:
                //        case WMSZ_RIGHT:
                //            rc.Height = (int)(ConstantHeight * rc.Width / ConstantWidth);
                //            break;
                //        case WMSZ_TOP:
                //        case WMSZ_BOTTOM:
                //            rc.Width = (int)(ConstantWidth * rc.Height / ConstantHeight);
                //            break;
                //        case WMSZ_LEFT + WMSZ_TOP:
                //        case WMSZ_RIGHT + WMSZ_TOP:
                //            rc.Height = (int)(ConstantHeight * rc.Width / ConstantWidth);
                //            break;
                //        case WMSZ_LEFT + WMSZ_BOTTOM:
                //        case WMSZ_RIGHT + WMSZ_BOTTOM:
                //            rc.Width = (int)(ConstantWidth * rc.Height / ConstantHeight);
                //            break;
                //    }
                //    Marshal.StructureToPtr(rc, m.LParam, true);
                //    break;
            }

            if (!handled)
            {
                base.WndProc(ref m);
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
