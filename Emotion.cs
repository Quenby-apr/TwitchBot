using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    public static class Emotion
    {
        public static Dictionary<string, string> emotions;
        static Emotion()
        {
            emotions = new Dictionary<string, string>
            {
                { "sadness", "BibleThump" },
                { "joy", "VoHiYo" },
                { "dropping","PMSTwin"}, //эмоционально поникший
                { "dinoStandart", "ChefFrank "},
                { "predator", "PogBones "}
            };
        }
    }
}
