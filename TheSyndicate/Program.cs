using System;
using System.Runtime.InteropServices;
using TheSyndicate.Actions;

namespace TheSyndicate
{
    class Program
    {

        public static int WINDOW_WIDTH = 140;
        public static int WINDOW_HEIGHT = 68;
        public static string ASSETS_PATH = "";

        static void Main(string[] args)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ConsoleWindow.ShowWindow(ConsoleWindow.ThisConsole, ConsoleWindow.MAXIMIZE);
                ConsoleWindow.SetWindowsWindow();
                ASSETS_PATH = @"..\..\..\assets\";
            }
            else
            {
                ConsoleWindow.SetMacWindow();
                ASSETS_PATH = @"../../../assets/";
            }
            GameEngine gameEngine = new GameEngine();
            gameEngine.Start();
            //Console.WriteLine(Program.WINDOW_HEIGHT);
        }
    }
}


