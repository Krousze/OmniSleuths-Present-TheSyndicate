using System;
using System.Drawing;
using System.Threading.Tasks;
using TheSyndicate.Actions;
using CC = Colorful.Console;

namespace TheSyndicate
{
    public class Scene
    {
        public static int SAVE_OPTION = 0;
        Player player = Player.GetInstance();
        public string Id { get; private set; }
        public string Text { get; private set; }
        public string[] Options { get; private set; }
        public string[] Destinations { get; private set; }
        public string ActualDestinationId { get; private set; }
        public bool Start { get; private set; }
        public IAction Action { get; set; }


        public int ScenePoints { get; private set; }
        public int[] LovePointsMaxMin { get; private set; } /// Maximum allowable points for "evil" path, Minimum allowable
                                                            /// Love Points for 'love' path.

        bool[] choiceArray = { true, true, true, true };
        private TextToSpeech tts = new TextToSpeech();



        public Scene(string id, string text, string[] options, string[] destinations, bool start, int[] lovePointsMaxMin)
        {
            this.Id = id;
            this.Text = text;
            this.Options = options;
            this.Destinations = destinations;
            this.ActualDestinationId = null;
            this.Start = start;
            this.LovePointsMaxMin = lovePointsMaxMin;
        }

        public void Play()
        {
            TextBox sceneTextBox = RenderText();
            RenderProgressBar();
            RenderOptions(sceneTextBox);
            if (this.Options.Length > 0)
            {
                ExecutePlayerOption(sceneTextBox);
            }
        }

        public void RenderProgressBar()
        {
            
            int blocks = player.LovePointTotal * 20 / 100;

            char POINT_METER_BLOCK = '\u2588';
            int cursorX = Program.WINDOW_WIDTH - 10;
            int cursorY = Program.WINDOW_HEIGHT - 25;
            Console.SetCursorPosition(cursorX - 1, cursorY--);
            int r = 240;
            int g = 0;
            int b = 0;
            CC.Write("EVIL", Color.FromArgb(r, g, b));
            for (int i = 0; i <= 19; i++)
            {
                if (i == 0)
                {
                    r = 240;
                    g = 0;
                    b = 0;
                }
                if (i == 10)
                {
                    r = 240;
                    g = 240;
                    b = 240;
                }
                Console.SetCursorPosition(cursorX, cursorY--);
                if (i <= 9)
                {
                    CC.Write(new string(POINT_METER_BLOCK, 2), Color.FromArgb(r, g, b));
                    g += 23;
                    b += 23;
                }
                else if (i > 9 && i <= 19)
                {
                    CC.Write(new string(POINT_METER_BLOCK, 2), Color.FromArgb(r, g, b));
                    r -= 23;
                    b -= 23;
                }
                if (i == blocks)
                {
                    CC.Write(" <--", Color.Gray);
                }
            }
            r = 0;
            g = 240;
            b = 0;

            Console.SetCursorPosition(cursorX - 1, cursorY);
            CC.Write("Good", Color.FromArgb(r, g, b));
        }

        TextBox RenderText()
        {
            ClearConsole();

            //TextBox is instantiated to pass this.Text and get access to TextBox Width and Height properties 

            TextBox dialogBox = new TextBox(this.Text, Program.WINDOW_WIDTH * 3 / 4, 2, (Program.WINDOW_WIDTH - (Program.WINDOW_WIDTH * 3 / 4)) / 2, 2);
            dialogBox.FormatText(this.Text);
            dialogBox.DrawDialogBox(this.Text);
            //tts.HearText(this.Text);
            //playVoice(); //??Asynchronous play

            //returning dialogBox for information about height of dialog box


            return dialogBox;
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
            Console.WriteLine($"Love Points: {player.LovePointTotal} | HateMax: {this.LovePointsMaxMin[0]} | LoveMin: {this.LovePointsMaxMin[1]}");
        }

        private void PrintAvailableOptions(TextBox sceneTextBox)
        {
            int optLen = this.Options.Length;
            //Random rnd = new Random();
            for (int i = 0; i < optLen; i++)
            {
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                ConsoleColor currentColor = Console.ForegroundColor;
                CC.WriteLine($"{i + 1}: {this.Options[i]}", IsOptionAvailable(i) ? Color.Cyan : Color.Gray);
                Console.ForegroundColor = currentColor;
                sceneTextBox.TextBoxY += 2;
            }
        }

        private bool IsOptionAvailable(int index)
        {
            int loveMin = this.LovePointsMaxMin[1];
            int hateMax = this.LovePointsMaxMin[0];

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
            Console.ForegroundColor = ConsoleColor.Cyan;
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
            int userInput=-1;//Unassigned, userInput is zero.

            do
            {
                string spaces = "                                          ";
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                Console.Write(spaces);
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                if (Int32.TryParse(Console.ReadLine(), out int xInput))
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
            bool isValid = IsOptionAvailable(userInput-1);
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


            if (this.ActualDestinationId.Equals("fight"))
            {

                player.AddLovePoints(-5);//?? Lose Love Points for getting into fight
                this.Action = new FightAction();
                Action.ExecuteAction();
                if (Action.DidPlayerSucceed())
                {
                    this.ActualDestinationId = "recyclerTruck";
                }
                else
                {
                    this.ActualDestinationId = "dead";
                    player.AddLovePoints(0);//?? Love Points go to zero upon death.
                }
            }
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
            this.Action = new KeyPressAction();
            Action.ExecuteAction();
            player.AddLovePoints(Action.DidPlayerSucceed() ? 5 : -5);
        }

        public bool HasNextScenes()
        {
            return Destinations.Length > 0;
        }
    }
}