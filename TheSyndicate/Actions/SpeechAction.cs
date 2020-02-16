using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheSyndicate.Actions
{
    public class SpeechAction : IAction
    {
        string targetPhrase;
        string result;
        static List<string> phrases = new List<string>()
        { "I am a robot", "The quick brown fox jumps over the lazy dog " };

        public SpeechAction()
        {
            SetTargetPhrase();
        }

        public bool DidPlayerSucceed()
        {
            return result.ToLower().Contains(targetPhrase.ToLower());
        }

        public async Task ExecuteActionAsync()
        {
            Console.Clear();
            Console.WriteLine($"Please say '{targetPhrase}' to the microphone.");
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
