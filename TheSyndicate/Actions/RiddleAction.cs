using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheSyndicate.Actions
{
    public class RiddleAction : IAction
    {
        string targetRiddle;
        string result;
        int guessesRemaining;
        public List<string> riddles = new List<string>()
        {
             "What is always old and sometimes new; never sad, sometimes blue; never empty, but sometimes full; never pushes, always pulls?"
        };
        public Dictionary<string, string> riddleAnswerPairs = new Dictionary<string, string>()
        {
            { "What is always old and sometimes new; never sad, sometimes blue; never empty, but sometimes full; never pushes, always pulls?","moon" }
        };

        public RiddleAction()
        {
           
        }

        private void SetTargetRiddle()
        {
            Random rd = new Random();
            int targetRiddleIndex = rd.Next(0, riddles.Count);
            targetRiddle = riddles[targetRiddleIndex];
        }

        public async Task ExecuteActionAsync()
        {
            guessesRemaining = 3;
            SetTargetRiddle();

            Console.Clear();
            string instruction = $"Here's my riddle: '{targetRiddle}' ";
            Console.WriteLine(instruction);
            //tts.HearText(instruction);

            while (guessesRemaining > 0)
            {
                Console.WriteLine($"You have {guessesRemaining} chances remaining. What's your answer?");
                Console.WriteLine("Press ENTER when you are ready");
                Console.ReadLine();
                result = await SpeechToText.RecognizeSpeechAsync();
                if (!DidPlayerSucceed())
                {
                    guessesRemaining--;
                    continue;
                }
                else
                {
                    break;
                }
            } 
            
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

        public bool DidPlayerSucceed()
        {
            string formattedResult = Regex.Replace(result.ToLower(), @"[^\w\s]", "");
            string formattedCorrectAnswer = Regex.Replace(riddleAnswerPairs[targetRiddle].ToLower(), @"[^\w\s]", "");
            return formattedResult.Contains(formattedCorrectAnswer);
        }
    }
}
