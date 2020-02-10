using System;
using System.Runtime.InteropServices;
using TheSyndicate.Actions;

namespace TheSyndicate
{
    class Program
    {
        public const int WINDOW_WIDTH = 140;
        public const int WINDOW_HEIGHT = 68;

        static void Main(string[] args)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //ConsoleWindow.ShowWindow(ConsoleWindow.ThisConsole, ConsoleWindow.MAXIMIZE);
                ConsoleWindow.SetWindowsWindow();
            }
            else
            {
                Console.WriteLine(Program.WINDOW_HEIGHT + ", " + Program.WINDOW_WIDTH);
                ConsoleWindow.SetMacWindow();
                Console.WriteLine(Program.WINDOW_HEIGHT + ", " + Program.WINDOW_WIDTH);
                Console.ReadKey();
            }


            GameEngine gameEngine = new GameEngine();
            gameEngine.Start();
        }
    }
}


