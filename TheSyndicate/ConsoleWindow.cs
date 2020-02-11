//using System;
//using System.Runtime.InteropServices;

<<<<<<< HEAD
//namespace TheSyndicate
//{
//    class ConsoleWindow
//    {
//        [DllImport("kernel32.dll", ExactSpelling = true)]
=======
namespace TheSyndicate
{


    class ConsoleWindow
    {



        [DllImport("kernel32.dll", ExactSpelling = true)]
>>>>>>> master

//        private static extern IntPtr GetConsoleWindow();
//        public static IntPtr ThisConsole = GetConsoleWindow();

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

<<<<<<< HEAD
//        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
//        private const int HIDE = 0;
//        public const int MAXIMIZE = 3;
//        private const int MINIMIZE = 6;
//        private const int RESTORE = 9;
//    }
//}
=======
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
>>>>>>> master
