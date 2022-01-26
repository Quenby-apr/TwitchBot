using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    internal class DinoWorld
    {
        private Emotion emotion;
        public int maxDinos = 1;
        public Dictionary<string, int> dinosDict{ get; set; }
        public List<Dinozavr> dinozavrs { get; set; }
        public DinoWorld()
        {
            dinosDict = new Dictionary<string, int>();
            dinozavrs = new List<Dinozavr>();
            emotion = new Emotion();
        }
        public string createDino(string userName, string dinoName)
        {
            if (!dinosDict.ContainsKey(userName))
            {
                dinosDict.Add(userName, 1);
                dinozavrs.Add(new Herbivore(dinoName));
                return userName+", Ваш первый динозавр создан! " + emotion.emotions["joy"]; // дино впервые от человека добавлен
            }
            if (dinosDict.ContainsKey(userName))
            {
                if (dinosDict[userName] < maxDinos)
                {
                    dinosDict[userName] = dinosDict[userName]++;
                    if (dinozavrs.Where(x => x.Name == dinoName).Any())
                    {
                        return userName + ", К сожалению у вас уже есть динозавр " + emotion.emotions["sadness"]; //такое имя уже существует
                    }
                    dinozavrs.Add(new Herbivore(dinoName));
                    return userName + ", Новый динозавр добавлен"; //дино добавлен
                }
                else
                {
                    return userName + ", У вас уже слишком много динозавров"; //ошибка 
                }
            }
            return "Ошибка";
        }

        public string killDino(string dinoName)
        {
            var dinoToDelete = dinozavrs.Where(x => x.Name == dinoName).Select(x => x).First();
            dinozavrs.Remove(dinoToDelete);
            return "Динозавра " + dinoName + "больше нет с нами";
        }
    }
}
