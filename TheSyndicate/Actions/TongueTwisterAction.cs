using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheSyndicate.Actions
{
    public class TongueTwisterAction : IAction
    {
        string targetPhrase;
        string result;
        static List<string> phrases = new List<string>()
        { "Fred fed Ted bread, and Ted fed Fred bread",
        "The quick brown fox jumps over a lazy dog"};
        private TextToSpeech tts = new TextToSpeech();


        public TongueTwisterAction()
        {
           
        }

        public bool DidPlayerSucceed()
        {
            string formattedResult = Regex.Replace(result.ToLower(), @"[^\w\s]", "");
            string formattedTargetPhrase = Regex.Replace(targetPhrase.ToLower(), @"[^\w\s]", "");
            return formattedResult.Contains(formattedTargetPhrase);
        }

        public async Task ExecuteActionAsync()
        {
            SetTargetPhrase();
            Console.Clear();
            string instruction = $"Please say '{targetPhrase}' to the microphone.";
            Console.WriteLine(instruction);
            //tts.HearText(instruction);
           
            Console.WriteLine("Press ENTER when you are ready");
            Console.ReadLine();
            result = await SpeechToText.RecognizeSpeechAsync();
            if (DidPlayerSucceed())
            {
                Console.WriteLine("You won");
            }
            else
            {
                Console.WriteLine("You lost");
            }
            Console.WriteLine(result);
            Console.WriteLine("Press ENTER to return.");
            Console.ReadLine();

        }

        public int GetIndexOfDestinationBasedOnUserSuccessOrFail()
        {
            throw new NotImplementedException();
        }


        private void SetTargetPhrase()
        {
            Random rd = new Random();
            int targetPhraseIndex = rd.Next(0, phrases.Count);
            targetPhrase = phrases[targetPhraseIndex];
        }
    }
}
