using System;
using System.Runtime.InteropServices;
using TheSyndicate.Actions;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace TheSyndicate
{
    class Program
    {

        public static int WINDOW_WIDTH = 140;
        public static int WINDOW_HEIGHT = 68;
        public static string ASSETS_PATH = "";
        public static async Task RecognizeSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription("0c685477e7a34e9a98bb25d61c815137", "westus2");

            using (var recognizer = new SpeechRecognizer(config))
            {
                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }
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
                ASSETS_PATH = @"../../../../assets/";
            }
            //RecognizeSpeechAsync().Wait();
            //Console.WriteLine("Please press <Return> to continue.");
            //Console.ReadLine();
            GameEngine gameEngine = new GameEngine();
            gameEngine.Start();
            //Console.WriteLine(Program.WINDOW_HEIGHT);
        }
    }

}


