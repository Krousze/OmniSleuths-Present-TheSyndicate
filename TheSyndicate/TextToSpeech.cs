// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

// <code>
using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace TheSyndicate
{
    class TextToSpeech
    {
        private string key = "0c685477e7a34e9a98bb25d61c815137";
        public string text { get; set; }
        public Queue<Dictionary<string, string>> q { get; private set; } = new Queue<Dictionary<string, string>>();
        string[] voices = new string[] { "en-US-ZiraRUS" ,"en-GB-George-Apollo","en-GB-Susan-Apollo", "en-US-BenjaminRUS","en-US-JessaNeural" };
        Dictionary<string, string> voiceBank = new Dictionary<string, string>() { { "Narrator", "en-GB-Susan-Apollo" },{ "C.R.A.I.G.", "en-GB-George-Apollo" }, { "B.A.W.S. 5000", "en-US-ZiraRUS" },{ "Dog","en-US-BenjaminRUS"}, { "C.R.A.I.G. unit", "en-US-JessaNeural" } };

        public async void HearText(string words)
        {
            this.text = words;
            CleanText();
            SynthesisToSpeakerAsync().Wait();
        }
        public async void HearText(Dictionary<string, string>[] words)
        {
            foreach (var dict in words)
            {
                this.q.Enqueue(dict);

            }

            while (q.Count > 0)
            {
                var actor = q.Dequeue();
                string spkr = actor["Speaker"];
                string dlog = actor["Dialogue"];
                SynthesisToSpeakerAsync(spkr,dlog).Wait();
            }
        }

        private void CleanText()
        {
            Regex rgx = new Regex(@"([a-zA-Z0-9,?.']+)");
            MatchCollection matches = rgx.Matches(this.text);

            var textArray = from Match m in matches select m.Value;
            this.text = string.Join(' ', textArray).Replace(':', '.');


        }

        private async Task SynthesisToSpeakerAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription(key, "westus2");
            config.SpeechSynthesisVoiceName = "en-AU-HayleyRUS"; //Voice options available at: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech

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
        private async Task SynthesisToSpeakerAsync(string actor, string actorWords)
        {

            var config = SpeechConfig.FromSubscription(key, "westus2");
            config.SpeechSynthesisVoiceName = voiceBank[actor]; //Voice options available at: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech
            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                // Receive a text from console input and synthesize it to speaker.


                using (var result = await synthesizer.SpeakTextAsync(actorWords))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);

                        if (cancellation.Reason == CancellationReason.Error)
                        {

                        }
                    }
                }
            }
        }


    }
}
