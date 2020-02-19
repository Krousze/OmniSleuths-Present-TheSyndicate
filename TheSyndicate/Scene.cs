using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;

using TheSyndicate.Actions;
using CC = Colorful.Console;
using System.Text.RegularExpressions;
using System.Threading;

namespace TheSyndicate
{
    public class Scene
    {
        public static int SAVE_OPTION = 0;
        player player = player.GetInstance();
        public string Id { get; private set; }
        public string Text { get; private set; }
        public string[] Options { get; private set; }
        public string[] Destinations { get; private set; }
        public string ActualDestinationId { get; private set; }
        public bool Start { get; private set; }
        public IAction Action { get; set; }
        public static List<IAction> Games = new List<IAction>() { new RiddleAction(), new TongueTwisterAction(), new KeyPressAction() };


        public int Count { get; private set; }
        public int ScenePoints { get; private set; }
        public int[] HumanityPointsMaxMin { get; private set; } /// Maximum allowable points for "evil" path, Minimum allowable
                                                                /// Love Points for 'love' path.

        private TextToSpeech tts = new TextToSpeech();
        public Dictionary<string, string>[] dialogue { get; private set; }



        public Scene(string id, string text, string[] options, string[] destinations, bool start, int[] lovePointsMaxMin, Dictionary<string, string>[] script, int count)
        {
            this.Id = id;
            this.Text = text;
            this.Options = options;
            this.Destinations = destinations;
            this.ActualDestinationId = null;
            this.Start = start;
            this.HumanityPointsMaxMin = lovePointsMaxMin;
            this.dialogue = script;
            this.Count = count;
        }

        public void Play()
        {
            TextBox sceneTextBox = RenderText();
            RenderProgressBar();
            RenderOptions(sceneTextBox);
            if (this.Options.Length > 0)
            {
                Count++;
                ExecutePlayerOption(sceneTextBox);
            }
        }

        private async Task<string> GetVoiceInput()
        {
            Console.WriteLine("Say something...Press ENTER when you are ready.");
            Console.ReadLine();
            string voiceInput = await SpeechToText.RecognizeSpeechAsync();
            return Regex.Replace(voiceInput.ToLower(), @"[^\w\s]", "");
        }

        public void RenderProgressBar()
        {

            int blocks = player.LovePointTotal * 20 / 100;

            char POINT_METER_BLOCK = '\u2588';
            int cursorX = Program.WINDOW_WIDTH - 10;
            int cursorY = Program.WINDOW_HEIGHT - 25;
            Console.SetCursorPosition(cursorX - 7, cursorY--);
            CC.Write("Syndicate Bot", Color.Red);
            for (int i = 0; i <= 19; i++)
            {
                Console.SetCursorPosition(cursorX, cursorY--);
                if (i <= 9)
                {
                    CC.Write(new string(POINT_METER_BLOCK, 1), Color.Red);
                }
                else if (i > 9 && i <= 19)
                {
                    CC.Write(new string(POINT_METER_BLOCK, 1), Color.Green);
                }
                if (i == blocks)
                {
                    CC.Write(" <--", Color.White);
                }
            }
            Console.SetCursorPosition(cursorX - 4, cursorY);
            CC.Write("Humanity", Color.Green);
            CC.ForegroundColor = Color.White;
        }

        TextBox RenderText()
        {
            ClearConsole();

            //TextBox is instantiated to pass this.Text and get access to TextBox Width and Height properties 
            TextBox dialogBox = new TextBox(this.Text, Program.WINDOW_WIDTH * 3 / 4, 2, (Program.WINDOW_WIDTH - (Program.WINDOW_WIDTH * 3 / 4)) / 2, 2);
            dialogBox.FormatText(this.Text);
            dialogBox.DrawDialogBox(this.Text);
            //tts.HearText(this.Text);
            if (Count == 0)
            {
                //tts.HearText(this.dialogue);
                Voice.PlayMusic(this.Id);
                //return dialogBox;
            }
            //else
            //{
            //    return dialogBox;
            //}
            return dialogBox;

            //playVoice(); //??Asynchronous play

            //returning dialogBox for information about height of dialog box


        }

        //??Play asynchronously?
        //public async Task playVoice()
        //{
        //   await Task.Run(()=> tts.HearText(this.Text));

        //}


        void RenderOptions(TextBox sceneTextBox)
        {
            //checks for end scene
            if (this.Options.Length > 0)
            {
                RenderUserOptions(sceneTextBox);
            }
            else
            {
                RenderQuitMessage(sceneTextBox);
            }
        }

        private void RenderUserOptions(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);

            RenderInstructions(sceneTextBox);
            if (!this.Id.Equals("fight"))
            {

                PrintAvailableOptions(sceneTextBox);
            }
            else
            {

                for (int i = 0; i < this.Options.Length; i++)
                {
                    sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);

                    Console.WriteLine($"{i + 1}: {this.Options[i]}");
                    sceneTextBox.TextBoxY += 2;
                }
            }
            sceneTextBox.SetBoxPosition(Program.WINDOW_WIDTH - (Program.WINDOW_WIDTH / 3), Program.WINDOW_HEIGHT - 2);
            Console.WriteLine($"Press 0 at any point to save and quit.");

