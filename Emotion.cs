using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class Emotion
    {
        public Dictionary<string, string> emotions;
        public Emotion()
        {
            emotions = new Dictionary<string, string>
            {
                { "sadness", "BibleThump" },
                { "joy", "VoHiYo" },
                { "dropping","PMSTwin"}, //эмоционально поникший
                { "dinoStandart", "ChefFrank "}
            };
        }
    }
}
