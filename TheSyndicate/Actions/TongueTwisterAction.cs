﻿using System;
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
        { 
            "Fred fed Ted bread, and Ted fed Fred bread",
            "The quick brown fox jumps ove a lazy dog",
            "How much wood would a woodchuck chuck if a wood chuck could chuck wood?",
            "Susie works in a shoeshine shop. Where she shines she sits, and where she sits she shines",
            "I have got a date at a quarter to eight; I’ll see you at the gate, so don’t be late",
            "Can you can a can as a canner can can a can?"
        };
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
            string instruction = $"Please say '{targetPhrase}', to the microphone. Press ENTER when you are ready.";
            TextBox instructions = new TextBox(instruction, Program.WINDOW_WIDTH / 2, 2, Program.WINDOW_WIDTH / 4, Program.WINDOW_HEIGHT / 3);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(instruction);
            if (GameEngine.UseVoiceInput)
            {
                tts.SynthesisToSpeakerAsync("Narrator", instruction).Wait();
            }
            Console.ReadLine();
            result = await SpeechToText.RecognizeSpeechAsync();
            Console.SetCursorPosition((Program.WINDOW_WIDTH - 50) / 2, instructions.TextBoxY + 7);
            string msg = "";
            if (DidPlayerSucceed())
            {
                msg = "You won, +5 points. ";
            }
            else
            {
                msg = "You lost, -5 points. ";
            }
            msg += "Press ENTER to return";
            Console.WriteLine(msg);
            if (GameEngine.UseVoiceInput)
            {
                tts.SynthesisToSpeakerAsync("Narrator", msg).Wait();
            }
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
