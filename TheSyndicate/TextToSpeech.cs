// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

// <code>
using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using System.Text.RegularExpressions;
using System.Linq;

namespace TheSyndicate
{
    class TextToSpeech
    {
        public string text { get; set; }

        public async void HearText(string words)
        {
            this.text = words;
            CleanText();
            SynthesisToSpeakerAsync().Wait();
        }

        private void CleanText()
        {
            Regex rgx = new Regex(@"([a-zA-Z0-9,?.']+)");
            MatchCollection matches = rgx.Matches(this.text);

            var textArray =from Match m in matches select m.Value;
            this.text = string.Join(' ', textArray).Replace(':','.');
            
            
        }

        private async Task SynthesisToSpeakerAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("0c685477e7a34e9a98bb25d61c815137", "westus2");
            config.SpeechSynthesisVoiceName = "en-GB-George-Apollo"; //Voice options available at: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech
            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                // Receive a text from console input and synthesize it to speaker.
                //Console.WriteLine("Type some text that you want to speak...");
                //Console.Write("> ");
                //string text = Console.ReadLine();

                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        //Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        //Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            //Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            //Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            //Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }


    }
}
