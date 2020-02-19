﻿using System;
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
             "What is always old and sometimes new; never sad, sometimes blue; never empty, but sometimes full; never pushes, always pulls?","What gets wetter and wetter the more it dries?","You answer me, although I never ask you questions. What am I?","What word is always pronounced wrong?","What is greater than God, more evil than the devil, the poor have it, the rich need it, and if you eat it, you'll die?","Which creature walks on four legs in the morning, two legs in the afternoon, and three legs in the evening?"
        };
        public Dictionary<string, string> riddleAnswerPairs = new Dictionary<string, string>()
        {
            { "What is always old and sometimes new; never sad, sometimes blue; never empty, but sometimes full; never pushes, always pulls?","moon" },
            {"What gets wetter and wetter the more it dries?", "Towel" },{"You answer me, although I never ask you questions. What am I?","Telephone"},{"What word is always pronounced wrong?","Wrong"},{"What is greater than God, more evil than the devil, the poor have it, the rich need it, and if you eat it, you'll die?","Nothing"},{"Which creature walks on four legs in the morning, two legs in the afternoon, and three legs in the evening?","Man"}
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
            //Console.WriteLine(instruction);
            //tts.HearText(instruction);
            TextBox instructions = new TextBox(instruction, Program.WINDOW_WIDTH / 2, 2, Program.WINDOW_WIDTH / 4, Program.WINDOW_HEIGHT / 3);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(instruction);

            while (guessesRemaining > 0)
            {
                Console.SetCursorPosition(instructions.TextBoxX, instructions.TextBoxY+10);
                Console.WriteLine($"You have {guessesRemaining} chances remaining. Press ENTER when you are ready to answer");
                Console.ReadLine();
                //result = await SpeechToText.RecognizeSpeechAsync();
                result = "wrong";
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


            Console.SetCursorPosition(instructions.TextBoxX, instructions.TextBoxY + 10);
            Console.WriteLine(new string(' ', Program.WINDOW_WIDTH));
       

            Console.SetCursorPosition((Program.WINDOW_WIDTH - 50) / 2, instructions.TextBoxY + 10);
            if (DidPlayerSucceed())
            {
                Console.Write("You won, +5 points. ");
            }
            else
            {
                Console.Write("You lost, -5 points. ");
            }
            //Console.WriteLine(result);
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
