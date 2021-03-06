﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TheSyndicate.Actions
{
    enum SpacebarOrTab { Neither, Spacebar, Tab }

    class KeyPressAction : IAction
    {
        private static int SECONDS_TO_PRESS_KEYS = 3;
        private static int TIMES_KEYS_MUST_BE_PRESSED = 20;
        private static string INSTRUCTIONS = $"In order to successfully complete your action you must alternate pressing the Spacebar and Tab keys at least {TIMES_KEYS_MUST_BE_PRESSED} times in {SECONDS_TO_PRESS_KEYS} seconds.";   
        private Stopwatch Stopwatch { get; set; }
        private ConsoleKey CurrentKeyPressed { get; set; }
        private SpacebarOrTab LastKeyPressed {get; set;}
        private int SpacebarAndTabPresses { get; set; }
        
        public KeyPressAction()
        {
            
        }

        public Task ExecuteActionAsync()
        {
            this.LastKeyPressed = SpacebarOrTab.Neither;
            this.SpacebarAndTabPresses = 0;
            this.Stopwatch = new Stopwatch();
            Console.CursorVisible = false;
            RenderInstructions();
            WaitForPlayerToPressEnter();
            HaveUserAlternatePressingSpacebarAndTab();
            RenderEndMessage();
            Console.CursorVisible = true;
            return Task.Delay(0);
        }
        
        private void RenderInstructions()
        {
            TextBox instructions = new TextBox(INSTRUCTIONS, Program.WINDOW_WIDTH/2, 2, Program.WINDOW_WIDTH / 4, Program.WINDOW_HEIGHT / 2);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(INSTRUCTIONS);
        }

        private void WaitForPlayerToPressEnter()
        {
            string enterPrompt = "Press ENTER to continue.";
            Console.SetCursorPosition(Program.WINDOW_WIDTH/2 - enterPrompt.Length/2, Program.WINDOW_HEIGHT - (Program.WINDOW_HEIGHT/4));
            Console.WriteLine(enterPrompt);
            ConsoleKey userInput = Console.ReadKey(true).Key;
            while (userInput != ConsoleKey.Enter)
            {
                userInput = Console.ReadKey(true).Key;
            }
        }

        // https://stackoverflow.com/questions/5945533/how-to-execute-the-loop-for-specific-time
        private void HaveUserAlternatePressingSpacebarAndTab()
        {
            Console.Clear();
            Console.SetCursorPosition(Program.WINDOW_WIDTH/2 - 4, Program.WINDOW_HEIGHT/2);
            Console.WriteLine("START!!!");
            this.Stopwatch.Start();
            while (this.Stopwatch.Elapsed <= TimeSpan.FromSeconds(SECONDS_TO_PRESS_KEYS))
            {
                if (KeyPressedIsSpacebarOrTab())
                {
                    SpacebarAndTabPresses++;
                    ToggleLastKeyPressed();
                }
            }
            this.Stopwatch.Stop();
        }

        private bool KeyPressedIsSpacebarOrTab()
        {
            SetCurrentKeyPressed();
            return IsCurrentKeyValid(ConsoleKey.Tab, SpacebarOrTab.Tab) ||
                   IsCurrentKeyValid(ConsoleKey.Spacebar, SpacebarOrTab.Spacebar);
        }

        private void SetCurrentKeyPressed()
        {
            if (Console.KeyAvailable) 
            {
                this.CurrentKeyPressed = Console.ReadKey(true).Key;
            }
        }

        private bool IsCurrentKeyValid(ConsoleKey consoleKey, SpacebarOrTab validKeyPressed)
        {
            return this.CurrentKeyPressed == consoleKey &&
                this.LastKeyPressed != validKeyPressed;
        }

        private void ToggleLastKeyPressed()
        {
            if (CurrentKeyPressed == ConsoleKey.Tab)
            {
                this.LastKeyPressed = SpacebarOrTab.Tab;
            }
            else if (CurrentKeyPressed == ConsoleKey.Spacebar)
            {
                this.LastKeyPressed = SpacebarOrTab.Spacebar;
            }
        }

        private void RenderEndMessage()
        {
            Console.Clear();
            if (DidPlayerSucceed())
            {
                string successMessage = $"Congratulations! You alternated pressing Tab and Spacebar {SpacebarAndTabPresses} time(s).";
                Console.SetCursorPosition(Program.WINDOW_WIDTH/2 - (successMessage.Length / 2), Program.WINDOW_HEIGHT / 2);
                Console.WriteLine(successMessage);
            }
            else
            {
                string failMessage = "Darn, you were too slow. Looks like you're not going to make it.";
                Console.SetCursorPosition(Program.WINDOW_WIDTH/2 - (failMessage.Length / 2), Program.WINDOW_HEIGHT / 2);
                Console.WriteLine(failMessage);
            }
            WaitForPlayerToPressEnter();
        }

        public int GetIndexOfDestinationBasedOnUserSuccessOrFail()
        {
            return DidPlayerSucceed() ? 1 : 0;
        }

        public bool DidPlayerSucceed()
        {
            return SpacebarAndTabPresses >= TIMES_KEYS_MUST_BE_PRESSED;
        }
    }
}
