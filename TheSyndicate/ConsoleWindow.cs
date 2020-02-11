using System;
using System.Runtime.InteropServices;

namespace TheSyndicate
{


    class ConsoleWindow
    {



        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        public const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;


        [DllImport("libc")]
        private static extern int system(string exec);

        public static void SetMacWindow()
        {
            system(@"printf '\e[8;68;140t'");
        }

        public static void SetWindowsWindow()
        {
            Program.WINDOW_WIDTH = Console.WindowWidth;
            Program.WINDOW_HEIGHT = Console.WindowHeight;
        }

    }
}
