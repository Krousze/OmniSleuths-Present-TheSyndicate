﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace TheSyndicate.Actions
{
    enum Attack { LeftHook, RightHook, LaserBeam}
    enum Dodge { DodgeRight, DodgeLeft, Duck, NoDodge }

    class FightAction
    {
        private static int SECONDS_USER_HAS_TO_DODGE = 1;
        private static int TIMES_USER_MUST_DODGE_ATTACKS = 3;
        private static int NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST = 5;
        private static string FIGHTASCII =
"                                             (#%%%#(\n                                 .###%#.    /%%%#%*#(\n                                *#%%%%%##   #%%%%###,\n                                #%#%%%%%*  ,%%%%%%##   .##%##%*\n                               .#####%%#   #%%%%%%#,   ,(%%%%%#*\n                               #%%%%%#%(  .#%##%%##   /%%%%%%%#\n                               /%%%%%##   ##%#####/  .%%%%%%##.\n                              /%%%%%#%#  .##%%%%%#.  %#%%%%%%/\n                              #*%%%%##.  (%%%%%%%#  /(%%%%%%%\n                             *#(#%%%%%   //%#%%%#. .%#%%%%%#.    #%%###\n                             ##(%%%%%,  *###%%#%#  ##/##%%#(    #%%%%##,\n                            ,%##%%%%%%##%(###,/#, *#%%%%%%#   ,%#%%%##\n           ,*,              #%%##%%%%%%#%(###**# .#%%%%%%#*  *###%%#(\n         #%%#%#(#/         .#%%%%%%%%%%%%%%%%%%%#%#%%%%%%#  %(%##%%*\n          %%#%(##%#%       ##%%%%%##%%%#/#%%%%###%%%#/#.*..*%%(%%%\n           *#%####%%##\\\\//.##(((%#,      *%%(    #*|        |%#%#\n            .%%##%%%#/    %%    ##        %%%    *#|        |%#/\n              ##%%%%%#    **    ##    %   ,#%    .#%#,    #%##.\n               #%%%%%%    %%    (#    %    ##,    ##%#    (%%\n                /#%%%#.         *#    %%   ,#(    %%#%    ,(\n                 *#%%%/    %    .#          #%    (#%#    /\n                  ,#%%#    %%    %    %%    .%    */##.__/\n                   .###    %%    %    %%%    /%    %%// /\n                     #%    %%    /##%%%%%%%%####%%#%#%//\n                      #%%%/%%#%%%%%%#%#%%%%%%####%%%%.\n                       .#((%%%%%%%%#(/#%%%%%%(%%%%%#\n                          /%%#%%%%#%%#####%%%#%#/(\n                              .(%##%(%##,(#%#/.\n\n\n\n                             Press ENTER to continue.                 \n";
        private static string INSTRUCTIONS = $"HALT. YOU ARE NOT AUTHORIZED TO ACCESS THIS LOCATION!!\n\nYou turn to find a relic of the war between The Syndicate and humans, a Watchman robot. You've gotten yourself into a fight! You're a lover, not a fighter though so you refuse to hurt your opponent. Looks like you're going to have to dodge.\nYour opponent will attack you {NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST} time(s) and you must successfully dodge {TIMES_USER_MUST_DODGE_ATTACKS} time(s). \nYou will have {SECONDS_USER_HAS_TO_DODGE} second(s) to respond by pressing the correct arrow key.\nIf your opponent throws a left hook, you must dodge right (right arrow key)\nIf your opponent throws a right hook, you must dodge left (left arrow key)\nIf your opponent shoots a laser beam, you must duck(down arrow key)";
        private static int NumberOfTypesOfAttacks = Attack.GetNames(typeof(Attack)).Length;
        private Stopwatch Stopwatch { get; set; }
        private Random Random { get; }
        private int SuccessfullDodges { get; set; }
        private Attack CurrentAttack { get; set; }
        private Dodge CurrentDodge { get; set; }
        private ConsoleKey CurrentKeyPressed { get; set; }

        public FightAction()
        {
            this.SuccessfullDodges = 0;
            this.Random = new Random();
        }

        public void ExecuteActionAsync()
        {
            Console.CursorVisible = false;
            RenderInstructions();
            WaitForPlayerToPressEnter();
            Fight();
            RenderEndMessage();
            Console.CursorVisible = true;
        }

        private void RenderInstructions()
        {
            TextBox fightACSII = new TextBox(FIGHTASCII, Program.WINDOW_WIDTH / 3, 2, Program.WINDOW_WIDTH / 3, Program.WINDOW_HEIGHT / 3);
            TextBox instructions = new TextBox(INSTRUCTIONS, Program.WINDOW_WIDTH / 3, 2, Program.WINDOW_WIDTH / 3, Program.WINDOW_HEIGHT / 8);
            Console.Clear();
            fightACSII.SetBoxPosition(fightACSII.TextBoxX, fightACSII.TextBoxY);
            fightACSII.FormatText(FIGHTASCII);
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(INSTRUCTIONS);
        }

        private void WaitForPlayerToPressEnter()
        {
            //string enterPrompt = "Press ENTER to continue.";
            //Console.SetCursorPosition(Program.WINDOW_WIDTH / 2 - enterPrompt.Length / 2, Program.WINDOW_HEIGHT - (Program.WINDOW_HEIGHT / 16));
            //Console.WriteLine(enterPrompt);

            ConsoleKey userInput = Console.ReadKey(true).Key;
            while (userInput != ConsoleKey.Enter)
            {
                userInput = Console.ReadKey(true).Key;
            }
        }

        private void Fight()
        {
            for (int i = 0; i < NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST; i++)
            {
                CurrentDodge = Dodge.NoDodge;
                RenderFightOptions();
                SetCurrentAttack();
                Console.SetCursorPosition(Program.WINDOW_WIDTH / 2 - 18, Program.WINDOW_HEIGHT / 2);
                Console.WriteLine($"Opponent's attack: {CurrentAttack}");
                SetCurrentDodge();
                if (UserSuccessfullyDodged())
                {
                    SuccessfullDodges++;
                }
            }
        }

        private void RenderFightOptions()
        {
            string options = "Left Hook  --> Right Dodge (Right Arrow Key)\nRight Hook --> Left Dodge (Left Arrow Key)\nLaser Beam --> Duck (Down Arrow Key)";
            TextBox instructions = new TextBox(options, Program.WINDOW_WIDTH / 3, 2, Program.WINDOW_WIDTH/2 - Program.WINDOW_WIDTH / 6, Program.WINDOW_HEIGHT / 4);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(options);
        }

        private void SetCurrentAttack()
        {
            CurrentAttack = (Attack)GetRandomNumberLessThanNumberOfAttacks();
        }

        private int GetRandomNumberLessThanNumberOfAttacks()
        {
            return Random.Next(0, NumberOfTypesOfAttacks);
        }

        private void SetCurrentDodge()
        {
            GetUserInput();
            if (CurrentKeyPressed == ConsoleKey.LeftArrow ||
                CurrentKeyPressed == ConsoleKey.RightArrow ||
                CurrentKeyPressed == ConsoleKey.DownArrow)
            {
                CurrentDodge = ConvertUserInputToDodge();
            }
        }

        private void GetUserInput()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            while (this.Stopwatch.Elapsed <= TimeSpan.FromSeconds(SECONDS_USER_HAS_TO_DODGE))
            {
                SetCurrentKeyPressed();                
            }
            this.Stopwatch.Stop();
        }

        private void SetCurrentKeyPressed()
        {
            if (Console.KeyAvailable)
            {
                this.CurrentKeyPressed = Console.ReadKey(true).Key;
            }
        }

        private Dodge ConvertUserInputToDodge()
        {
            if (CurrentKeyPressed == ConsoleKey.LeftArrow)
            {
                return Dodge.DodgeLeft;
            }
            else if (CurrentKeyPressed == ConsoleKey.RightArrow)
            {
                return Dodge.DodgeRight;
            }
            else if (CurrentKeyPressed == ConsoleKey.DownArrow)
            {
                return Dodge.Duck;
            }
            else
            {
                return Dodge.NoDodge;
            }
        }

        private void RenderEndMessage()
        {
            Console.Clear();
            if (DidPlayerSucceed())
            {
                string successMessage = $"Phew, that was close! You successfully dodged {SuccessfullDodges} attack(s). You've still been caught but at least you live to see another day. Off to the reclamation center you go.";
                Console.SetCursorPosition(Program.WINDOW_WIDTH/2 - successMessage.Length/2, Program.WINDOW_HEIGHT/2);
                Console.WriteLine(successMessage);
            }
            else
            {
                string failMessage = $"Darn, you were too slow. It was an honor to narrate you.";
                Console.SetCursorPosition(Program.WINDOW_WIDTH / 2 - failMessage.Length / 2, Program.WINDOW_HEIGHT / 2);
                Console.WriteLine(failMessage);
            }
            WaitForPlayerToPressEnter();
        }

        private bool UserSuccessfullyDodged()
        {
            return (int)CurrentAttack == (int)CurrentDodge;
        }

        public int GetIndexOfDestinationBasedOnUserSuccessOrFail()
        {
            return DidPlayerSucceed() ? 1 : 0;
        }

        public bool DidPlayerSucceed()
        {
            return SuccessfullDodges >= TIMES_USER_MUST_DODGE_ATTACKS;
        }
    }
}
