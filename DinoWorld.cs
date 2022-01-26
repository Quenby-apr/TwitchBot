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
                Random rand = new Random();
                int value = rand.Next(0, 4);
                if (value<3)
                {
                    dinosDict.Add(userName, 1);
                    dinozavrs.Add(new Herbivore(userName,dinoName));
                    return userName + ", Ваш первый динозавр создан, и он травоядный! " + emotion.emotions["joy"]; // дино впервые от человека добавлен
                } else if (value == 4)
                {
                    dinosDict.Add(userName, 1);
                    dinozavrs.Add(new Predator(userName, dinoName));
                    return userName + ", Ваш первый динозавр создан, и он хищный! " + emotion.emotions["predator"]; 
                }
            }
            if (dinosDict.ContainsKey(userName))
            {
                Console.WriteLine(dinosDict[userName] + "ключ");
                if (dinosDict[userName] < maxDinos)
                {
                    if (dinozavrs.Where(x => x.Name == dinoName).Any())
                    {
                        return userName + ", К сожалению у вас уже есть динозавр " + emotion.emotions["sadness"]; //такое имя уже существует
                    }
                    Random rand = new Random();
                    int value = rand.Next(0, 4);
                    if (value < 3)
                    {
                        dinosDict[userName] = dinosDict[userName] + 1;
                        dinozavrs.Add(new Herbivore(userName, dinoName));
                        return userName + ", Новый динозавр добавлен, и он травоядный " + emotion.emotions["joy"]; //дино добавлен   
                    }
                    else if (value == 3)
                    {
                        dinosDict[userName] = dinosDict[userName] + 1;
                        dinozavrs.Add(new Predator(userName, dinoName));
                        return userName + ", Новый динозавр добавлен, и он хищный! " + emotion.emotions["predator"];
                    } 
                }
                else
                {
                    return userName + ", У вас уже слишком много динозавров"; //ошибка 
                }
            }
            return "Ошибка";
        }

        public string killDino(string userName, string dinoName)
        {
            if (!dinosDict.ContainsKey(userName))
            {
                return userName + ", у вас и так нет ни единого динозавра " + emotion.emotions["sadness"];
            }
            else if (dinosDict[userName]!=0)
            {
                dinosDict[userName] = dinosDict[userName] - 1;
                Console.WriteLine(dinosDict[userName] +"ключ удаления");
                dinozavrs.RemoveAll(x => x.Name == dinoName && x.userName == userName);
                return "Динозавра " + dinoName + " больше нет с нами";
            }
            else
            {
                return userName+", у вас и так нет ни единого динозавра " + emotion.emotions["sadness"];
            } 
        }
    }
}
