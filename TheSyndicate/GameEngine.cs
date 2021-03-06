﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace TheSyndicate
{
    class GameEngine
    {
        private string PATH_TO_STORY = Program.ASSETS_PATH + "story.json";
        private Dictionary<string, Scene> Scenes { get; set; }
        private Scene CurrentScene { get; set; }
        private GamePlayer player { get; set; }
        public static bool UseVoiceInput = false;
        public  enum StartMenuOptions{ START, SETTING, HELP, QUIT, undefined };
        public static TextToSpeech tts = new TextToSpeech();

        public GameEngine()
        {

            StartScreen();
            this.player = GamePlayer.GetInstance();
            LoadScenes();
            LoadCurrentScene();
        }

        public void Start()
        {
            while (CurrentScene.HasNextScenes())
            {
                PlayScene();

            }
            PlayFinalScene();
            if (UseVoiceInput)
            {
                tts.SynthesisToSpeakerAsync("B.A.W.S. 5000", "You have reached the end of your journey. Until next time...").Wait();
            }
            
        }


        private void LoadScenes()
        {
            Scenes = GetScenes();
        }

        private Dictionary<string, Scene> GetScenes()
        {
            List<Scene> scenes = ConvertStoryFromJsonToScenes();
            Dictionary<string, Scene> sceneIdsToScene = new Dictionary<string, Scene>();
            
            foreach(Scene scene in scenes)
            {
                sceneIdsToScene[scene.Id] = scene;
            }

            return sceneIdsToScene;
        }

        // https://stackoverflow.com/questions/18192357/deserializing-json-object-array-with-json-net
        private List<Scene> ConvertStoryFromJsonToScenes()
        {
            string story = GetStoryFromFile();
            return JsonConvert.DeserializeObject<List<Scene>>(story);
        }

        private string GetStoryFromFile()
        {
            return File.ReadAllText(PATH_TO_STORY);
        }

        private void LoadCurrentScene()
        {
            CurrentScene = GetStartingScene();
        }

        private Scene GetStartingScene()
        {
            if (this.player != null && this.player.CurrentSceneId != null)
            {
                return GetSceneFromPlayer();
            }
            else
            {
                return GetFirstScene();
            }
        }

        private Scene GetSceneFromPlayer()
        {
            Scene startScene = null;
            foreach (KeyValuePair<string, Scene> scene in this.Scenes)
            {
                if (scene.Key.Equals(player.CurrentSceneId))
                {
                    startScene = scene.Value;
                }
            }
            return startScene;
        }

        private Scene GetFirstScene()
        {
            Scene stateScene = null;
            foreach (KeyValuePair<string, Scene> scene in this.Scenes)
            {
                if (scene.Value.Start == true)
                {
                    stateScene = scene.Value;
                }
            }
            return stateScene;
        }

        private void StartScreen()
        {
            StartMenuOptions chosenOption = StartMenuOptions.undefined;
            StartMenuOptions currentOption = StartMenuOptions.START;

            int curIdx = 0;
            
            var input = ConsoleKey.Spacebar;

            while(chosenOption != StartMenuOptions.START)
            {
                PrintStartScreen(currentOption);
                input = Console.ReadKey().Key;
                if (input == ConsoleKey.DownArrow)
                {
                    curIdx = (curIdx + 1) % 4;
                    currentOption = (StartMenuOptions)curIdx;
                }
                else if (input == ConsoleKey.UpArrow)
                {
                    
                    curIdx = (curIdx + 3) % 4;
                    currentOption = (StartMenuOptions)curIdx;
                }
                else if (input == ConsoleKey.Enter)
                {
                    chosenOption = currentOption;
                    ExecuteMenuOption(chosenOption);
                }
            }
        }

        private void PrintStartScreen(StartMenuOptions curOption)
        {
            Console.Clear();
            StreamReader file = new StreamReader(Program.ASSETS_PATH + "title.txt");
            string line;
            int optionLeft = (Program.WINDOW_WIDTH - 7) / 2;
            int titleLeft = (Program.WINDOW_WIDTH - 115)/2;
            int counter = 1;
            while ((line = file.ReadLine()) != null)
            {
                Console.SetCursorPosition(titleLeft , counter++);
                Console.WriteLine(line);
            }

            file.Close();
            string cursorSymbol = ">>>";
            int top = 30;
            //int left = (Program.WINDOW_WIDTH - 7) / 2;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(optionLeft - 4, top + (int)curOption);
            Console.Write(cursorSymbol);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < 4; i++)
            {
                Console.SetCursorPosition(optionLeft, top++);
                Console.WriteLine((StartMenuOptions)i);
            }

        }

        private void ExecuteMenuOption(StartMenuOptions option)
        {
            switch (option)
            {
                case StartMenuOptions.HELP:
                    DisplayHelpMenu();
                    break;
                case StartMenuOptions.SETTING:
                    DisplaySettingMenu();
                    break;
                case StartMenuOptions.QUIT:
                    Environment.Exit(0);
                    break;
                case StartMenuOptions.START:
                    SetNewOrSavedGame();
                    break;
                default:
                    break;

            }
        }

        private void SetNewOrSavedGame()
        {
            bool pickNewGame = true;
            var input = ConsoleKey.Spacebar;
            PrintGameModeOptions(pickNewGame);
            while(input != ConsoleKey.Enter)
            {
                input = Console.ReadKey().Key;
                if(input == ConsoleKey.LeftArrow)
                {
                    pickNewGame = true;
                }
                else if(input == ConsoleKey.RightArrow)
                {
                    pickNewGame = false;
                }
                PrintGameModeOptions(pickNewGame);
            }
            if (pickNewGame)
            {
                GamePlayer.SetInstance("a");
            }
            else
            {
                GamePlayer.SetInstance("b");
            }
           

        }

        private void PrintGameModeOptions(bool pickNewGame)
        {
            int top = 25;
            int left = (Program.WINDOW_WIDTH - 20) / 2;
            Console.Clear();
            Console.ForegroundColor = pickNewGame ? ConsoleColor.DarkCyan : ConsoleColor.White;
            Console.SetCursorPosition(left, top);
            Console.Write("NEW GAME");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\t");
            Console.ForegroundColor = pickNewGame ? ConsoleColor.White : ConsoleColor.DarkCyan;
            Console.Write("SAVED GAME");
            Console.ForegroundColor = ConsoleColor.White;

        }

        private void DisplayHelpMenu()
        {
            Console.Clear();
            StreamReader file = new StreamReader(Program.ASSETS_PATH + "helpContent.txt");
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("Normal Game"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (line.Contains("Accessibility Game"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (line.Contains("Both Modes"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(line);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press Enter to return");

            file.Close();
            Console.ReadLine();
        }

        private void DisplaySettingMenu()
        {
            var input = ConsoleKey.Spacebar;
            PrintSettingOption();
            while (input != ConsoleKey.Enter)
            {
                input = Console.ReadKey().Key;
                if (input == ConsoleKey.LeftArrow)
                {
                    UseVoiceInput = true;
                }
                else if (input == ConsoleKey.RightArrow)
                {
                    UseVoiceInput = false;
                }
                PrintSettingOption();
            }
        }

        private void PrintSettingOption()
        {
            int top = 25;
            int left = (Program.WINDOW_WIDTH - 20) / 2;
            Console.Clear();
            Console.SetCursorPosition(left, top);
            Console.Write("Voice Assistance: ");
            Console.ForegroundColor = UseVoiceInput ? ConsoleColor.DarkCyan : ConsoleColor.White;
            Console.Write("ON");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("/");
            Console.ForegroundColor = UseVoiceInput ? ConsoleColor.White : ConsoleColor.DarkCyan;
            Console.Write("OFF");
            Console.ForegroundColor = ConsoleColor.White;
        }


        private void PlayScene()
        {
            CurrentScene.Play();
            CurrentScene = GetNextScene();
        }

        private Scene GetNextScene()
        {
            return this.Scenes[CurrentScene.ActualDestinationId];
        }

        private void PlayFinalScene()
        {
            //string firstSceneId = GetFirstScene().Id;
            //player.ResetPlayerData(firstSceneId);
            CurrentScene.Play();
        }
    }
}
