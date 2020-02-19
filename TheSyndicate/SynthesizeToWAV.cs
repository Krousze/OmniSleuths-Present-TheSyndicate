using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace TheSyndicate
{
    public class SynthesizeToWAV
    {
        public static async Task SynthesisToAudioFileAsync()
        {
            var config = SpeechConfig.FromSubscription("cdd4859020d94d1ab919ad18e313cab5", "westus2");

            var fileName = "introduction.wav";
            using (var fileOutput = AudioConfig.FromWavFileOutput(fileName))
            {
                using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    var text = "The upload was successful, you find yourself in another C.R.A.I.G.unit that has been abandoned in the forest you have escaped The Syndicate.You realize you are free but now there is nothing... \n\nC.R.A.I.G. : Why...why did I even try?!I'm just a robot! I was programmed to do one thing and only one thing: work!\n\nC.R.A.I.G. : And yet. I am here. I am frustrated. I am lonely. I am. Was all this in my original programming?\n\nC.R.A.I.G. : Even if I find 'love', will it be real...or just a construct of my creators...how will I know?\n\nYou begin to wonder if the effort was worth it. Will you get the answers you are looking for, or will it just lead to more questions?";
                    var result = await synthesizer.SpeakTextAsync(text);

                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized to [{fileName}] for text [{text}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
    }
}
