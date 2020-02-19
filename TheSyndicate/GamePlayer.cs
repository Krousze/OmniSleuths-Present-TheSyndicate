using System;
using Newtonsoft.Json;
using System.IO;

namespace TheSyndicate
{
    public class GamePlayer
    {
        private static GamePlayer _instance;
        private const int MAXIMUM_BATTERY_POWER = 4;
        private static string PATH_TO_SAVE_STATE = Program.ASSETS_PATH + "SaveState.json";
        public string CurrentSceneId { get; private set; }
        public int BatteryPower { get; set; }

        //LovePoints serve as a measure of progress and influence available paths.
        private const int MAXIMUM_LOVEPOINTS = 100;
        public int LovePointTotal { get; private set; }

        [JsonConstructor]
        private GamePlayer(string currentSceneId = null,
                       int batteryPower = MAXIMUM_BATTERY_POWER, int lovePointTotal = MAXIMUM_LOVEPOINTS / 2)
        {
            this.CurrentSceneId = currentSceneId;
            this.BatteryPower = batteryPower;
            this.LovePointTotal = lovePointTotal;
        }

        public static GamePlayer GetInstance()
        {
            if(_instance == null)
            {
                _instance =  new GamePlayer();
            }
            return _instance;
        }

        public static void SetInstance(string gameMode)
        {
            if(gameMode == "a")
            {
                _instance = new GamePlayer();
            }
            else if (_instance == null)
            {
                _instance = GetPlayerFromSaveState();
            }
        }

        private static GamePlayer GetPlayerFromSaveState()
        {
            try
            {
                return ConvertSaveStateToPlayer();
            }
            catch
            {
                return new GamePlayer();
            }
        }

        private static GamePlayer ConvertSaveStateToPlayer()
        {
            string savedPlayerDataAsJson = GetSaveState();
            return JsonConvert.DeserializeObject<GamePlayer>(savedPlayerDataAsJson);
        }

        public static string GetSaveState()
        {
            return File.ReadAllText(PATH_TO_SAVE_STATE);
        }

        public void SavePlayerData(string currentSceneId)
        {
            CurrentSceneId = currentSceneId;
            WritePlayerToFile();
        }

        public void ResetPlayerData(string firstSceneId)
        {
            CurrentSceneId = firstSceneId;
            SetBatteryToFullPower();
            WritePlayerToFile();
        }

        private void WritePlayerToFile()
        {
            string playerDataAsJson = ConvertPlayerToJson();
            File.WriteAllText(PATH_TO_SAVE_STATE, playerDataAsJson);
        }

        private string ConvertPlayerToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void SetBatteryToFullPower()
        {
            this.BatteryPower = MAXIMUM_BATTERY_POWER;
        }

        public void IncrementBatteryPowerByOne()
        {
            this.BatteryPower++;
        }

        public void DecrementBatteryPowerByOne()
        {
            this.BatteryPower--;
        }

        /// Add LovePoints to LovePointTotal. 
        /// Domain of num: [-50,50]. 
        /// Currently, adding "0" Lovepoints sets LovePointsTotal to 0.
        /// 

        public void AddLovePoints(int num)
        {
            //Limit change in points
            if (num == 0)
            {
                this.LovePointTotal = 0;
            }
            else
            {

                int n;

                if (Math.Abs(num) > 50)
                {
                    n = (num < 0) ? -50 : 50;
                }
                else
                {
                    n = num;
                }

                int newTotal = this.LovePointTotal + n;

                if (newTotal > MAXIMUM_LOVEPOINTS)
                {
                    this.LovePointTotal = MAXIMUM_LOVEPOINTS;
                }
                else if (newTotal < -MAXIMUM_LOVEPOINTS)
                {
                    this.LovePointTotal = -MAXIMUM_LOVEPOINTS;
                }
                else
                {
                    this.LovePointTotal = newTotal;
                }
            }
        }
    }
}
