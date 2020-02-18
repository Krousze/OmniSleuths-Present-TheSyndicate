using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace TheSyndicate
{
    public class SpeechToText
    {
        public static async Task<string> RecognizeSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription("cdd4859020d94d1ab919ad18e313cab5", "westus2");

            using (var recognizer = new SpeechRecognizer(config))
            {
                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"You said: {result.Text}");
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
                return result.Text;
            }
        }
    }
}
