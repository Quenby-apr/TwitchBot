using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot
{
    public abstract class Dinozavr
    {
        public Emotion emotion;
        public string userName { get; set; }
        public string Name { get; set; }
        public int XP { get; set; } //опыт
        public int[] levels = new int[]
        {
            5,20,40,65,90,130,170,215,260,340,500,720,960,1200,1470,1800,2230,2700
        };
        public int Level { get; set; }
        public int Lives { get; set; }
        public int Wins { get; set; }
        public bool Busy { get; set; }

        public Dinozavr(string name)
        {
            XP = 1;
            Level = 0;
            Lives = 1;
            Wins = 0;
            Name = name;
            Busy = false;
            emotion = new Emotion();
        }
        public abstract string dinner();
        public abstract string getLevel();
        public abstract string lvlUp();
    }

    public class Herbivore: Dinozavr
    {
        public int Fruits { get; private set; }
        public Herbivore(string name):base(name)
        {
            Fruits = 0;
            Name = name;
        }

        public override string dinner()
        {
            if (!Busy)
            {
                Random rnd = new Random();
                int value = rnd.Next(0, 10);
                Thread.Sleep(10000);
                Random rnd2 = new Random();
                int xp = rnd2.Next(4, 10);
                XP += xp;
                if (value >= 8)
                {
                    Fruits++;
                    return userName + ", ваш динозавр смог найти замечательный фрукт! А также " + xp + " опыта " + emotion.emotions["joy"];
                }
                else
                {
                    return userName + ", в этот раз фрукты найти не удалось, но мы смогли получить " + xp + " опыта " + emotion.emotions["slightPain"];
                }
            }
            if (Busy)
            {
                return userName + ", ваш динозавр ещё не вернулся, необходимо подождать";
            }
            return "что-то пошло не так";
        }

        public override string getLevel()
        {
            for (int i=0;i<levels.Count();i++)
            {
                if (levels[i] > XP)
                {
                    if (i > Level)
                    {
                        return userName + ", у вашего динозавра сейчас " + Level + " уровень и заработано " + XP + " опыта, вы уже можете поднять свой уровень!";
                    }
                    else
                    {
                        return userName + ", у вашего динозавра сейчас " + Level + " уровень и заработано " + XP + " опыта, до следующего уровня не хватает "+(levels[i]-XP)+" опыта";
                    }
                }
            }
            return "Ошибка логики уровня";
        }

        public override string lvlUp()
        {
            for (int i = 0; i < levels.Count(); i++)
            {
                if (levels[i] > XP)
                {
                    if (i > Level)
                    {
                        Level++;
                        return userName + ", поздравляю, ваш динозавр стал " + Level + " уровня " + emotion.emotions["joy"];
                    }
                    else
                    {
                        return userName + ", к сожалению, вы ещё не можете поднять уровень своего динозавра " + emotion.emotions["slightPain"];
                    }
                }
            }
            return "Ошибка логики поднятия уровня";
        }
    }

    public class Predator: Dinozavr
    {
        private int Kills { get; }
        public Predator(string name) : base(name)
        {
            Name = name;
        }

        public override string dinner()
        {
            throw new NotImplementedException();
        }

        public override string getLevel()
        {
            throw new NotImplementedException();
        }

        public override string lvlUp()
        {
            throw new NotImplementedException();
        }
    }
}
