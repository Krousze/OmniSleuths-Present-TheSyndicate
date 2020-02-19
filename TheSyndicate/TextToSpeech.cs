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
using System.Threading;

namespace TheSyndicate
{
    class TextToSpeech
    {
        private string key = "cdd4859020d94d1ab919ad18e313cab5";
        public string text { get; set; }
        public Queue<Dictionary<string, string>> q { get; private set; } = new Queue<Dictionary<string, string>>();
        string[] voices = new string[] { "en-US-ZiraRUS", "en-GB-George-Apollo", "en-GB-Susan-Apollo", "en-US-BenjaminRUS", "en-US-JessaNeural" };
        Dictionary<string, string> voiceBank = new Dictionary<string, string>() { { "Narrator", "en-GB-Susan-Apollo" }, { "C.R.A.I.G.", "en-GB-George-Apollo" }, { "B.A.W.S. 5000", "en-US-ZiraRUS" }, { "Dog", "en-US-BenjaminRUS" }, { "C.R.A.I.G. unit", "en-US-JessaNeural" }, { "Destroyer", "en-IE-Sean" } };

        //public async void HearText(string words)
        //{
        //    this.text = words;
        //    CleanText();
        //    SynthesisToSpeakerAsync().Wait();
        //}

        public async Task HearText(Dictionary<string, string>[] words)
        {
            foreach (var dict in words)
            {
                this.q.Enqueue(dict);

            }
            using (var cts = new CancellationTokenSource())
            {
                var keyBoardTask = Task.Run(() =>
                {
                    int cursorX = 0;
                    int cursorY = Program.WINDOW_HEIGHT - 3;
                    Console.SetCursorPosition(cursorX, cursorY);
                    Console.WriteLine("Press 's' to cancel active Text to Speech");
                    char ch;
                    do
                    {
                        ch = Console.ReadKey(true).KeyChar;

                        if (ch == 's')
                        {
                            // Cancel the task
                            Console.WriteLine("active Text to Speech Cancelling");
                            cts.Cancel();

                        }
                       
                    } while (ch != 's');

                });

                try
                {
                    await GetSpeechAsync(cts.Token);

                }
                catch (Exception e)
                {
                }
                keyBoardTask.Wait();

            }


            //while (q.Count > 0)
            //{
            //    var actor = q.Dequeue();
            //    string spkr = actor["Speaker"];
            //    string dlog = actor["Dialogue"];
            //    SynthesisToSpeakerAsync(spkr, dlog).Wait();
            //}
        }

        private async Task GetSpeechAsync(CancellationToken cToken)
        {

            while (q.Count > 0)
            {
                if (cToken.IsCancellationRequested)
                {
                    q.Clear();
                    Console.SetCursorPosition(0, Program.WINDOW_HEIGHT - 3);
                    Console.WriteLine("                                          ");
                    Console.WriteLine("                                          ");
                    throw new TaskCanceledException("Canceled Thread");
                }
                var actor = q.Dequeue();
                string spkr = actor["Speaker"];
                string dlog = actor["Dialogue"];
                SynthesisToSpeakerAsync(spkr, dlog).Wait();
            }
            //await SynthesisToSpeakerAsync();

        }
        public async Task SynthesisToSpeakerAsync(string actor, string actorWords)
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
