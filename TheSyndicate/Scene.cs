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
        GamePlayer player = GamePlayer.GetInstance();
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

        public Dictionary<string, string>[] dialogue2 { get; private set; }


        public Scene(string id, string text, string[] options, string[] destinations, bool start, int[] lovePointsMaxMin, Dictionary<string, string>[] script, Dictionary<string, string>[] optionsScript, int count)
        {
            this.Id = id;
            this.Text = text;
            this.Options = options;
            this.Destinations = destinations;
            this.ActualDestinationId = null;
            this.Start = start;
            this.HumanityPointsMaxMin = lovePointsMaxMin;
            this.dialogue = script;
            this.dialogue2 = optionsScript;
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
            string msg = "Press ENTER when you are ready.";
            //Console.WriteLine("Press ENTER to stop the screen reader.");
            //Console.ReadLine();
            //Voice.StopMusic();
            Console.WriteLine(msg);
            tts.SynthesisToSpeakerAsync("Narrator", msg).Wait();
            Console.ReadLine();
            string voiceInput = await SpeechToText.RecognizeSpeechAsync();
            //Voice.pl.Resume();
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
            if (Count == 0 && GameEngine.UseVoiceInput)
            {
                tts.HearText(this.dialogue).Wait();
                return dialogBox;
            }
            else
            {
                return dialogBox;
            }
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
                if (GameEngine.UseVoiceInput)
                {
                tts.HearText(this.dialogue2).Wait();
                }
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
            Console.WriteLine($"Press 0 to save and quit.");

            // ??Test Love Points implementation.
            sceneTextBox.SetBoxPosition(Console.WindowWidth - (Console.WindowWidth / 3), Console.WindowHeight - 3);
            Console.WriteLine($"Humanity Points: {player.LovePointTotal}");
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

       

        private void RenderInstructions(TextBox sceneTextBox)
        {
            string msg = "What will you do next? Enter to speak the number next to the option you wish to choose:";
            string msg1 = "What will you do next? Enter the number next to the option and press enter:";
            string hpa = $"Your humanity points are, {player.LovePointTotal}";
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);
            if (GameEngine.UseVoiceInput)
            {
            Console.WriteLine(msg);
            tts.SynthesisToSpeakerAsync("Narrator", msg).Wait(); 
            tts.SynthesisToSpeakerAsync("Narrator", hpa).Wait();
            }
            else
            {
                Console.WriteLine(msg1);
            }

        }

        private void RenderQuitMessage(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);
            Console.WriteLine("You have reached the end of your journey.");
            //Console.ForegroundColor = ConsoleColor.Cyan;
        }

        private void ExecutePlayerOption(TextBox sceneTextBox)
        {
            int userInput = GetValidUserInput(sceneTextBox);
            if (userInput == SAVE_OPTION)
            {
                player.SavePlayerData(this.Id);
                if (GameEngine.UseVoiceInput)
                {
                    tts.SynthesisToSpeakerAsync("B.A.W.S. 5000", "You've chosen to save and quit the game. Until next time...").Wait();
                }
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
                //Console.SetCursorPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 3);
                //Console.WriteLine(new string(' ', Program.WINDOW_WIDTH));
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
                    Console.SetCursorPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
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
                Console.SetCursorPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 3);
            }
            while (!IsValidInput(userInput,sceneTextBox));

            return userInput;
        }

        public bool IsValidInput(int userInput, TextBox tb)
        {
            int numberOfOptions = this.Options.Length;
            //bool isValid = IsOptionAvailable(userInput - 1);
            //if (!isValid)
            //{



            //    string msg = "Pick again...";
            //    //Console.Write(spaces);
            //    Console.Write(msg);
            //    //Console.ReadKey();

            //    //Console.SetCursorPosition(Console.CursorLeft-msg.Length, Console.CursorTop);
            //    //Console.Write(spaces);
            //}
            bool isValid = false;
            Console.SetCursorPosition(tb.TextBoxX, tb.TextBoxY + 3);
            string msg = new string(' ', 60);
            Console.WriteLine(msg);
            int loveMin = this.HumanityPointsMaxMin[1];
            int hateMax = this.HumanityPointsMaxMin[0];
            switch (userInput)
            {
                case 0:
                    isValid = true;
                    break;
                case 1:
                    if(player.LovePointTotal >= loveMin)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        msg = $"You still need {loveMin - player.LovePointTotal} points to choose this path.";
                    }
                    break;
                case 2:
                    if (player.LovePointTotal <= hateMax)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        msg = $"You need to lose {player.LovePointTotal - hateMax} points to choose this path.";
                    }
                    break;
                case 3:
                    isValid = true;
                    break;
                case 4:
                    isValid = true;
                    break;
                default:
                    isValid = false;
                    msg = "Not a valid option.";
                    break;

            }
            Console.SetCursorPosition(tb.TextBoxX, tb.TextBoxY + 3);
            Console.Write(msg);
            if (GameEngine.UseVoiceInput && msg != "")
            {
                tts.SynthesisToSpeakerAsync("Narrator", msg).Wait();
            }


            return isValid;
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
            }
        }

        private void PlayMiniGameAndUpdatePoints()
        {
            Random rd = new Random();
            //int gameIdx = GameEngine.UseVoiceInput ? rd.Next(0,Games.Count-1) : 2;
            this.Action = Games[1];
            Action.ExecuteActionAsync().Wait();
            player.AddLovePoints(Action.DidPlayerSucceed() ? 5 : -5);
        }

        public bool HasNextScenes()
        {
            return Destinations.Length > 0;
        }
    }
}