            // ??Test Love Points implementation.
            sceneTextBox.SetBoxPosition(Console.WindowWidth - (Console.WindowWidth / 3), Console.WindowHeight - 3);
            Console.WriteLine($"Humanity Points: {player.LovePointTotal} | SyndicateManMax: {this.HumanityPointsMaxMin[0]} | HumanityMin: {this.HumanityPointsMaxMin[1]}");
        }

        private void PrintAvailableOptions(TextBox sceneTextBox)
        {
            int optLen = this.Options.Length;
            //Random rnd = new Random();
            for (int i = 0; i < optLen; i++)
            {
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                //ConsoleColor currentColor = Console.ForegroundColor;
                CC.WriteLine($"{i + 1}: {this.Options[i]}", IsOptionAvailable(i) ? Color.Cyan : Color.Gray);
                //Console.ForegroundColor = currentColor;
                sceneTextBox.TextBoxY += 2;
            }
        }

        private bool IsOptionAvailable(int index)
        {
            int loveMin = this.HumanityPointsMaxMin[1];
            int hateMax = this.HumanityPointsMaxMin[0];

            if (index == -1)
            {
                return true;
            }
            if (index == 0)
            {
                return player.LovePointTotal >= loveMin;
            }
            else if (index == 1)
            {
                return player.LovePointTotal <= hateMax;
            }
            else if (index == 2 || index == 3)
            {
                return true;
            }
            return false;

        }

        private void RenderInstructions(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);

            Console.WriteLine("What will you do next? Enter the number next to the option and press enter:");
        }

        private void RenderQuitMessage(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);
            Console.WriteLine("You have reached the end of your journey. Press CTRL + C to end.");
            //Console.ForegroundColor = ConsoleColor.Cyan;
        }

        private void ExecutePlayerOption(TextBox sceneTextBox)
        {
            int userInput = GetValidUserInput(sceneTextBox);
            if (userInput == SAVE_OPTION)
            {
                player.SavePlayerData(this.Id);
                Environment.Exit(0);
            }
            else
            {
                SetDestinationId(userInput);
            }
        }

        private int GetValidUserInput(TextBox sceneTextBox)
        {
            int userInput = -1;//Unassigned, userInput is zero.

            do
            {
                string spaces = "                                                                       ";
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                Console.Write(spaces);
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                string words = "";
                if (GameEngine.UseVoiceInput)
                {
                    Task<string> inputTask = GetVoiceInput();
                    inputTask.Wait();
                    words = inputTask.Result;
                }
                else
                {
                    words = Console.ReadLine();
                }
                if(words == "s")
                {
                    Voice.StopMusic();
                }
                if (Int32.TryParse(words, out int xInput))
                {
                    userInput = xInput;
                }
                Console.SetCursorPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
            }
            while (!IsValidInput(userInput));

            return userInput;
        }

        public bool IsValidInput(int userInput)
        {
            int numberOfOptions = this.Options.Length;
            bool isValid = IsOptionAvailable(userInput - 1);
            if (!isValid)
            {
                string msg = "Pick again...";
                //Console.Write(spaces);
                Console.Write(msg);
                Console.ReadKey();

                //Console.SetCursorPosition(Console.CursorLeft-msg.Length, Console.CursorTop);
                //Console.Write(spaces);
            }
            return isValid;
        }

        void ClearConsole()
        {
            Console.Clear();
        }

        void SetDestinationId(int selectedOption)
        {
            this.ActualDestinationId = this.Destinations[selectedOption - 1];
            if (selectedOption == 4)
            {
                this.ActualDestinationId = this.Id;
                PlayMiniGameAndUpdatePoints();
                //if (!Action.DidPlayerSucceed())
                //{
                //    this.ActualDestinationId = "dead";
                //}
            }


            //if (this.ActualDestinationId.Equals("fight"))
            //{

            //    player.AddLovePoints(-5);//?? Lose Love Points for getting into fight
            //    this.Action = new FightAction();
            //    Action.ExecuteActionAsync();
            //    if (Action.DidPlayerSucceed())
            //    {
            //        this.ActualDestinationId = "recyclerTruck";
            //    }
            //    else
            //    {
            //        this.ActualDestinationId = "dead";
            //        player.AddLovePoints(0);//?? Love Points go to zero upon death.
            //    }
            //}
            //else if (this.Id.Equals("upload") ||
            //    (this.Id.Equals("recyclerTruck") && this.ActualDestinationId.Equals("city")))
            //else if (this.Id.Equals("game"))
            //{
            //    this.Action = new KeyPressAction();
            //    Action.ExecuteAction();
            //    this.ActualDestinationId = "introScene";
            //    //if (!Action.DidPlayerSucceed())
            //    //{
            //    //    this.ActualDestinationId = "dead";
            //    //}
            //}
        }

        private void PlayMiniGameAndUpdatePoints()
        {
            Random rd = new Random();
            int gameIdx = rd.Next(0, Games.Count);
            this.Action = Games[gameIdx];
            Action.ExecuteActionAsync().Wait();
            player.AddLovePoints(Action.DidPlayerSucceed() ? 5 : -5);
        }

        public bool HasNextScenes()
        {
            return Destinations.Length > 0;
        }
    }
}