using System;
using NetCoreAudio;
namespace TheSyndicate
{
    public class Voice
    {
        public static Player pl = new Player();

        public static void PlayMusic(string sceneID)
        {
            pl.Play(Program.ASSETS_PATH + sceneID + ".wav");

        }

        public static void StopMusic()
        {
            pl.Stop();
        }

    }
}
