using System;
using Gtk;

namespace WSDBR
{
    public class Program
    {
        public static void Main()
        {
            Application.Init();
            MainWindow.Instance = new MainWindow();
            MainWindow.Instance.Show();
            Application.Run();
        }

        public static DateTime GetWhatsAppDateTime(long ticks)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            return dt.AddMilliseconds(ticks);
        }
    }
}