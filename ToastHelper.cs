using System.Runtime.InteropServices;

namespace _2.sınıf_2._dönem_projesi
{
    public static class ToastHelper
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void ShellExecute(int hwnd, string op, string file,
                                                 string param, string dir, int show);

        public static void Goster(string baslik, string mesaj)
        {
            // WinForms NotifyIcon ile sistem bildirimi
            var notify = new System.Windows.Forms.NotifyIcon();
            notify.Icon = System.Drawing.SystemIcons.Information;
            notify.Visible = true;
            notify.BalloonTipTitle = baslik;
            notify.BalloonTipText = mesaj;
            notify.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            notify.ShowBalloonTip(4000); // 4 saniye göster

            // Belirli süre sonra temizle
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000;
            timer.Tick += (s, e) =>
            {
                notify.Visible = false;
                notify.Dispose();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
    }
}