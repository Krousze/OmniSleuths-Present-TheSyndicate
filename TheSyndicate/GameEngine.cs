using System;
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
        private Player Player { get; set; }
        public static bool UseVoiceInput = false;
        public  enum StartMenuOptions{ START, SETTING, HELP, QUIT };

        public GameEngine()
        {
            string gameMode = ChooseGameMode();
            ChooseVoiceAssistance();
            Player.SetInstance(gameMode);
            this.Player = Player.GetInstance();
            LoadScenes();
            LoadCurrentScene();
        }

        public void Start()
        {
            //Console.CursorVisible = true;
            while (CurrentScene.HasNextScenes())
            {
                PlayScene();

            }
            PlayFinalScene();
        }

        private void ChooseVoiceAssistance()
        {
            Console.WriteLine("Would you like to use voice assistance? (Y/N)");
            string input = Console.ReadLine().ToLower();
            UseVoiceInput = input == "y" ? true : false;
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
            if (this.Player != null && this.Player.CurrentSceneId != null)
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
                if (scene.Key.Equals(Player.CurrentSceneId))
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

        private string ChooseGameMode()
        {
            Console.WriteLine("________                 .__            _________.__                 __  .__\n\\_____  \\   _____   ____ |__|          /   _____/|  |   ____  __ ___/  |_|  |__   ______\n /   |   \\ /     \\ /    \\|  |  ______  \\_____  \\ |  | _/ __ \\|  |  \\   __\\  |  \\ /  ___/\n/    |    \\  | |  \\   |  \\  | /_____/  /        \\|  |_\\  ___/|  |  /|  | |   |  \\___ \\\n\\_______  /__|_|  /___|  /__|         /_______  /|____/\\___  >____/ |__| |___|  /____ \\>\n        \\/      \\/     \\/                     \\/           \\/                 \\/     \\/\n__________                                      __                                      \n\\______   \\_______   ____   ______ ____   _____/  |_  /\\\n |     ___/\\_  __ \\_/ __ \\ /  ___// __ \\ /    \\   __\\ \\/\n |    |     |  | \\/\\  ___/ \\___ \\\\  ___/|   |  \\  |   /\\\n |____|     |__|    \\___  >____  >\\___  >___|  /__|   \\/\n                        \\/     \\/     \\/     \\/\n   ___________.__\n   \\__    ___/|  |__   ____\n     |    |   |  |  \\_/ __ \\\n     |    |   |   |  \\  ___/\n     |____|   |___|  /\\___  >\n                   \\/     \\/\n         _________                 .___.__               __\n        /   _____/__.__. ____    __| _/|__| ____ _____ _/  |_  ____\n        \\_____  <   |  |/    \\  / __ | |  |/ ___\\\\__  \\\\   __\\/ __ \\\n        /        \\___  |   |  \\/ /_/ | |  \\  \\___ / __ \\|  | \\  ___/\n       /_______  / ____|___|  /\\____ | |__|\\___  >____  /__|  \\___  >\n               \\/\\/         \\/      \\/         \\/     \\/          \\/\n                                            \n\na. new game | b. saved game");

            string userChoice = "";

            string cursorSymbol = "\uD83D\uDC36";
            int top = 25;
            int cur = top;
            var curIdx = 0;
            int left = (Program.WINDOW_WIDTH - 7) / 2;
            Console.SetCursorPosition(left - 2, top + curIdx);
            Console.Write(cursorSymbol);
            for (int i = 0; i < 4; i++)
            {
                Console.SetCursorPosition(left, cur++);
                Console.WriteLine((StartMenuOptions)i);
            }
            var input = ConsoleKey.Spacebar;
            
            
           

            while (input != ConsoleKey.Enter)
            {
                input = Console.ReadKey().Key;
                if (input == ConsoleKey.DownArrow)
                {
                    Console.SetCursorPosition(left - 2, top + curIdx);
                    Console.Write("  ");
                    curIdx = (curIdx + 1) % 4;
                    Console.SetCursorPosition(left - 2, top + curIdx);
                    Console.Write(cursorSymbol);

                }
                else if (input == ConsoleKey.UpArrow)
                {
                    Console.SetCursorPosition(left - 2, top + curIdx);
                    Console.Write("  ");
                    curIdx = (curIdx + 3) % 4;
                    Console.SetCursorPosition(left - 2, top + curIdx);
                    Console.Write(cursorSymbol);

                }

            }

            switch (curIdx)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    break;

            }


            //Console.SetCursorPosition(30,30);
            //Console.BackgroundColor = ConsoleColor.Blue;
            //Console.WriteLine("New Game");
            //Console.ResetColor();
            //Console.SetCursorPosition(40, 30);
            //Console.WriteLine("Saved Game");
            //var input = ConsoleKey.Spacebar;
            //while (input != ConsoleKey.Enter)
            //{
            //    input = Console.ReadKey().Key;
            //    if(input == ConsoleKey.LeftArrow)
            //    {
            //        userChoice = "a";
            //        Console.SetCursorPosition(30, 30);
            //        Console.BackgroundColor = ConsoleColor.Blue;
            //        Console.WriteLine("New Game");
            //        Console.ResetColor();
            //        Console.SetCursorPosition(40, 30);
            //        Console.WriteLine("Saved Game");
            //        Console.ResetColor();

            //    }
            //    else if (input == ConsoleKey.RightArrow)
            //    {
            //        userChoice = "b";
            //        Console.SetCursorPosition(30, 30);
            //        Console.WriteLine("New Game");
            //        Console.BackgroundColor = ConsoleColor.Blue;
            //        Console.SetCursorPosition(40, 30);
            //        Console.WriteLine("Saved Game");
            //        Console.ResetColor();
            //    }

            //}


            
            //while(userChoice != "a" && userChoice != "b")
            //{
            //    Console.WriteLine("You chose: " + userChoice);
            //    userChoice = Console.ReadLine().ToLower();
            //}
            return userChoice;
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
            string firstSceneId = GetFirstScene().Id;
            Player.ResetPlayerData(firstSceneId);
            CurrentScene.Play();
        }
    }
}
